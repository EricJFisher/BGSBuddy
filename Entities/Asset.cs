using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public class Asset
    {
        public string Name { get; set; }
        public string SolarSystem { get; set; }
        public string Faction { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
