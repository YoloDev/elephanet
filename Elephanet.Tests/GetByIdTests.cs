using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture;
using Shouldly;
using Xunit;
using System;

namespace Elephanet.Tests
{
    public class GetByIdTests : IDisposable
    {
        private readonly StoreInfo _testStore;
        private readonly DocumentStore _store;
        private readonly List<Car> dummyCars;

        public  GetByIdTests()
        {
            _testStore = new StoreInfo();
            _store = new TestStore(); 
            dummyCars =  new Fixture().CreateMany<Car>().ToList();
            using (var session = _store.OpenSession())
            {
                foreach (var car in dummyCars)
                {
                    session.Store(car);
                }

                session.SaveChanges();
            }
        }

        [Fact]
        public void GetById_Should_NotByNull()
        {
            using (var session = _store.OpenSession())
            {
                var first = session.GetById<Car>(dummyCars[0].Id);
                first.ShouldNotBe(null);
            }
        }

        [Fact]
        public void GetByIds_Should_NotByNull()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.GetByIds<Car>(dummyCars.Select(c => c.Id).ToList()).ToList();
                cars[0].Id.ShouldBe(dummyCars[0].Id);
                cars[2].Id.ShouldBe(dummyCars[2].Id);
            }

        }

        [Fact]
        public void GetById_Should_Throw_EntityNotFoundException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.Throws<EntityNotFoundException>(() =>
                {
                    session.GetById<Car>(Guid.NewGuid());
                });
            }
        }

        [Fact]
        public void GetById_Should_Return_Null_When_ReturnNull_Convention_Set()
        {
            var store = TestStore.CreateStoreWithEntityNotFoundBehaviorReturnNull();
            using (var session = store.OpenSession())
            {
                session.GetById<Car>(Guid.NewGuid())
                    .ShouldBe(null);
            }
        }

        [Fact]
        public void GetAll_ShouldGetAll()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.GetAll<Car>().ToList();
                cars.Count.ShouldBe(3);
            }
        }

        public void Dispose()
        {
            _store.Destroy();
        }
    }
}
