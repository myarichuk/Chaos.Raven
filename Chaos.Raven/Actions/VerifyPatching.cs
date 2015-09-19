using Chaos.Raven.Common;
using Orders;
using Raven.Abstractions.Data;
using Raven.Client;
using System;

namespace Chaos.Raven.Actions
{
    public class VerifyPatching : VerificationAction
    {
        protected override bool DoAction(IDocumentStore store)
        {
            var docsToPatch = store.DatabaseCommands.StartsWith("orders", null, 0, Constants.SmallBatchSize);
            var testValue = $"TestValue {Guid.NewGuid()}";
            foreach(var doc in docsToPatch)
            {
                store.DatabaseCommands.Patch(doc.Key, new PatchRequest[]
                {
                   new PatchRequest
                   {
                       Type = PatchCommandType.Modify,
                       Name = "ShipVia",
                       Value = testValue
                   }
                });
            }

            var hasSucceeded = true;
            //now verify
            using (var session = store.OpenSession())
            {
                foreach (var doc in docsToPatch)
                {
                    var loadedDoc = session.Load<Order>(doc.Key);
                    if(!loadedDoc.ShipVia.Equals(testValue))
                        hasSucceeded = false;
                    loadedDoc.ShipVia = doc.DataAsJson.Value<string>("ShipVia");
                }

                session.SaveChanges();
            }
            return hasSucceeded;
        }
    }
}
