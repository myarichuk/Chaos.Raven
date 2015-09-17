using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using FizzWare.NBuilder;
using Raven.Client.Indexes;
using Chaos.Raven.Common;

namespace Chaos.Raven.Actions
{
    public class VerifyBulkInsertDocs : BaseVerificationAction
    {
        List<DummyUser> dummyUsers = new List<DummyUser>();

        protected override void GenerateData()
        {
            dummyUsers = Builder<DummyUser>.CreateListOfSize(Constants.SmallBatchSize).Build().ToList();
        }

        protected override bool DoAction(IDocumentStore store)
        {            
            new DummyUserIndex().Execute(store); //this will make sure the index is there
            using (var bulkInsert = store.BulkInsert())
                dummyUsers.ForEach(user => bulkInsert.Store(user));

            WaitForIndexing(store);

            using (var session = store.OpenSession())
            using (var stream = session.Advanced.Stream(session.Query<DummyUser, DummyUserIndex>()))
            {
                do
                {
                    if (!dummyUsers.Any(x => x.Id == stream.Current.Key))
                        return false;
                } while (stream.MoveNext());
            }

            return true;
        }

        #region Document and Index definitions

        public class DummyUser
        {
            public string Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class DummyUserIndex : AbstractIndexCreationTask<DummyUser>
        {
            public DummyUserIndex()
            {
                Map = users => from user in users
                               select new
                               {
                                   user.Id,
                                   user.FirstName,
                                   user.LastName
                               };
            }
        }

        #endregion
    }
}
