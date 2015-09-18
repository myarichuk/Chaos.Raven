using Raven.Client;
using System;
using System.Threading;

namespace Chaos.Raven.Common
{
    public abstract class ChaosAction : BaseAction
    {
        public abstract void DoSomeChaos(IDocumentStore store);
    }
}
