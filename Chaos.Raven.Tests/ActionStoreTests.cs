using Chaos.Raven;
using System;
using Xunit;
using Raven.Client;
using FluentAssertions;
using System.ComponentModel.Composition;
using Chaos.Raven.Common;
using Raven.Tests.Helpers;
using Raven.Database.Config;

namespace Chaos.Raven.Tests
{
    public class ActionStoreTests : RavenTestBase
    {
        readonly ActionStore store;

        public ActionStoreTests()
        {
            store = new ActionStore(@".\Actions");
        }

        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            configuration.Storage.Voron.AllowOn32Bits = true;
        }

        [Fact]
        public void Action_store_can_resolve_chaos_actions()
        {
            using (var ravenStore = NewRemoteDocumentStore())
            {
                    var action = store.GetRandomChaosAction();
                    action.Invoking(x => x.DoSomeChaos(ravenStore))
                          .ShouldNotThrow();
            }
        }

        [Fact]
        public void Action_store_can_resolve_verification_actions()
        {
            using (var ravenStore = NewRemoteDocumentStore())
            {
                var action = store.GetRandomVerificationAction();
                long elapsed;
                action.Invoking(x => x.VerifyAction(ravenStore, out elapsed))
                      .ShouldNotThrow();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (store != null)
                store.Dispose();
        }
    }
}
