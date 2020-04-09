using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Asset
    {
        public string Name { get; set; }
        public virtual SolarSystem SolarSystem { get; set; }
        public virtual Faction Faction { get; set; }
    }
}
