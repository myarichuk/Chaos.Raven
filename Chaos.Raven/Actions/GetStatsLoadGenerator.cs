using Chaos.Raven.Common;
using Raven.Client;

namespace Chaos.Raven.Actions
{
    public class GetStatsLoadGenerator : LoadGenerationAction
    {
        protected override void LoadGenerationToExecuteInLoop(IDocumentStore store)
        {
            store.DatabaseCommands.GetStatistics();
        }
    }
}
