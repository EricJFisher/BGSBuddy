using System;
using System.Collections.Generic;

namespace Entities
{
    public class SolarSystem
    {
        public string Name { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ConflictStatus { get; set; }
        public string ConflictType { get; set; }
        public string ControllingFaction { get; set; }
        public List<Asset> Assets { get; set; } = new List<Asset>();
        public List<SubFaction> SubFactions { get; set; } = new List<SubFaction>();
    }
}
