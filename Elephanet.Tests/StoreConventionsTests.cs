using Elephanet.Conventions;
using Elephanet.Serialization;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class StoreConventionsTests
    {
        [Fact]
        public void Ctor_Sets_JsonConverter_And_TableInfo()
        {
            var jsonConverter = new JilJsonConverter();
            var info = new TableInfo();

            var convention = new StoreConventions(jsonConverter, info);

            convention.JsonConverter.ShouldBeSameAs(jsonConverter);
            convention.TableInfo.ShouldBeSameAs(info);
        }

        [Fact]
        public void Ctor_Providing_IJsonConverter_Sets_JsonConverter_And_TableInfo_To_Default()
        {
            var jsonConverter = new JilJsonConverter();

            var conventions = new StoreConventions(jsonConverter);

            conventions.JsonConverter.ShouldBeSameAs(jsonConverter);
            conventions.TableInfo.ShouldBeOfType<TableInfo>();
        }

        [Fact]
        public void EntityNotFoundBehavior_Defaults_To_Throw()
        {
            new StoreConventions()
                .EntityNotFoundBehavior.ShouldBe(EntityNotFoundBehavior.Throw);
        }

        [Fact]
        public void SetEntityNotFoundBehavior_Sets__EntityNotFoundBehavior()
        {
            var conventions = new StoreConventions();

            conventions.SetEntityNotFoundBehavior(EntityNotFoundBehavior.ReturnNull);

            conventions.EntityNotFoundBehavior.ShouldBe(EntityNotFoundBehavior.ReturnNull);
        }
    }
}
