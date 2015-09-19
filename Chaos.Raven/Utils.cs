using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Smuggler;
using Raven.Abstractions.Data;
using Raven.Abstractions.Util;
using Raven.Client.Connection.Async;
using Chaos.Raven.Northwind;

namespace Chaos.Raven
{
    public static class Utils
    {
        public static void CreateInitialData(IDocumentStore store, string databaseName)
        {
            store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(databaseName);
            using (var bulkInsert = store.BulkInsert())
            {
                foreach (var category in DataFactory.Categories.GenerateMany(Constants.NumOfCategories))
                    bulkInsert.Store(category);

                foreach (var company in DataFactory.Companies.GenerateMany(Constants.NumOfCompanies))
                    bulkInsert.Store(company);

                foreach (var employee in DataFactory.Employees.GenerateMany(Constants.NumOfEmployees))
                    bulkInsert.Store(employee);

                foreach (var order in DataFactory.Orders.GenerateMany(Constants.NumOfOrders))
                    bulkInsert.Store(order);

                foreach (var product in DataFactory.Products.GenerateMany(Constants.NumOfProducts))
                    bulkInsert.Store(product);

                foreach (var region in DataFactory.Regions.GenerateMany(Constants.NumOfRegions))
                    bulkInsert.Store(region);

                foreach (var shipper in DataFactory.Shippers.GenerateMany(Constants.NumOfShippers))
                    bulkInsert.Store(shipper);

                foreach (var supplier in DataFactory.Suppliers.GenerateMany(Constants.NumOfSuppliers))
                    bulkInsert.Store(supplier);
            }

            new OrdersByCompany().Execute(store);
            new OrdersTotals().Execute(store);
            new ProductSales().Execute(store);
            new OrderLines_ByProduct().Execute(store);
        }

        public static void ExecuteIndexes(IDocumentStore store)
        {
            new OrdersByCompany().Execute(store);
            new OrdersTotals().Execute(store);
            new ProductSales().Execute(store);
            new OrderLines_ByProduct().Execute(store);
        }
    }
}
