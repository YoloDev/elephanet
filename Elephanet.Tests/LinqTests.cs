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
    public class LinqTests
    {
        private IDocumentStore _store;

        public LinqTests()
        {
            _store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;");
        }

        [Fact]
        public void WherePredicate_Should_Build()
        {

            var dummyCars = new Fixture().Build<Car>()
               .With(x => x.Make, "Subaru")
               .CreateMany();

            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Store<Car>(car);
                }
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var results = session.Query<Car>().Where(x => x.Make == "Subaru");
                results.ShouldNotBeEmpty();
            }
        }

        [Fact]
        public void ExpressionTest()
        {
            using (var session = _store.OpenSession())
            {
                var query = session.Query<Car>();
                query.CommandText().ShouldBe("select body from public.elephanet_tests_car;");
            }
        }

        [Fact]
        public void WhereExpression_Should_ReturnWhereQuery()
        {
            using (var session = _store.OpenSession())
            {
                var query = session.Query<Car>().Where(c => c.Make == "Subaru");
                //query.CommandText().ShouldBe("select body from public.elephanet_tests_car where body @> '{\"Make\":\"Subaru\"}';");
              
            }
        }
    }
}
