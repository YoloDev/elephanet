using Elephanet.Conventions;

namespace Elephanet.Tests.Infrastructure
{
    public class TestStore : DocumentStore
    {
        private const string TestDbConnectionString = "Server=127.0.0.1;Port=5432;User id=elephanet_tests_user;password=my super secret password;database=elephanet_tests_store;";

        static TestStore()
        {
            TestStoreDatabaseFactory.CreateCleanStoreDatabase();
        }

        public TestStore() : this(new StoreConventions())
        {
        }

        private TestStore(IStoreConventions storeConventions) : base(TestDbConnectionString, storeConventions)
        {
        }

        /// <summary>
        /// Factory method for creating a configured TestStore
        /// </summary>
        /// <returns></returns>
        public static TestStore CreateStoreWithEntityNotFoundBehaviorReturnNull()
        {
            var conventions = new StoreConventions();
            conventions.SetEntityNotFoundBehavior(EntityNotFoundBehavior.ReturnNull);
            return new TestStore(conventions);
        }
    }
}
