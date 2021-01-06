using System;
using System.Collections.Generic;

namespace Entities
{
    public class ExpansionReport
    {
        public string FactionName { get; set; }
        public DateTime LastTick { get; set; } = new DateTime();
        public DateTime TimeOfReport
        {
            get { return DateTime.UtcNow; }
        }
        public string ExpandFromSystem { get; set; }
        public List<string> OffLimits { get; set; } = new List<string>();
        public List<SolarSystem> NeverRetreatedSystemsWithSpace { get; set; } = new List<SolarSystem>();
        public List<SolarSystem> RetreatedSystemsWithSpace { get; set; } = new List<SolarSystem>();
        public List<InvasionReport> InvasionTargets { get; set; } = new List<InvasionReport>();
    }
}