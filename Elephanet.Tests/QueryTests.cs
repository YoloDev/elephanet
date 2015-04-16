using System;
using Xunit;
using Shouldly;

namespace Elephanet.Tests
{
    public class QueryTests
    {
        [Fact]
        public void SingleQuotes_ShouldBe_EscapedWhenSaving()
        {
            var car = new Car();
            car.Id = Guid.NewGuid();
            car.Make = "Kia";
            car.Model = "Cee'd";

            var store = new TestStore();
            //save the car
            using (var session = store.OpenSession())
            {
                session.Store(car);
                session.SaveChanges();
            }

            //retrieve and check the make and model are correct
            using (var session = store.OpenSession())
            {
                var savedCar = session.GetById<Car>(car.Id);
                savedCar.Model.ShouldBe("Cee'd");
                savedCar.Make.ShouldBe("Kia");

            }
        }
    }
}
