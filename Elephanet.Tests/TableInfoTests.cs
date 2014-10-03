using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Shouldly;

namespace Elephanet.Tests
{
    public class ConventionTests
    {
        [Fact]
        public void TableInfo_GivenNoSchema_ShouldReturnSchemaOfPublic()
        {
            TableInfo tableInfo = new TableInfo();
            tableInfo.Schema.ShouldBe("public");
        }

        [Fact]
        public void TableInfo_GivenASchema_ShouldReturnThatSchema()
        {
            TableInfo tableInfo = new TableInfo("aschema");
            tableInfo.Schema.ShouldBe("aschema");
        }

        [Fact]
        public void TableInfo_GivenAType_ShouldReturn_TheCorrectTableNameWithoutSchema()
        {
            TableInfo tableInfo = new TableInfo("aschema");
            tableInfo.TableNameWithoutSchema(typeof(Car)).ShouldBe("elephanet_tests_car");
 
        }

        [Fact]
        public void TableInfo_GivenAType_ShouldReturn_TheCorrectTableNameWithSchema()
        {
            TableInfo tableInfo = new TableInfo("aschema");
            tableInfo.TableNameWithSchema(typeof(Car)).ShouldBe("aschema.elephanet_tests_car");

        }
    }
}
