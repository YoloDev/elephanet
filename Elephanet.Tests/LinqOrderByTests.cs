using Elephanet.Tests.Entities;
using Ploeh.AutoFixture;
using Xunit;
using Shouldly;
using System.Linq;
using System;
using Elephanet.Tests.Infrastructure;

namespace Elephanet.Tests
{
    public class LinqOrderByTests
    {
        private readonly TestStore _store;

        public LinqOrderByTests()
        {
            _store = new TestStore();

            var carA = new Fixture().Build<EntityForLinqOrderByTests>()
              .With(x => x.PropertyOne, "Mazda")
              .With(y => y.PropertyTwo, "A")
              .Create();

            var carB = new Fixture().Build<EntityForLinqOrderByTests>()
                .With(x => x.PropertyOne, "Mazda")
                .With(y => y.PropertyTwo, "B")
                .Create();

            using (var session = _store.OpenSession())
            {
                session.Store(carA);
                session.Store(carB);
                session.SaveChanges();
            }
        }

        [Fact]
        public void IQueryable_Should_ImplementOrderBy()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.Query<EntityForLinqOrderByTests>().Where(c => c.PropertyOne == "Mazda").OrderBy(o => o.PropertyTwo).ToList();
                cars[0].PropertyTwo.ShouldBe("A");
                cars[1].PropertyTwo.ShouldBe("B");
            }
        }

        [Fact]
        public void IQueryable_Should_ImplementOrderByDescending()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.Query<EntityForLinqOrderByTests>().Where(c => c.PropertyOne == "Mazda").OrderByDescending(o => o.PropertyTwo).ToList();
                cars[0].PropertyTwo.ShouldBe("B");
                cars[1].PropertyTwo.ShouldBe("A");
            }
        }
    }
}
