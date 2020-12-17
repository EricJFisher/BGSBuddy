using Entities;
using System.Collections.Generic;

namespace BGSBuddyWeb.Models
{
    public class ExpansionPlannerViewModel
    {
        public string SystemName { get; set; } = string.Empty;
        public List<SolarSystem> Systems { get; set; } = new List<SolarSystem>();
        public List<SolarSystem> NeverRetreatedSystems { get; set; } = new List<SolarSystem>();
        public List<SolarSystem> RetreatedSystems { get; set; } = new List<SolarSystem>();
        public List<SolarSystem> InvasionSystems { get; set; } = new List<SolarSystem>();
    }
}
