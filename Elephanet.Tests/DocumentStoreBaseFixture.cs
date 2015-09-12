using System;

namespace Elephanet.Tests
{
    /// <summary>
    /// Use this as an IClassFixture to inject the TestStore into other test fixtures
    /// </summary>
    public class DocumentStoreBaseFixture : IDisposable
    {
        public DocumentStoreBaseFixture()
        {
            TestStore = new TestStore();
        }

        public TestStore TestStore { get; private set; }

        public void Dispose()
        {
            TestStore.Destroy();
        }
    }
}