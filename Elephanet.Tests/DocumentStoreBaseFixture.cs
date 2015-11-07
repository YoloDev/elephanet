using System;
using Elephanet.Tests.Infrastructure;

namespace Elephanet.Tests
{
    /// <summary>
    /// Use this as an IClassFixture to inject the TestStore into other test fixtures
    /// </summary>
    public class DocumentStoreBaseFixture
    {
        public DocumentStoreBaseFixture()
        {
            TestStore = new TestStore();
        }

        public TestStore TestStore { get; private set; }
    }
}