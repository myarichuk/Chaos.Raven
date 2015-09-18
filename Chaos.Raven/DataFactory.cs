using Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;

namespace Chaos.Raven
{
    public static class DataFactory
    {
        internal class IdFactory
        {
            private readonly int maxId;
            private readonly string collectionName;

            public IdFactory(string collectionName,int maxId = 100)
            {
                this.collectionName = collectionName;
                this.maxId = maxId;
            }

            public string RandomId()
            {
                return $"{collectionName}/{new Random(DateTime.UtcNow.Millisecond).Next(1,maxId)}";
            }
        }

        public static class Companies
        {
            public static IEnumerable<Company> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }

            public static Company GenerateOne()
            {
                return Builder<Company>.CreateNew()
                                       .With(x => x.Address = Addresses.GenerateOne())
                                       .With(x => x.Contact = new Contact
                                       {
                                           Name = Faker.Name.FullName(),
                                           Title = "Chief Contact Person"
                                       })
                                       .With(x => x.Fax = Faker.Phone.Number())
                                       .With(x => x.Phone = Faker.Phone.Number())
                                       .With(x => x.Id = null)
                                       .Build();
            }
        }

        public static class Addresses
        {
            public static Address GenerateOne()
            {
                return Builder<Address>.CreateNew()
                                            .With(x => x.City = Faker.Address.City())
                                            .With(x => x.Country = Faker.Address.Country())
                                            .With(x => x.Region = null)
                                            .With(x => x.PostalCode = Faker.Address.ZipCode())
                                            .With(x => x.Line1 = Faker.Address.StreetAddress())
                                            .With(x => x.Line2 = Faker.Lorem.Sentence())
                                       .Build();
            }
        }

        public static class Orders
        {
            private static IdFactory productsIdFactory = new IdFactory("products",75);
            private static IdFactory employeesIdFactory = new IdFactory("employees",9);
            private static IdFactory companiesIdFactory = new IdFactory("companies",90);
            private static RandomGenerator random = new RandomGenerator(DateTime.UtcNow.Millisecond);

            public static IEnumerable<Order> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }

            public static Order GenerateOne()
            {
                return Builder<Order>.CreateNew()
                                        .With(x => x.Company = companiesIdFactory.RandomId())
                                        .With(x => x.Employee = employeesIdFactory.RandomId())
                                        .With(x => x.Id = null)
                                        .With(x => x.Freight = random.Next(0.1m,100.0m))
                                        .With(x => x.Lines = Builder<OrderLine>.CreateListOfSize(random.Next(1,10))
                                                                               .All()
                                                                                    .With(ix => ix.Product = productsIdFactory.RandomId())
                                                                                    .With(ix => ix.ProductName = Faker.Lorem.Sentence())
                                                                                    .With(ix => ix.PricePerUnit = random.Next(0.5m,50m))
                                                                                    .With(ix => ix.Discount = random.Next(0.0m,25.0m))
                                                                                    .With(ix => ix.Quantity = random.Next(1,100000))
                                                                                .Build().ToList())
                                                                                            
                                                                                    
                                        .Build();
                                    
            }
        }
    }
}
