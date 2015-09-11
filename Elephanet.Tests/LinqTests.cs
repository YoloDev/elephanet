using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Shouldly;
using Ploeh.AutoFixture;
using NSubstitute;


namespace Elephanet.Tests
{
    public class LinqTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private IDocumentStore _store;

        public LinqTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore; 
            CreateDummyCar();
        }

        public void CreateDummyCar()
        {

            var dummycar = new Fixture().Build<CarLinq>()
               .With(x => x.Make, "Subaru")
               .CreateMany();

            var lowercar = new Fixture().Build<CarLinq>()
                .With(x => x.Make, "SAAB")
                .CreateMany();

            using (var session = _store.OpenSession())
            {
                foreach (var car in dummycar)
                {
                    session.Store<CarLinq>(car);
                }

                foreach (var car in lowercar)
                {
                    session.Store<CarLinq>(car);
                }
                session.SaveChanges();
            }

        }

        [Fact]
        public void WherePredicate_Should_Build()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<CarLinq>().Where(x => x.Make == "Subaru").ToList();
                results.ShouldNotBeEmpty();
            }
        }

        [Fact]
        public void WhereExpression_Should_ReturnWhereQuery()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<CarLinq>().Where(c => c.Make == "Subaru");
                var car = results.ToList();
                car.Count.ShouldBe(3);
                car.ShouldBeOfType<List<CarLinq>>();
                car.ForEach(c => c.Make.ShouldBe("Subaru"));
              
            }
        }

        [Fact]
        public void WhereExpression_Should_HandleExpressionSubtrees()
        {
            string make = "Subaru";
            using (var session = _store.OpenSession())
            {
                var results = session.Query<CarLinq>().Where(c => c.Make == make);
                var car = results.ToList();
                car.Count.ShouldBe(3);
                car.ShouldBeOfType<List<CarLinq>>();
                car.ForEach(c => c.Make.ShouldBe("Subaru"));
            }

        }

        [Fact]
        public void WhereExpression_Should_HandleExtensionMethodsInSubtrees()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<CarLinq>().Where(c => c.Make == "saab".ToUpper());
                var car = results.ToList();
                car.Count.ShouldBe(3);
                car.ShouldBeOfType<List<CarLinq>>();
                car.ForEach(c => c.Make.ShouldBe("SAAB"));
            }

        }

        [Fact]
        public void IQueryable_Should_ImplementTakeMethod()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<CarLinq>().Where(c => c.Make == "SAAB").Take(2);
                var car = results.ToList();
                car.Count.ShouldBe(2);
                car.ShouldBeOfType<List<CarLinq>>();
            }
        }

        [Fact]
        public void IQueryable_Should_ImplementSkipMethod()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<CarLinq>().Where(c => c.Make == "SAAB").Skip(2);
                var car = results.ToList();
                car.Count.ShouldBe(1);
                car.ShouldBeOfType<List<CarLinq>>();
            }

        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<CarLinq>();
            }
        }
    }

    public class CarLinq
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }
        public string NumberPlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
