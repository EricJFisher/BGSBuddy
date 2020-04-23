using BGSBuddy.ViewModels;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        private DateTime lastTick;
        private string myFaction;
        private List<string> offLimits;

        public MainWindow()
        {
            InitializeComponent();
            GetSituations();
        }

        private void GetSettings()
        {
            myFaction = Properties.Settings.Default.Faction;
            offLimits = Properties.Settings.Default.OffLimits?.Split(',')?.ToList();
        }

        private async Task GetSituations()
        {
            RefreshButton.Content = "Updating, Please Wait";
            GetSettings();
            ReportTitle.Text = myFaction + " Situation Report";
            if (String.IsNullOrEmpty(myFaction))
            {
                var popup = new Settings();
                popup.ShowDialog();
                RefreshButton.Content = "Refresh";
            }

            var repository = new EliteBgsRepository();
            lastTick = await repository.GetTick();
            var faction = await repository.GetFaction(myFaction, lastTick);

            CriticalReports.Clear();
            WarningReports.Clear();
            OpportunityReports.Clear();
            ControlledReports.Clear();

            foreach(var system in faction.SolarSystems)
            {
                if (offLimits.Any(e => e.ToLower() == system.Name.ToLower()))
                    continue;

                var influences = system.SubFactions.OrderByDescending(e => e.Influence).Select(e => e.Influence).ToList();

                bool weControl = false;
                bool totalControl = true;
                bool closeToConflict = false;

                if (system.ControllingFaction.Equals(myFaction, StringComparison.OrdinalIgnoreCase))
                    weControl = true;
                if (influences[0] - 10 <= influences[1])
                    closeToConflict = true;
                if (system.Assets.Any(e => e.Faction.ToLower() != myFaction))
                    totalControl = false;

                // Stale Data
                if (system.UpdatedOn <= DateTime.UtcNow.AddDays(-2))
                    WarningReports.Add(new Report(system.Name, "Stale Data", "Over " + (DateTime.UtcNow - system.UpdatedOn).Days.ToString() + " days old"));

                // In or Pending Conflict
                if (!String.IsNullOrEmpty(system.ConflictType) && !String.IsNullOrEmpty(system.ConflictStatus))
                    CriticalReports.Add(new Report(system.Name, system.ConflictType, system.ConflictStatus));
                
                // Asset Reallocation opportunity
                if (!totalControl && closeToConflict)
                    OpportunityReports.Add(new Report(system.Name, "Asset Reallocation Opportunity", system.Assets.FirstOrDefault(e => e.Faction.ToLower() != myFaction.ToLower()).Faction));
                // Pointless conflict risk
                else if(closeToConflict)
                    WarningReports.Add(new Report(system.Name,"Pointless Conflict Risk","inf gap : " + Math.Round(influences[0] - influences[1], 2)));
                
                // Total Control
                if (totalControl)
                    ControlledReports.Add(new Report(system.Name, "Total Control", system.Assets.Count + " assets controlled."));
                // Unclaimed Assets
                else
                    ControlledReports.Add(new Report(system.Name, "Unclamed Assets", system.Assets.Count + " assets unclaimed."));

                // Conquest opportunity
                if (!weControl)
                    OpportunityReports.Add(new Report(system.Name, "Conquest Opportunity", "inf gap : " + Math.Round(influences[0] - influences[1], 2)));
            }

            CriticalGrid.DataContext = CriticalReports;
            CriticalGrid.Items.Refresh();
            WarningGrid.DataContext = WarningReports;
            WarningGrid.Items.Refresh();
            OpportunitiesGrid.DataContext = OpportunityReports;
            OpportunitiesGrid.Items.Refresh();
            ControlledGrid.DataContext = ControlledReports;
            ControlledGrid.Items.Refresh();
            RefreshButton.Content = "Refresh";
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await GetSituations();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = new Settings();
            popup.ShowDialog();
        }
    }
}
