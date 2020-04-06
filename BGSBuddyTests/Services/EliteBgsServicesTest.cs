using Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BGSBuddyTests.Services
{
    public class EliteBgsServicesTest
    {
        private EliteBgsService service = new EliteBgsService();

        [Fact]
        public void GetFaction()
        {
            var faction = service.GetFaction("Alliance Rapid-reaction Corps");
            Assert.True(faction != null, "GetFaction didn't return anything.");
        }
    }
}
