using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Chaos.Raven.Northwind
{
    public class OrdersTotals : AbstractIndexCreationTask
    {
        public override string IndexName
        {
            get
            {
                return "Orders/Totals";
            }
        }
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
            {
                Map = @"from order in docs.Orders
select new { order.Employee,  order.Company, Total = order.Lines.Sum(l=>(l.Quantity * l.PricePerUnit) *  ( 1 - l.Discount)) }",
                SortOptions =  {
                {
                    "Total",
                    SortOptions.Double
                }
            }
            };
        }
    }

}


