using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Indexes;
using Orders;
using Raven.Abstractions.Data;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class VerifyIndexing : BaseVerificationAction
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
            using (var session = store.OpenSession())
            {
                var random = new Random(123);
                for(int i = 0; i < Constants.SmallBatchSize; i++)
                {
                    session.Store(new Order
                    {
                        OrderedAt = DateTime.UtcNow.AddDays(3),
                        ShippedAt = DateTime.UtcNow.AddDays(3),
                        RequireAt = DateTime.UtcNow.AddDays(3),
                        Employee = "employees/1",
                        Company = "companies/1",
                        Freight = random.Next(),
                        Lines = new List<OrderLine>
                        {
                            new OrderLine
                            {
                                PricePerUnit = 1.1m,
                                Product = "products/1",
                                Quantity = random.Next(1,50),
                                ProductName = Faker.Lorem.Sentence(2)
                            }
                        }
                    });
                }

                session.SaveChanges();
            }

            WaitForIndexing(store);

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
