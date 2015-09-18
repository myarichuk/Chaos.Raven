using Chaos.Raven.Common;
using System.Linq;
using Raven.Client;

namespace Chaos.Raven.Actions
{
    public class DeleteDocumentsInSession : ChaosAction
    {
        public override void DoSomeChaos(IDocumentStore store)
        {
            var docs = store.DatabaseCommands.StartsWith("companies",null,0,Constants.SmallBatchSize);

            using (var session = store.OpenSession())
            {
                foreach (var key in docs.Select(x => x.Key))
                    session.Delete(key);
                session.SaveChanges();
            }
        }
    }
}
