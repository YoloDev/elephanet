using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture;
using Shouldly;
using Xunit;
using System;

namespace Elephanet.Tests
{
    public class GetByIdTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private readonly TestStore _store;
        private readonly List<CarGetById> dummyCars;

        public GetByIdTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore; 
            dummyCars = new Fixture().CreateMany<CarGetById>().ToList();
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
                var first = session.GetById<CarGetById>(dummyCars[0].Id);
                first.ShouldNotBe(null);
            }
        }

        [Fact]
        public void GetByIds_Should_NotByNull()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.GetByIds<CarGetById>(dummyCars.Select(c => c.Id).ToList()).ToList();
                cars[0].Id.ShouldBe(dummyCars[0].Id);
                cars[2].Id.ShouldBe(dummyCars[2].Id);
            }
        }

        [Fact]
        public void GetById_Should_Throw_EntityNotFoundException()
        {
            using (var session = _store.OpenSession())
            {   
                Should.Throw<EntityNotFoundException>(() =>
                {
                    session.GetById<CarGetById>(Guid.NewGuid());
                });
            }
        }

        [Fact]
        public void GetById_Should_Return_Null_When_ReturnNull_Convention_Set()
        {
            var store = TestStore.CreateStoreWithEntityNotFoundBehaviorReturnNull();
            using (var session = store.OpenSession())
            {
                session.GetById<CarGetById>(Guid.NewGuid())
                    .ShouldBe(null);
            }
        }

        [Fact]
        public void GetAll_ShouldGetAll()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.GetAll<CarGetById>().ToList();
                cars.Count.ShouldBe(3);
            }
        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<CarGetById>();
            }
        }

       
    }

    public class CarGetById
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }
        public string NumberPlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
