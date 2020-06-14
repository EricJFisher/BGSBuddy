using System;
using System.Collections.Generic;

namespace Entities
{
    public class SolarSystem
    {
        public string Name { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ControllingFaction { get; set; }
        public string State { get; set; } = string.Empty;
        public List<string> ActiveStates { get; set; } = new List<string>();
        public List<string> PendingStates { get; set; } = new List<string>();
        public List<Asset> Assets { get; set; } = new List<Asset>();
        public List<Conflict> Conflicts { get; set; } = new List<Conflict>();
        public List<SubFaction> SubFactions { get; set; } = new List<SubFaction>();
    }
}
