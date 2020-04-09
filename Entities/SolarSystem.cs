using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class SolarSystem
    {
        public string Name { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ConflictStatus { get; set; }
        public string ConflictType { get; set; }
        public virtual Faction ControllingFaction { get; set; }
        public List<Asset> Assets { get; set; } = new List<Asset>();
    }
}
