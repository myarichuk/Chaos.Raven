using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using FizzWare.NBuilder;
using Raven.Client.Indexes;
using Chaos.Raven.Common;
using Orders;

namespace Chaos.Raven.Actions
{
    public class VerifyBulkInsertDocs : BaseVerificationAction
    {
        protected override bool DoAction(IDocumentStore store)
        {
            var lotsOfOrders = DataFactory.Orders.GenerateMany(Constants.MediumBatchSize).ToList();
            using (var bulkInsert = store.BulkInsert())
                lotsOfOrders.ForEach(order => bulkInsert.Store(order));

            WaitForIndexing(store);

            using (var session = store.OpenSession())
            using (var stream = session.Advanced.Stream(session.Query<Order>("Orders/Totals")))
            {
                do
                {
                    if (!lotsOfOrders.Any(x => x.Id == stream.Current.Key))
                        return false;
                } while (stream.MoveNext());
            }

            return true;
        }
    }
}
