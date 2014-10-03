using System;
using System.Collections.Generic;
using System.Linq;
using Elephanet;
using Xunit;
using Shouldly;

namespace Elephanet.Tests
{
    public class StoreInfoTests
    {
        [Fact]
        public void StoreInfo_GivenANewTable_Should_AddItToItsTableNamesList()
        {
            StoreInfo storeInfo = new StoreInfo();
            storeInfo.Add("someting or rather");

            storeInfo.Tables.ShouldContain("someting or rather");
        }
    }
}
