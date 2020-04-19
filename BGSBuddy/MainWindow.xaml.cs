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
            var repository = new Repositories.EliteBgsRepository();
            var faction = await repository.GetFaction("Alliance Rapid-reaction Corps");
            var offLimits = new List<string> { "Biria", "Bruthanvan", "CD-45 7854", "Colando Po", "HIP 61097", "Kebes", "LTT 5058", "Mulukang", "Orerve", "Quator", "Reorte", "Tiveronisa", "Virawn", "Xucuri", "Tionisla" };

            foreach(var system in faction.SolarSystems)
            {
                // do not include anything off limits
                if (offLimits.Any(e => e.ToLower() == system.Name.ToLower()))
                    continue;

                var influences = system.SubFactions.OrderByDescending(e => e.Influence).Select(e => e.Influence).ToList();

                if (system.UpdatedOn <= DateTime.UtcNow.AddDays(-1))
                {
                    var warningReport = new Report
                    {
                        Location = system.Name,
                        Situation = "Stale Data",
                        Condition = "Over " + (DateTime.UtcNow - system.UpdatedOn).Days.ToString() + " days old"
                    };
                    WarningReports.Add(warningReport);
                }

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

                    if (system.Assets.Any(e => e.Faction.ToLower() != "alliance rapid-reaction corps"))
                    {
                        if (influences[0] - 10 <= influences[1])
                        {
                            var opportunityReport = new Report
                            {
                                Location = system.Name,
                                Situation = "Asset Reallocation Opportunity",
                                Condition = system.Assets.FirstOrDefault(e => e.Faction.ToLower() != "alliance rapid-reaction corps").Faction
                            };
                            OpportunityReports.Add(opportunityReport);
                        }
                        report.Condition = "Assets unclaimed";
                    }
                    else
                    {
                        if (influences[0] - 10 <= influences[1])
                        {
                            var warningReport = new Report
                            {
                                Location = system.Name,
                                Situation = "Pointless Conflict Risk",
                                Condition = "inf gap : " + Math.Round(influences[0] - influences[1],2)
                            };
                            WarningReports.Add(warningReport);
                        }

                        report.Condition = "Total control";
                    }
                    report.Situation = string.Empty;
                    ControlledReports.Add(report);
                }
                else
                {
                    var opportunityReport = new Report
                    {
                        Location = system.Name,
                        Situation = "Conquest Opportunity",
                        Condition = "inf gap : " + Math.Round(influences[0] - influences[1], 2)
                    };
                    OpportunityReports.Add(opportunityReport);
                }
            }

            CriticalGrid.DataContext = CriticalReports;
            WarningGrid.DataContext = WarningReports;
            OpportunitiesGrid.DataContext = OpportunityReports;
            ControlledGrid.DataContext = ControlledReports;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GetSituations();
        }
    }
}
