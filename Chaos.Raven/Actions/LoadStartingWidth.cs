using System.ComponentModel.Composition;
using Raven.Client;
using Orders;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class LoadStartingWidth : ChaosAction
    {
        public override void DoSomeChaos(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                session.Advanced.LoadStartingWith<Company>("companies", pageSize:256);
            }
        }
    }
}
