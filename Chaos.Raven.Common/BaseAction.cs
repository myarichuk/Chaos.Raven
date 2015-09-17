using Raven.Client;
using System;
using System.Threading;

namespace Chaos.Raven.Common
{
    public abstract class BaseAction
    {
        public event Action<Exception> ExceptionThrown;
        protected static bool WaitForIndexing(IDocumentStore store, int millisecondsTimeout = 1000 * 30)
        {
            return SpinWait.SpinUntil(() =>
            {
                return store.DatabaseCommands.GetStatistics().StaleIndexes.Length == 0;
            }, millisecondsTimeout);
        }

        public virtual void DoSomeChaos(IDocumentStore store)
        {

        }

        protected void OnExceptionThrown(Exception e)
        {
            var exceptionThrown = ExceptionThrown;
            if (exceptionThrown != null)
                exceptionThrown(e);
        }
    }
}
