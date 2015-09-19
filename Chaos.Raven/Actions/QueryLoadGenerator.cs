using Chaos.Raven.Common;
using Raven.Client;
using Raven.Abstractions.Data;

namespace Chaos.Raven.Actions
{
    public class QueryLoadGenerator : LoadGenerationAction
    {
        private const string indexName = "Orders/ByCompany";
        protected override void LoadGenerationToExecuteInLoop(IDocumentStore store)
        {
            store.DatabaseCommands.Query(indexName, new IndexQuery());
        }
    }
}
