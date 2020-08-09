using System;
using System.Collections.Generic;

namespace Entities
{
    public class SituationReport
    {
        public string FactionName { get; set; } = string.Empty;
        public DateTime LastTick { get; set; } = new DateTime();
        public List<Report> CriticalReports = new List<Report>();
        public List<Report> WarningReports = new List<Report>();
        public List<Report> OpportunityReports = new List<Report>();
        public List<Report> ControlledReports = new List<Report>();
        public List<Report> PartialReports = new List<Report>();
        public List<string> OffLimits { get; set; } = new List<string>();

        public Faction Faction { get; set; } = new Faction();

        public bool HasChanged { get; set; } = false;
    }
}
