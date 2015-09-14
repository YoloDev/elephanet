using System;
using Elephanet.Tests.Entities;
using Xunit;
using Shouldly;

namespace Elephanet.Tests
{
    public class CorrectEscapingTests : IClassFixture<DocumentStoreBaseFixture>, IDisposable
    {
        private readonly TestStore _store;
        private EntityForCorrectEscapingTests _entity;

        public CorrectEscapingTests(DocumentStoreBaseFixture data)
        {
            _store = data.TestStore;
        }

        [Fact]
        public void SingleQuotes_ShouldBe_EscapedWhenSaving()
        {
            _entity = new EntityForCorrectEscapingTests() {Id = Guid.NewGuid(), PropertyOne = "Kia", PropertyTwo = "Cee'd"};


            //save the car
            using (var session = _store.OpenSession())
            {
                session.Store(_entity);
                session.SaveChanges();
            }

            //retrieve and check the make and model are correct
            using (var session = _store.OpenSession())
            {
                var savedCar = session.GetById<EntityForCorrectEscapingTests>(_entity.Id);
                savedCar.PropertyTwo.ShouldBe("Cee'd");
                savedCar.PropertyOne.ShouldBe("Kia");
            }
        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.Delete<EntityForCorrectEscapingTests>(_entity.Id);
                session.SaveChanges();
            } 
        }
    }
}
