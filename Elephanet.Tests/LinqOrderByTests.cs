using Ploeh.AutoFixture;
using Xunit;
using Shouldly;
using System.Linq;
using System;

namespace Elephanet.Tests
{
    public class LinqOrderByTests : IDisposable
    {
        private StoreInfo _testStore;
        private TestStore _store;

        public LinqOrderByTests()
        {
            _testStore = new StoreInfo();
            _store = new TestStore();


            var carA = new Fixture().Build<Car>()
              .With(x => x.Make, "Mazda")
              .With(y => y.Model, "A")
              .Create();

            var carB = new Fixture().Build<Car>()
                .With(x => x.Make, "Mazda")
                .With(y => y.Model, "B")
                .Create();

            using (var session = _store.OpenSession())
            {
                session.Store<Car>(carA);
                session.Store<Car>(carB);
                session.SaveChanges();
            }
        }

        [Fact]
        public void IQueryable_Should_ImplementOrderBy()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.Query<Car>().Where(c => c.Make == "Mazda").OrderBy(o => o.Model).ToList();
                cars[0].Model.ShouldBe("A");
                cars[1].Model.ShouldBe("B");
            }
        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<Car>();
            }
        }
    }
}
