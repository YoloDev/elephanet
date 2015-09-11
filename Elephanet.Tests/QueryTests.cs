using System;
using Xunit;
using Shouldly;

namespace Elephanet.Tests
{
    public class QueryTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private TestStore _store;

        public QueryTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore;
        }

        [Fact]
        public void SingleQuotes_ShouldBe_EscapedWhenSaving()
        {
            var car = new Car();
            car.Id = Guid.NewGuid();
            car.Make = "Kia";
            car.Model = "Cee'd";

          
            //save the car
            using (var session = _store.OpenSession())
            {
                session.Store(car);
                session.SaveChanges();
            }

            //retrieve and check the make and model are correct
            using (var session = _store.OpenSession())
            {
                var savedCar = session.GetById<Car>(car.Id);
                savedCar.Model.ShouldBe("Cee'd");
                savedCar.Make.ShouldBe("Kia");

            }
        }

        public void Dispose()
        {
            
        }
    }
}
