
namespace Elephanet.Tests
{
    public class TestStore : DocumentStore
    {
        public TestStore() : base("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;")
        { }
    }
}
