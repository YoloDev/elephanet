using System;
using System.Linq;
using Ploeh.AutoFixture;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class Tests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private TestStore _store;

        public Tests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore;
        }
            
        [Fact]
        public void WillPass()
        {
            true.ShouldBe(true);
        }

        [Fact]
        public void NewStore_ConnectionStringShouldBe_SameAsCtor()
        {
            _store.ConnectionString.ShouldBe("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;");
        }

        [Fact]
        public void Store_Should_OpenSession()
        {
            using (var session = _store.OpenSession())
            {
                //not really doing anything
            }
        }

        [Fact]
        public void SessionStore_Should_NotThrow()
        {
            using (var session = _store.OpenSession())
            {
                var car = new CarTests
                {
                    Id = Guid.NewGuid(),
                    Make = "Subaru",
                    Model = "Impreza",
                    ImageUrl = "http://www.carsfotodb.com/uploads/subaru/subaru-impreza/subaru-impreza-08.jpg",
                    NumberPlate = "NRM1003" 
                };

                Should.NotThrow(() => session.Store<CarTests>(car));
            }

        }

        [Fact]
        public void SessionSaveChanges_Should_SaveChanges()
        {
            using (var session = _store.OpenSession())
            {
                var car = new CarTests
                {
                    Id = Guid.NewGuid(),
                    Make = "Subaru",
                    Model = "Impreza",
                    ImageUrl = "http://www.carsfotodb.com/uploads/subaru/subaru-impreza/subaru-impreza-08.jpg",
                    NumberPlate = "NRM1003"
                };

                session.Store<CarTests>(car);
                session.SaveChanges();
            }

        }

        [Fact]
        public void StoringAnEnitityAndFetchingWithinSession_Should_ReturnEntity()
        {

            var dummyCar = new Fixture().Create<CarTests>();

            using (var session = _store.OpenSession())
            {
                session.Store<CarTests>(dummyCar);
                var result = session.GetById<CarTests>(dummyCar.Id);
                result.Id.ShouldBe(dummyCar.Id);
                result.Make.ShouldBe(dummyCar.Make);
                result.Model.ShouldBe(dummyCar.Model);
                result.ImageUrl.ShouldBe(dummyCar.ImageUrl);
                result.NumberPlate.ShouldBe(dummyCar.NumberPlate);

            }
        }

        [Fact]
        public void NewStoreWithNoCtors_Should_SetConventions()
        {
            _store.StoreInfo.ShouldNotBe(null);
            _store.Conventions.ShouldNotBe(null);
        }

        [Fact]
        public void SaveSessionThenLoading_Should_ReturnEntity()
        {

            var dummyCar = new Fixture().Create<CarTests>();

            using (var session = _store.OpenSession())
            {
                session.Store<CarTests>(dummyCar);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var car = session.GetById<CarTests>(dummyCar.Id);
                car.Id.ShouldBe(dummyCar.Id);
                car.Make.ShouldBe(dummyCar.Make);
                car.Model.ShouldBe(dummyCar.Model);
                car.ImageUrl.ShouldBe(dummyCar.ImageUrl);
                car.NumberPlate.ShouldBe(dummyCar.NumberPlate);
            }
        }

        [Fact]
        public void SaveChangesWithMultipleDifferentClasses_Should_Save()
        {

            var dummyCar = new Fixture().Create<CarTests>();
            var dummyBike = new Fixture().Create<Bike>();

            using (var session = _store.OpenSession())
            {
                session.Store<CarTests>(dummyCar);
                session.Store<Bike>(dummyBike);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                session.GetById<Bike>(dummyBike.Id).ShouldNotBe(null);
                session.GetById<CarTests>(dummyCar.Id).ShouldNotBe(null);
            }
        }


        [Fact]
        public void SessionCanDeleteById()
        {

            var dummyCars = new Fixture().Build<CarTests>()
                .With(x => x.Make, "Subaru")
                .CreateMany().ToList();
            //setup
            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Store<CarTests>(car);
                }
                session.SaveChanges();
            }
            //delete
            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Delete<CarTests>(car.Id);
                    session.SaveChanges();
                }
            }
            //check
            using (var session = _store.OpenSession())
            {
                var records = session.Query<CarTests>().Where(c => c.Make == "Subaru").ToList();
                records.Count.ShouldBe(0);
            }
        }

        [Fact]
        public void SaveChangesWithUpdates_Should_Persist()
        {

            var dummyCar = new Fixture().Create<CarTests>();
            var dummyBike = new Fixture().Create<Bike>();

            using (var session = _store.OpenSession())
            {
                session.Store<CarTests>(dummyCar);
                session.Store<Bike>(dummyBike);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var retrievedBike = session.GetById<Bike>(dummyBike.Id);
                var retrievedCar = session.GetById<CarTests>(dummyCar.Id);

                retrievedBike.WheelSize = 900;
                retrievedCar.Make = "Lada";
                session.Store(retrievedCar);
                session.Store(retrievedBike);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var alteredBike = session.GetById<Bike>(dummyBike.Id);
                var alteredCar = session.GetById<CarTests>(dummyCar.Id);

                alteredBike.WheelSize.ShouldBe(900);
                alteredCar.Make.ShouldBe("Lada");

            }
        }

          [Fact]
        public void SaveChangesWithUpdates_Should_BeQueryable()
        {
           var dummyCars = new Fixture().Build<CarTests>()
          .With(x => x.Make, "Subaru")
          .CreateMany(100).ToList();
            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Store(car);
                    session.SaveChanges();
                }

            }

              using (var session = _store.OpenSession())
              {
                  //retrieve a couple of cars
                  var car1ToAlter = session.GetById<CarTests>(dummyCars[15].Id);
                  var car2ToAlter = session.GetById<CarTests>(dummyCars[85].Id);
                  car1ToAlter.Make = "Ford";
                  car2ToAlter.Make = "Ford";
                  session.Store(car1ToAlter);
                  session.Store(car2ToAlter);
                  session.SaveChanges();
              }

              using (var session = _store.OpenSession())
              {
                  var cars = session.Query<CarTests>().Where(c => c.Make == "Ford").ToList();
                  cars.Count.ShouldBe(2);
              }
        }

        public void Dispose()
        {
            //_store.Destroy();
            using(var session = _store.OpenSession()) session.DeleteAll<CarTests>();
        }
    }

    public class CarTests
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }
        public string NumberPlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

