using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Chaos.Raven.Northwind
{
    public class ProductSales : AbstractIndexCreationTask
    {
        public override string IndexName
        {
            get
            {
                return "Product/Sales";
            }
        }
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
            {
                Map = @"from order in docs.Orders
from line in order.Lines
select new { Product = line.Product, Count = 1, Total = ((line.Quantity * line.PricePerUnit) *  ( 1 - line.Discount)) }",
                Reduce = @"from result in results
group result by result.Product into g
select new
{
	Product = g.Key,
	Count = g.Sum(x=>x.Count),
	Total = g.Sum(x=>x.Total)
}",
                MaxIndexOutputsPerDocument = 30
            };
        }
    }
}



