using Chaos.Raven.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;

namespace Chaos.Raven.Actions
{
    public class ResetAllIndexes : ChaosAction
    {
        public override void DoSomeChaos(IDocumentStore store)
        {
            var stats = store.DatabaseCommands.GetStatistics();
            foreach(var index in stats.Indexes)
                store.DatabaseCommands.ResetIndex(index.Name);
        }
    }
}
