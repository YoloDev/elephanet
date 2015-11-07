using System.Collections.Generic;
using System.Linq;
using Elephanet.Tests.Entities;
using Ploeh.AutoFixture;
using Shouldly;
using Xunit;
using System;
using Elephanet.Tests.Infrastructure;

namespace Elephanet.Tests
{
    public class GetByIdTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private readonly TestStore _store;
        private readonly List<EntityForGetByIdTests> _dummyCars;

        public GetByIdTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore; 
            _dummyCars = new Fixture().CreateMany<EntityForGetByIdTests>().ToList();
            using (var session = _store.OpenSession())
            {
                foreach (var car in _dummyCars)
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
                var first = session.GetById<EntityForGetByIdTests>(_dummyCars[0].Id);
                first.ShouldNotBe(null);
            }
        }

        [Fact]
        public void GetByIds_Should_NotByNull()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.GetByIds<EntityForGetByIdTests>(_dummyCars.Select(c => c.Id).ToList()).ToList();
                cars[0].Id.ShouldBe(_dummyCars[0].Id);
                cars[2].Id.ShouldBe(_dummyCars[2].Id);
            }
        }

        [Fact]
        public void GetById_Should_Throw_EntityNotFoundException()
        {
            using (var session = _store.OpenSession())
            {   
                Should.Throw<EntityNotFoundException>(() =>
                {
                    session.GetById<EntityForGetByIdTests>(Guid.NewGuid());
                });
            }
        }

        [Fact]
        public void GetById_Should_Return_Null_When_ReturnNull_Convention_Set()
        {
            var store = TestStore.CreateStoreWithEntityNotFoundBehaviorReturnNull();
            using (var session = store.OpenSession())
            {
                session.GetById<EntityForGetByIdTests>(Guid.NewGuid())
                    .ShouldBe(null);
            }
        }

        [Fact]
        public void GetAll_ShouldGetAll()
        {
            using (var session = _store.OpenSession())
            {
                var cars = session.GetAll<EntityForGetByIdTests>().ToList();
                cars.Count.ShouldBe(3);
            }
        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<EntityForGetByIdTests>();
            }
        }

       
    }
}
