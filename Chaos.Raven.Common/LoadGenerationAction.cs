using Raven.Client;
using System.Diagnostics;

namespace Chaos.Raven.Common
{
    public abstract class LoadGenerationAction : BaseAction
    {
        protected abstract void LoadGenerationToExecuteInLoop(IDocumentStore store);

        public void GenerateLoad(IDocumentStore store, int loadDurationInMilliseconds)
        {
            var sw = Stopwatch.StartNew();
            do
            {
                LoadGenerationToExecuteInLoop(store);
            } while (sw.ElapsedMilliseconds < loadDurationInMilliseconds);        
        }
    }
}
