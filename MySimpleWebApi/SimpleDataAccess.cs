using System.Collections.Generic;
using System.Linq;
using Marten;
using MySimpleWebApi.Controllers;

namespace MySimpleWebApi
{
    internal class SimpleDataAccess
    {
        private readonly string _connectionString;

        public SimpleDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDocumentStore Store
        {
            get
            {
                var store = DocumentStore.For(_ =>
                {
                    _.Connection(_connectionString);
                });
                return store;
            }
        }

        public IEnumerable<string> GetAllValues()
        {
            using (var session = Store.QuerySession())
            {
                return session.Query<SimpleValue>().Select(simpleValue => simpleValue.Value).ToArray();
            }
        }

        public string GetValueById(int id)
        {
            using (var session = Store.QuerySession())
            {
                var simpleValue = session.Load<SimpleValue>(id);

                return simpleValue?.Value;
            }
        }

        public int AddNewValue(string value)
        {
            var newSimpleValue = new SimpleValue { Value = value };

            using (var session = Store.OpenSession())
            {
                session.Store(newSimpleValue);

                session.SaveChanges();
            }

            return newSimpleValue.Id;
        }

        public void AddNewValue(SimpleValue simpleValue)
        {
            using (var session = Store.OpenSession())
            {
                session.Store(simpleValue);

                session.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (var session = Store.OpenSession())
            {
                session.Delete<SimpleValue>(id);

                session.SaveChanges();
            }
        }
    }
}
