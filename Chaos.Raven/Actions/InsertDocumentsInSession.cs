using Raven.Client;
using System.ComponentModel.Composition;
using Orders;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class InsertDocumentsInSession : BaseAction
    {
        public override void DoSomeChaos(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                for(int i = 0; i < Constants.SmallBatchSize; i++)
                {
                    session.Store(new Company
                    {
                        Name = Faker.Company.Name(),
                        Address = new Address
                        {
                            City = Faker.Address.City(),
                            Country = Faker.Address.Country(),
                            PostalCode = Faker.Address.ZipCode()
                        },
                        Contact = new Contact
                        {
                            Name = Faker.Name.FullName(),
                            Title = "Dummy Title"
                        }
                    });
                }
                session.SaveChanges();
            }
        }
    }
}
