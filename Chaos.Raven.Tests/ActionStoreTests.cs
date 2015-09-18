using Xunit;
using FluentAssertions;
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
        public void CreateInitialData_works()
        {
            using (var store = NewRemoteDocumentStore())
            {
                this.Invoking(_ => Utils.CreateInitialData(store, store.DefaultDatabase))
                    .ShouldNotThrow();

                var stats = store.DatabaseCommands.GetStatistics();
                stats.CountOfIndexes.Should().BeGreaterThan(1);
                stats.CountOfDocuments.Should().BeGreaterThan(0);
            }
        }

        [Fact]
        public void Action_store_can_resolve_chaos_actions()
        {
            using (var ravenStore = NewRemoteDocumentStore())
            {
                Utils.CreateInitialData(ravenStore, ravenStore.DefaultDatabase);
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
                Utils.CreateInitialData(ravenStore, ravenStore.DefaultDatabase);
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
