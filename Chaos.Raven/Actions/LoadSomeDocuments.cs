using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using System.ComponentModel.Composition;
using System.Threading;
using Orders;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class LoadSomeDocuments : ChaosAction
    {
        int isInitialized;
        readonly List<string> orderIDs;
        readonly Random random;

        public LoadSomeDocuments()
        {
            random = new Random(123);
            orderIDs = new List<string>();
        }

        public override void DoSomeChaos(IDocumentStore store)
        {
            if(Interlocked.CompareExchange(ref isInitialized,1,0) == 0)
            {
                using (var session = store.OpenSession())
                {
                    var queryResult = session.Query<Order>().Select(x => x.Id).ToList();
                    orderIDs.AddRange(queryResult);
                }
            }
            else
            {
                var val = random.Next();
                if (val % 4 == 0)
                    LoadDocs(store);
                else if (val % 3 == 0)
                    LoadDocsLazy(store);
                else
                    GetWithDatabaseCommands(store);
            }
        }

        private void GetWithDatabaseCommands(IDocumentStore store)
        {
            orderIDs.ForEach(id => store.DatabaseCommands.Get(id));
        }

        private void LoadDocsLazy(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                orderIDs.ForEach(id => session.Advanced.Lazily.Load<Order>(id));
                session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
            }
        }

        private void LoadDocs(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                session.Load<Order>(orderIDs);
            }
        }
    }
}
