
using Elephanet.Conventions;

namespace Elephanet.Tests
{
    public class TestStore : DocumentStore
    {
        private const string TestDbConnectionString = "Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;";

        public TestStore() : base(TestDbConnectionString, new StoreConventions())
        { }

        public TestStore(IStoreConventions storeConventions) : base(TestDbConnectionString, storeConventions)
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
