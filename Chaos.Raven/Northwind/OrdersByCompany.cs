using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Chaos.Raven.Northwind
{

    public class OrdersByCompany : AbstractIndexCreationTask
    {
        public override string IndexName
        {
            get
            {
                return "Orders/ByCompany";
            }
        }
        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
            {
                Map = @"from order in docs.Orders
select new { order.Company, Count = 1, Total = order.Lines.Sum(l=>(l.Quantity * l.PricePerUnit) *  ( 1 - l.Discount)) }",
                Reduce = @"from result in results
group result by result.Company into g
select new
{
	Company = g.Key,
	Count = g.Sum(x=>x.Count),
	Total = g.Sum(x=>x.Total)
}"
            };
        }
    }
}


