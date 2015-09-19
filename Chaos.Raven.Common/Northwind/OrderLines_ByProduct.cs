using Orders;
using Raven.Client.Indexes;
using System.Linq;

namespace Chaos.Raven.Northwind
{
    public class OrderLines_ByProduct : AbstractIndexCreationTask<Order>
    {
        public OrderLines_ByProduct()
        {
            Map = orders => from order in orders
                            from orderLine in order.Lines
                            select new
                            {
                                orderLine.Product,
                                orderLine.ProductName,
                                Employee = LoadDocument<Employee>(order.Employee).FirstName + " " + LoadDocument<Employee>(order.Employee).LastName,
                                Company = LoadDocument<Company>(order.Company).Name
                            };
        }
    }
}
