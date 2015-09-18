using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Indexes;
using Orders;
using Raven.Abstractions.Data;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class VerifyIndexing : VerificationAction
    {
        #region Index Definition

        public class OrdersIndex : AbstractIndexCreationTask<Order>
        {
            public OrdersIndex()
            {
                Map = orders => from order in orders
                                select new
                                {
                                    order.Id,
                                    order.OrderedAt,
                                    order.ShippedAt,
                                    order.Freight
                                };
            }
        }

        #endregion        

        protected override bool DoAction(IDocumentStore store)
        {
            var ordersIndex = new OrdersIndex();
            ordersIndex.Execute(store);

            for (int k = 0; k < 2; k++)
            {
                using (var session = store.OpenSession())
                {
                    var random = new Random(DateTime.UtcNow.Millisecond);
                    for (int i = 0; i < Constants.SmallBatchSize; i++) //save one by one on purpose
                        session.Store(DataFactory.Orders.GenerateOne());

                    session.SaveChanges();
                }
            }

            WaitForIndexing(store);

            //cleanup after the test
            using (var session = store.OpenSession())
            {
                var relevantOrderCount = session.Query<Order, OrdersIndex>()
                                                .Count(x => x.OrderedAt > DateTime.UtcNow);

                var op = store.DatabaseCommands.DeleteByIndex(ordersIndex.IndexName, new IndexQuery());
                op.WaitForCompletion();
                return relevantOrderCount == Constants.SmallBatchSize;
            }

        }
    }
}
