using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class GetAllTests
    {

        [Fact]
        public void GetAllCreatesTheTableIfItDoesNotExist()
        {
            var store = new TestStore();
            using (var session = store.OpenSession())
            {
                var things = session.GetAll<SomeTestClassThatNoOneWillEverUse>().ToList();
                things.Count.ShouldBe(0);
            }
        }
    }


    public class SomeTestClassThatNoOneWillEverUse
    {
        public Guid Id { get; set; }
        public string Something { get; set; }
    }
}
