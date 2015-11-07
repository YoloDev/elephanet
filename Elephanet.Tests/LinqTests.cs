using System;
using System.Collections.Generic;
using System.Linq;
using Elephanet.Tests.Entities;
using Xunit;
using Shouldly;
using Ploeh.AutoFixture;

namespace Elephanet.Tests
{
    public class LinqTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private readonly IDocumentStore _store;

        public LinqTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore; 
            CreateDummyCar();
        }

        public void CreateDummyCar()
        {

            var dummyEntity = new Fixture().Build<EntityForWhereLinqTests>()
               .With(x => x.PropertyOne, "Subaru")
               .CreateMany();

            var lowercasePropertyOneEntity = new Fixture().Build<EntityForWhereLinqTests>()
                .With(x => x.PropertyOne, "SAAB")
                .CreateMany();

            using (var session = _store.OpenSession())
            {
                foreach (var entity in dummyEntity)
                {
                    session.Store(entity);
                }

                foreach (var entity in lowercasePropertyOneEntity)
                {
                    session.Store(entity);
                }
                session.SaveChanges();
            }

        }

        [Fact]
        public void WherePredicate_Should_Build()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<EntityForWhereLinqTests>().Where(x => x.PropertyOne == "Subaru").ToList();
                results.ShouldNotBeEmpty();
            }
        }

        [Fact]
        public void WhereExpression_Should_ReturnWhereQuery()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<EntityForWhereLinqTests>().Where(c => c.PropertyOne == "Subaru");
                var car = results.ToList();
                car.Count.ShouldBe(3);
                car.ShouldBeOfType<List<EntityForWhereLinqTests>>();
                car.ForEach(c => c.PropertyOne.ShouldBe("Subaru"));
              
            }
        }

        [Fact]
        public void WhereExpression_Should_HandleExpressionSubtrees()
        {
            const string propertyOne = "Subaru";
            using (var session = _store.OpenSession())
            {
                var results = session.Query<EntityForWhereLinqTests>().Where(c => c.PropertyOne == propertyOne);
                var car = results.ToList();
                car.Count.ShouldBe(3);
                car.ShouldBeOfType<List<EntityForWhereLinqTests>>();
                car.ForEach(c => c.PropertyOne.ShouldBe("Subaru"));
            }
        }

        [Fact]
        public void WhereExpression_Should_HandleExtensionMethodsInSubtrees()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<EntityForWhereLinqTests>().Where(c => c.PropertyOne == "saab".ToUpper());
                var car = results.ToList();
                car.Count.ShouldBe(3);
                car.ShouldBeOfType<List<EntityForWhereLinqTests>>();
                car.ForEach(c => c.PropertyOne.ShouldBe("SAAB"));
            }

        }

        [Fact]
        public void IQueryable_Should_ImplementTakeMethod()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<EntityForWhereLinqTests>().Where(c => c.PropertyOne == "SAAB").Take(2);
                var car = results.ToList();
                car.Count.ShouldBe(2);
                car.ShouldBeOfType<List<EntityForWhereLinqTests>>();
            }
        }

        [Fact]
        public void IQueryable_Should_ImplementSkipMethod()
        {
            using (var session = _store.OpenSession())
            {
                var results = session.Query<EntityForWhereLinqTests>().Where(c => c.PropertyOne == "SAAB").Skip(2);
                var car = results.ToList();
                car.Count.ShouldBe(1);
                car.ShouldBeOfType<List<EntityForWhereLinqTests>>();
            }

        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<EntityForWhereLinqTests>();
            }
        }
    }
}
