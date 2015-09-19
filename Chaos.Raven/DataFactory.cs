using Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;

#pragma warning disable CS0612 // Type or member is obsolete

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

        public static class Categories
        {
            public static IEnumerable<Category> GenerateMany(int size)
            {
                return Builder<Category>.CreateListOfSize(size).All().With(x => x.Id = null).Build().ToList();
            }
        }

        public static class Products
        {
            private static RandomGenerator random = new RandomGenerator(DateTime.UtcNow.Millisecond);
            private static IdFactory supplierIdFactory = new IdFactory("suppliers", Constants.NumOfSuppliers);

            public static IEnumerable<Product> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }

            public static Product GenerateOne()
            {
                return Builder<Product>.CreateNew()
                                        .With(x => x.Id = null)
                                        .With(x => x.PricePerUnit = random.Next(0.5m,1000m))
                                        .With(x => x.QuantityPerUnit = "Kilograms")
                                        .With(x => x.Name = Faker.Lorem.GetSentence())
                                        .With(x => x.UnitsInStock = random.Next(5,5000000))
                                        .With(x => x.UnitsOnOrder = random.Next(0,1000000))
                                        .With(x => x.Supplier = supplierIdFactory.RandomId())    
                                        .With(x => x.ReorderLevel = random.Next(1,10))                               
                                       .Build();
            }
        }

        public static class Shippers
        {
            public static Shipper GenerateOne()
            {
                return Builder<Shipper>.CreateNew()
                                       .With(x => x.Id = null)
                                       .With(x => x.Name = Faker.Company.GetName())
                                       .With(x => x.Phone = Faker.PhoneNumber.GetPhoneNumber())
                                       .Build();
            }

            public static IEnumerable<Shipper> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }
        }

       public static class Regions
       {            
            public static Region GenerateOne()
            {
                return Builder<Region>.CreateNew()
                                      .With(x => x.Id = null)
                                      .With(x => x.Territories = Builder<Territory>.CreateListOfSize(new Random().Next(1,10))                                                                                   
                                                                                   .Build()
                                                                                   .ToList())    
                                      .Build();
            }

            public static IEnumerable<Region> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }
        }

        public static class Employees
        {
            private static IdFactory employeeIdFactory = new IdFactory("employees",Constants.NumOfEmployees);

            public static Employee GenerateOne()
            {
                return Builder<Employee>.CreateNew()
                                            .With(x => x.Id = null)
                                            .With(x => x.HomePhone = Faker.PhoneNumber.GetPhoneNumber())
                                            .With(x => x.FirstName = Faker.Name.GetFirstName())
                                            .With(x => x.LastName = Faker.Name.GetLastName())
                                            .With(x => x.Title = Faker.Company.GetPosition())
                                            .With(x => x.Notes = new List<string> { Faker.Lorem.GetSentence(6) })
                                            .With(x => x.Address = Addresses.GenerateOne())
                                            .With(x => x.ReportsTo = employeeIdFactory.RandomId())
                                            .With(x => x.Territories = new List<string>())
                                        .Build();
            }

            public static IEnumerable<Employee> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }
        }

        public static class Suppliers
        {
            public static IEnumerable<Supplier> GenerateMany(int size)
            {
                for (int i = 0; i < size; i++)
                    yield return GenerateOne();
            }

            public static Supplier GenerateOne()
            {
                return Builder<Supplier>.CreateNew()
                                        .With(x => x.Address = Addresses.GenerateOne())
                                        .With(x => x.Contact = Contacts.GenerateOne())
                                        .With(x => x.Fax = Faker.PhoneNumber.GetPhoneNumber())
                                        .With(x => x.Id = null)
                                        .With(x => x.HomePage = Faker.Internet.HttpUrl())
                                        .With(x => x.Name = Faker.Company.GetName())
                                        .With(x => x.Phone = Faker.PhoneNumber.GetPhoneNumber())
                                        .Build();
            }
        }

        public static class Contacts
        {
            public static Contact GenerateOne()
            {
                return new Contact
                {
                    Name = Faker.Name.GetName(),
                    Title = Faker.Company.GetPosition()
                };
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
                                       .With(x => x.Contact = Contacts.GenerateOne())
                                       .With(x => x.Fax = Faker.PhoneNumber.GetPhoneNumber())
                                       .With(x => x.Phone = Faker.PhoneNumber.GetPhoneNumber())
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
                                            .With(x => x.Country = Faker.Address.GetCountry())
                                            .With(x => x.Region = null)
                                            .With(x => x.PostalCode = Faker.Address.ZipCode())
                                            .With(x => x.Line1 = Faker.Address.StreetAddress())
                                            .With(x => x.Line2 = Faker.Lorem.Sentence())
                                       .Build();
            }
        }

        public static class Orders
        {
            private static IdFactory productsIdFactory = new IdFactory("products",Constants.NumOfProducts);
            private static IdFactory employeesIdFactory = new IdFactory("employees",Constants.NumOfEmployees);
            private static IdFactory companiesIdFactory = new IdFactory("companies",Constants.NumOfCompanies);
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
