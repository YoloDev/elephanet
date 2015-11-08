﻿using System.Linq;
using Elephanet.Tests.Entities;
using Elephanet.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class GetAllTests
    {
        private readonly TestStore _store;

        public GetAllTests()
        {
            _store = new TestStore();
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
    }
}
