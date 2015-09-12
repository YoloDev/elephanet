using Ploeh.AutoFixture;
using Xunit;
using Shouldly;
using System.Linq;
using System;

namespace Elephanet.Tests
{
    public class LinqOrderByTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private TestStore _store;

        public LinqOrderByTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore;

            var carA = new Fixture().Build<CarLinqOrder>()
              .With(x => x.Make, "Mazda")
              .With(y => y.Model, "A")
              .Create();

            var carB = new Fixture().Build<CarLinqOrder>()
                .With(x => x.Make, "Mazda")
                .With(y => y.Model, "B")
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
                var cars = session.Query<CarLinqOrder>().Where(c => c.Make == "Mazda").OrderBy(o => o.Model).ToList();
                cars[0].Model.ShouldBe("A");
                cars[1].Model.ShouldBe("B");
            }
        }

        [Fact]
        public void IQueryable_Should_ImplementOrderByDescending()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.Query<CarLinqOrder>().Where(c => c.Make == "Mazda").OrderByDescending(o => o.Model).ToList();
                cars[0].Model.ShouldBe("B");
                cars[1].Model.ShouldBe("A");
            }
        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<CarLinqOrder>();
            }
        }
    }

    public class CarLinqOrder
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }
        public string NumberPlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
