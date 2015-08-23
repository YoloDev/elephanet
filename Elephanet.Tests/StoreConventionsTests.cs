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
    }
}
