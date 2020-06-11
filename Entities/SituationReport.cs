using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public class SituationReport
    {
        private static readonly Task<SituationReport> _getInstanceTask = CreateSingleton();

        public string FactionName { get; set; } = string.Empty;
        public DateTime LastTick { get; set; } = new DateTime();
        public List<Report> CriticalReports = new List<Report>();
        public List<Report> WarningReports = new List<Report>();
        public List<Report> OpportunityReports = new List<Report>();
        public List<Report> ControlledReports = new List<Report>();
        public List<Report> PartialReports = new List<Report>();
        public List<string> OffLimits { get; set; } = new List<string>();

        public bool HasChanged { get; set; } = true;

        public static Task<SituationReport> Instance
        {
            get { return _getInstanceTask; }
        }

        private SituationReport()
        {

        }

        private static async Task<SituationReport> CreateSingleton()
        {
            var situationReport = new SituationReport();
            // Get Settings


            // Generate Report

            return situationReport;
        }
    }
}
