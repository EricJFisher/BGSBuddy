using BGSBuddy.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BGSBuddy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Report> CriticalReports = new List<Report>();
        public List<Report> WarningReports = new List<Report>();
        public List<Report> OpportunityReports = new List<Report>();
        public List<Report> ControlledReports = new List<Report>();

        public MainWindow()
        {
            InitializeComponent();
            GetSituations();
        }

        private async Task GetSituations()
        {
            var repository = new Repositories.EliteBgsRepository(new Repositories.FileSystemRepository());
            var faction = await repository.GetFaction("Alliance Rapid-reaction Corps");

            foreach(var system in faction.SolarSystems)
            {
                if (system.ControllingFaction.Equals("Alliance Rapid-reaction Corps",StringComparison.OrdinalIgnoreCase))
                {
                    var report = new Report();
                    report.Location = system.Name;
                    // Critical
                    if(!String.IsNullOrEmpty(system.ConflictType))
                    {
                        report.Situation = system.ConflictType;
                        report.Condition = system.ConflictStatus;
                        CriticalReports.Add(report);
                    }
                    
                    if(system.Assets.Any(e => e.Faction.ToLower() != "alliance rapid-reaction corps"))
                    {
                        var opportunityReport = new Report();
                        opportunityReport.Location = system.Name;
                        opportunityReport.Situation = "Asset Reallocation Opportunity";
                        opportunityReport.Condition = system.Assets.FirstOrDefault(e => e.Faction.ToLower() != "alliance rapid-reaction corps").Faction;
                        OpportunityReports.Add(opportunityReport);
                        report.Condition = "Assets unclaimed";
                    }
                    else
                    {
                        report.Condition = "Total control";
                    }
                    report.Situation = string.Empty;
                    ControlledReports.Add(report);
                }
            }

            CriticalGrid.DataContext = CriticalReports;
            WarningGrid.DataContext = WarningReports;
            OpportunitiesGrid.DataContext = OpportunityReports;
            ControlledGrid.DataContext = ControlledReports;
        }

        private List<Report> GetTestData()
        {
            return new List<Report> 
            {
                new Report{ Location = "SystemOne", Condition = "Active", Situation = "War" },
                new Report{ Location = "SystemTwo", Condition = "Active", Situation = "Election" }
            };
        }
    }
}
