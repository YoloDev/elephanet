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
    public class LinqTests : IDisposable
    {
        private IDocumentStore _store;

        public LinqTests()
        {
            _store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;");
            CreateDummyCars();
        }

        public void CreateDummyCars()
        {

            var dummyCars = new Fixture().Build<Car>()
               .With(x => x.Make, "Subaru")
               .CreateMany();

            var lowerCars = new Fixture().Build<Car>()
                .With(x => x.Make, "SAAB")
                .CreateMany();

            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Store<Car>(car);
                }

                foreach (var car in lowerCars)
                {
                    session.Store<Car>(car);
                }
                session.SaveChanges();
            }

        }

        [Fact]
        public void WherePredicate_Should_Build()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<Car>().Where(x => x.Make == "Subaru").ToList();
                results.ShouldNotBeEmpty();
            }
        }

        [Fact]
        public void WhereExpression_Should_ReturnWhereQuery()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<Car>().Where(c => c.Make == "Subaru");
                var cars = results.ToList();
                cars.Count.ShouldBe(3);
                cars.ShouldBeOfType<List<Car>>();
                cars.ForEach(c => c.Make.ShouldBe("Subaru"));
              
            }
        }

        [Fact]
        public void WhereExpression_Should_HandleExpressionSubtrees()
        {
            string make = "Subaru";
            using (var session = _store.OpenSession())
            {
                var results = session.Query<Car>().Where(c => c.Make == make);
                var cars = results.ToList();
                cars.Count.ShouldBe(3);
                cars.ShouldBeOfType<List<Car>>();
                cars.ForEach(c => c.Make.ShouldBe("Subaru"));
            }

        }

        [Fact]
        public void WhereExpression_Should_HandleExtensionMethodsInSubtrees()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<Car>().Where(c => c.Make == "saab".ToUpper());
                var cars = results.ToList();
                cars.Count.ShouldBe(3);
                cars.ShouldBeOfType<List<Car>>();
                cars.ForEach(c => c.Make.ShouldBe("SAAB"));
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
