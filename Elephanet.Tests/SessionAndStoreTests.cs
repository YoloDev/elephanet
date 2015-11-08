using System;
using System.Linq;
using Elephanet.Tests.Entities;
using Elephanet.Tests.Infrastructure;
using Ploeh.AutoFixture;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class SessionAndStoreTests
    {
        private readonly TestStore _store;

        public SessionAndStoreTests()
        {
            _store = new TestStore();
        }

        [Fact]
        public void WillPass()
        {
            true.ShouldBe(true);
        }

        [Fact]
        public void NewStore_ConnectionStringShouldBe_SameAsCtor()
        {
            _store.ConnectionString.ShouldBe("Server=127.0.0.1;Port=5432;User id=elephanet_tests_user;password=my super secret password;database=elephanet_tests_store;");
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
                var car = new EntityForSessionAndStoreTests
                {
                    Id = Guid.NewGuid(),
                    PropertyOne = "Subaru",
                    PropertyTwo = "Impreza",
                    PropertyThree = "http://www.carsfotodb.com/uploads/subaru/subaru-impreza/subaru-impreza-08.jpg",
                    PropertyFour = "NRM1003"
                };

                Should.NotThrow(() => session.Store<EntityForSessionAndStoreTests>(car));
            }

        }

        [Fact]
        public void SessionSaveChanges_Should_SaveChanges()
        {
            using (var session = _store.OpenSession())
            {
                var car = new EntityForSessionAndStoreTests
                {
                    Id = Guid.NewGuid(),
                    PropertyOne = "Subaru",
                    PropertyTwo = "Impreza",
                    PropertyThree = "http://www.carsfotodb.com/uploads/subaru/subaru-impreza/subaru-impreza-08.jpg",
                    PropertyFour = "NRM1003"
                };

                session.Store(car);
                session.SaveChanges();
            }

        }

        [Fact]
        public void StoringAnEnitityAndFetchingWithinSession_Should_ReturnEntity()
        {

            var dummyCar = new Fixture().Create<EntityForSessionAndStoreTests>();

            using (var session = _store.OpenSession())
            {
                session.Store(dummyCar);
                var result = session.GetById<EntityForSessionAndStoreTests>(dummyCar.Id);
                result.Id.ShouldBe(dummyCar.Id);
                result.PropertyOne.ShouldBe(dummyCar.PropertyOne);
                result.PropertyTwo.ShouldBe(dummyCar.PropertyTwo);
                result.PropertyThree.ShouldBe(dummyCar.PropertyThree);
                result.PropertyFour.ShouldBe(dummyCar.PropertyFour);

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

            var dummyCar = new Fixture().Create<EntityForSessionAndStoreTests>();

            using (var session = _store.OpenSession())
            {
                session.Store(dummyCar);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var car = session.GetById<EntityForSessionAndStoreTests>(dummyCar.Id);
                car.Id.ShouldBe(dummyCar.Id);
                car.PropertyOne.ShouldBe(dummyCar.PropertyOne);
                car.PropertyTwo.ShouldBe(dummyCar.PropertyTwo);
                car.PropertyThree.ShouldBe(dummyCar.PropertyThree);
                car.PropertyFour.ShouldBe(dummyCar.PropertyFour);
            }
        }

        [Fact]
        public void SaveChangesWithMultipleDifferentClasses_Should_Save()
        {

            var firstDummyEntity = new Fixture().Create<EntityForSessionAndStoreTests>();
            var secondDummyEntity = new Fixture().Create<SecondEntityForSessionAndStoreTest>();

            using (var session = _store.OpenSession())
            {
                session.Store(firstDummyEntity);
                session.Store(secondDummyEntity);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                session.GetById<SecondEntityForSessionAndStoreTest>(secondDummyEntity.Id).ShouldNotBe(null);
                session.GetById<EntityForSessionAndStoreTests>(firstDummyEntity.Id).ShouldNotBe(null);
            }
        }


        [Fact]
        public void SessionCanDeleteById()
        {

            var dummyCars = new Fixture().Build<EntityForSessionAndStoreTests>()
                .With(x => x.PropertyOne, "Subaru")
                .CreateMany().ToList();
            //setup
            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Store(car);
                }
                session.SaveChanges();
            }
            //delete
            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Delete<EntityForSessionAndStoreTests>(car.Id);
                    session.SaveChanges();
                }
            }
            //check
            using (var session = _store.OpenSession())
            {
                var records = session.Query<EntityForSessionAndStoreTests>().Where(c => c.PropertyOne == "Subaru").ToList();
                records.Count.ShouldBe(0);
            }
        }

        [Fact]
        public void SaveChangesWithUpdates_Should_Persist()
        {

            var firstDummyEntity = new Fixture().Create<EntityForSessionAndStoreTests>();
            var secondDummyEntity = new Fixture().Create<SecondEntityForSessionAndStoreTest>();

            using (var session = _store.OpenSession())
            {
                session.Store(firstDummyEntity);
                session.Store(secondDummyEntity);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var retrievedBike = session.GetById<SecondEntityForSessionAndStoreTest>(secondDummyEntity.Id);
                var retrievedCar = session.GetById<EntityForSessionAndStoreTests>(firstDummyEntity.Id);

                retrievedBike.WheelSize = 900;
                retrievedCar.PropertyOne = "Lada";
                session.Store(retrievedCar);
                session.Store(retrievedBike);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var alteredBike = session.GetById<SecondEntityForSessionAndStoreTest>(secondDummyEntity.Id);
                var alteredCar = session.GetById<EntityForSessionAndStoreTests>(firstDummyEntity.Id);

                alteredBike.WheelSize.ShouldBe(900);
                alteredCar.PropertyOne.ShouldBe("Lada");

            }
        }

        [Fact]
        public void SaveChangesWithUpdates_Should_BeQueryable()
        {
            var dummyCars = new Fixture().Build<EntityForSessionAndStoreTests>()
           .With(x => x.PropertyOne, "Subaru")
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
                var car1ToAlter = session.GetById<EntityForSessionAndStoreTests>(dummyCars[15].Id);
                var car2ToAlter = session.GetById<EntityForSessionAndStoreTests>(dummyCars[85].Id);
                car1ToAlter.PropertyOne = "Ford";
                car2ToAlter.PropertyOne = "Ford";
                session.Store(car1ToAlter);
                session.Store(car2ToAlter);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var cars = session.Query<EntityForSessionAndStoreTests>().Where(c => c.PropertyOne == "Ford").ToList();
                cars.Count.ShouldBe(2);
            }
        }
    }
}

