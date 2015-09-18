using Raven.Client;
using System.ComponentModel.Composition;
using Orders;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class InsertDocumentsInSession : ChaosAction
    {
        public override void DoSomeChaos(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                for(int i = 0; i < Constants.SmallBatchSize; i++)
                    session.Store(DataFactory.Companies.GenerateOne());
                session.SaveChanges();
            }
        }
    }
}
