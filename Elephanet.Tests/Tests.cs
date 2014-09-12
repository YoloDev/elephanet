using Xunit;
using Shouldly;
using System;
using Ploeh.AutoFixture;

namespace Elephanet.Tests
{
    public class Tests
    {
    
        [Fact]
        public void WillPass()
        {
            true.ShouldBe(true);
        }

        [Fact]
        public void NewStore_ConnectionStringShouldBe_SameAsCtor()
        {
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);
            store.ConnectionString.ShouldBe("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;");
        }

        [Fact]
        public void Store_Should_OpenSession()
        {
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);
            using (var session = store.OpenSession())
            {
                //not really doing anything
            }

        }

        [Fact]
        public void SessionStore_Should_NotThrow()
        {
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);
            using (var session = store.OpenSession())
            {
                var car = new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Subaru",
                    Model = "Impreza",
                    ImageUrl = "http://www.carsfotodb.com/uploads/subaru/subaru-impreza/subaru-impreza-08.jpg",
                    NumberPlate = "NRM1003" 
                };

                Should.NotThrow(() => session.Store<Car>(car));
            }

        }

        [Fact]
        public void SessionSaveChanges_Should_SaveChanges()
        {
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);
            using (var session = store.OpenSession())
            {
                var car = new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Subaru",
                    Model = "Impreza",
                    ImageUrl = "http://www.carsfotodb.com/uploads/subaru/subaru-impreza/subaru-impreza-08.jpg",
                    NumberPlate = "NRM1003"
                };

                session.Store<Car>(car);
                session.SaveChanges();
            }

        }

        [Fact]
        public void StoringAnEnitityAndFetchingWithinSession_Should_ReturnEntity()
        {
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);

            var dummyCar = new Fixture().Create<Car>();

            using (var session = store.OpenSession())
            {
                session.Store<Car>(dummyCar);
                var result = session.Load<Car>(dummyCar.Id);
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
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);
            
            store.StoreInfo.ShouldNotBe(null);
            store.Conventions.ShouldNotBe(null);
        }

        [Fact]
        public void SaveSessionThenLoading_Should_ReturnEntity()
        {
            IStoreInfo testStore = new StoreInfo(storeName:"test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);

            var dummyCar = new Fixture().Create<Car>();

            using (var session = store.OpenSession())
            {
                session.Store<Car>(dummyCar);
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var car = session.Load<Car>(dummyCar.Id);
                car.Id.ShouldBe(dummyCar.Id);
                car.Make.ShouldBe(dummyCar.Make);
                car.Model.ShouldBe(dummyCar.Model);
                car.ImageUrl.ShouldBe(dummyCar.ImageUrl);
                car.NumberPlate.ShouldBe(dummyCar.NumberPlate);
            }
        }

        [Fact]
        public void DocumentStoreEmpty_Should_RemoveRecords()
        {
            IStoreInfo testStore = new StoreInfo(storeName: "test_store");
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;", testStore);
            store.Empty();
        }
    }

    public class Car
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }
        public string NumberPlate { get; set; }
    }
}

