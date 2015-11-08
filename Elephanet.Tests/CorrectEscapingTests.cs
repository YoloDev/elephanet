using System;
using Elephanet.Tests.Entities;
using Elephanet.Tests.Infrastructure;
using Xunit;
using Shouldly;

namespace Elephanet.Tests
{
    public class CorrectEscapingTests
    {
        private readonly TestStore _store;
        private EntityForCorrectEscapingTests _entity;

        public CorrectEscapingTests()
        {
            _store = new TestStore();
        }

        [Fact]
        public void SingleQuotes_ShouldBe_EscapedWhenSaving()
        {
            _entity = new EntityForCorrectEscapingTests() { Id = Guid.NewGuid(), PropertyOne = "Kia", PropertyTwo = "Cee'd" };
                
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
    }
}
