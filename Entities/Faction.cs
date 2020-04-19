using System;
using System.Collections.Generic;

namespace Entities
{
    public class Faction
    {
        public string Name { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<SolarSystem> SolarSystems { get; set; } = new List<SolarSystem>();
    }
}
