using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class GetAllTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private TestStore _store;

        public GetAllTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore;
        }

        [Fact]
        public void GetAllCreatesTheTableIfItDoesNotExist()
        {
            using (var session = _store.OpenSession())
            {
                var things = session.GetAll<SomeTestClassThatNoOneWillEverUse>().ToList();
                things.Count.ShouldBe(0);
            }
        }

        public void Dispose()
        {
            
        }
    }


    public class SomeTestClassThatNoOneWillEverUse
    {
        public Guid Id { get; set; }
        public string Something { get; set; }
    }
}
