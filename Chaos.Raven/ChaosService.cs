using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using System.Threading;
using System.Collections.Concurrent;
using Chaos.Raven.Common;

namespace Chaos.Raven
{
    public class ChaosService : IDisposable
    {
        private readonly int numOfActionsPerDispatch;
        private readonly string databaseName;
        private readonly IDocumentStore documentStore;
        private Timer actionDispatchTimer;
        private readonly object syncObj = new object();
        private readonly ActionStore actionStore;
        private readonly CancellationTokenSource cts;
        private volatile bool isInitialized;
        private readonly SemaphoreSlim concurrentActionsSemaphore;

        public static ConcurrentDictionary<BaseAction,DateTime> ExecutingActions { get; private set; }
        public static ConcurrentDictionary<Exception, DateTime> ActionErrors { get; private set; }        

        static ChaosService()
        {
            ExecutingActions = new ConcurrentDictionary<BaseAction, DateTime>();
            ActionErrors = new ConcurrentDictionary<Exception, DateTime>();
        }

        public ChaosService(IDocumentStore documentStore, string databaseName,string actionsFolder = Constants.ActionsPluginFolder, int? numOfActionsPerDispatch = null)
        {
            this.documentStore = documentStore;
            this.databaseName = databaseName;
            this.numOfActionsPerDispatch = numOfActionsPerDispatch ?? Constants.NumOfActionsPerDispatch;
            actionStore = new ActionStore(actionsFolder);
            cts = new CancellationTokenSource();
            concurrentActionsSemaphore = new SemaphoreSlim(Constants.MaxConcurrentActions);
        }

        public void Start()
        {
            documentStore.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(databaseName);
            var stats = documentStore.DatabaseCommands.GetStatistics();
            if (!isInitialized)
            {
                if (stats.CountOfDocuments == 0)
                    Task.Run(() => Utils.CreateInitialData(documentStore, databaseName));
                    
                isInitialized = true;
            }
            try 
            {
                if (!Monitor.TryEnter(syncObj, 5000))
                    throw new TimeoutException(@"Timed-out while trying to start Raven.Chaos service.
                                This should not happen - something is holding the lock on the service for too long");
                actionDispatchTimer = new Timer(state => DispatchActions(), null, 0, Constants.ActionDispatchFrequencyInMilliseconds);
            }
            finally
            {
                Monitor.Exit(syncObj);
            }
        }

        private void DispatchActions()
        {            
            Parallel.For(0, numOfActionsPerDispatch, i =>
            {
                if (!concurrentActionsSemaphore.Wait(1000, cts.Token))
                    return; //too much concurrent actions, nothing to do

                var useVerificationAction = new Random(DateTime.UtcNow.Millisecond).Next() % 5 == 0;
                cts.Token.ThrowIfCancellationRequested();

                if (useVerificationAction)
                {
                    var action = actionStore.GetRandomVerificationAction();
                    ExecutingActions.GetOrAdd(action,DateTime.UtcNow);

                    long elapsed;
                    try
                    {
                        action.VerifyAction(documentStore, out elapsed);
                    }
                    catch (Exception e)
                    {
                        ActionErrors.AddOrUpdate(e, DateTime.UtcNow, (k, v) => DateTime.UtcNow);
                    }
                    finally
                    {
                        DateTime whenStarted;
                        ExecutingActions.TryRemove(action, out whenStarted);
                        concurrentActionsSemaphore.Release();
                    }
                }
                else
                {
                    var action = actionStore.GetRandomChaosAction();
                    ExecutingActions.GetOrAdd(action, DateTime.UtcNow);

                    try
                    {
                        action.DoSomeChaos(documentStore);
                    }
                    catch (Exception e)
                    {
                        ActionErrors.AddOrUpdate(e, DateTime.UtcNow, (k, v) => DateTime.UtcNow);
                    }
                    finally
                    {
                        DateTime whenStarted;
                        ExecutingActions.TryRemove(action, out whenStarted);
                        concurrentActionsSemaphore.Release();
                    }
                }

            });
        }

        public void Stop()
        {
            try
            {
                if (!Monitor.TryEnter(syncObj, 5000))
                    throw new TimeoutException(@"Timed-out while trying to start Raven.Chaos service.
                                This should not happen - something is holding the lock on the service for too long");
                actionDispatchTimer.Dispose();
            }
            finally
            {
                Monitor.Exit(syncObj);
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            actionDispatchTimer.Dispose();
            actionStore.Dispose();
        }
    }
}
