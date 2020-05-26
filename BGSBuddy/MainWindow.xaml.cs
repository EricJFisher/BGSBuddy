using BGSBuddy.ViewModels;
using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
        public List<Report> PartialReports = new List<Report>();
        private DateTime lastTick;
        private string myFaction;
        private List<string> offLimits;

        public MainWindow()
        {
            InitializeComponent();
            CheckForUpdates();
            GetSituations();
        }

        private void GetSettings()
        {
            var repository = new FileSystemRepository();
            var json = repository.RetrieveJsonFromFile("Settings.txt").Result;
            if (!string.IsNullOrEmpty(json))
            {
                var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSettings>(json);
                myFaction = settings.FactionName;
                offLimits = settings.OffLimitsList?.Split(',')?.ToList();
            }
            else
            {
                myFaction = Properties.Settings.Default.Faction;
                offLimits = Properties.Settings.Default.OffLimits?.Split(',')?.ToList();
            }
        }

        private void CheckForUpdates()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var updates = GithubRepository.HasUpdates(version).Result;
            if(updates)
            {
                UpdateNotification.Visibility = Visibility.Visible;
                UpdateNotification.UpdateLayout();
            }
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
            PartialReports.Clear();

            foreach(var system in faction.SolarSystems)
            {
                if (offLimits.Any(e => e.ToLower() == system.Name.ToLower()))
                    continue;

                var influences = system.SubFactions.OrderByDescending(e => e.Influence).Select(e => e.Influence).ToList();

                bool weControl = false;
                bool totalControl = true;
                bool closeToConflict = false; 
                string states = string.Empty;

                if (system.ControllingFaction.Equals(myFaction, StringComparison.OrdinalIgnoreCase))
                    weControl = true;
                if (influences[0] - 10 <= influences[1])
                    closeToConflict = true;
                if (system.Assets.Any(e => e.Faction.ToLower() != myFaction.ToLower()))
                    totalControl = false;
                if (system.States != null)
                    states = string.Join(",", system.States);

                // Stale Data
                if (system.UpdatedOn <= DateTime.UtcNow.AddDays(-2))
                    WarningReports.Add(new Report(system.Name, "Stale Data", "Over " + (DateTime.UtcNow - system.UpdatedOn).Days.ToString() + " days old", states));

                // In or Pending Conflict
                if (!String.IsNullOrEmpty(system.ConflictType) && !String.IsNullOrEmpty(system.ConflictStatus))
                    CriticalReports.Add(new Report(system.Name, system.ConflictType, system.ConflictStatus, states));
               
                // Asset Reallocation opportunity
                if (!totalControl && closeToConflict)
                    OpportunityReports.Add(new Report(system.Name, "Asset Reallocation Opportunity", system.Assets.FirstOrDefault(e => e.Faction.ToLower() != myFaction.ToLower()).Faction, states));
                
                // Pointless conflict risk
                else if(closeToConflict && string.IsNullOrEmpty(system.ConflictType))
                    WarningReports.Add(new Report(system.Name,"Pointless Conflict Risk","inf gap : " + Math.Round(influences[0] - influences[1], 2), states));
                
                // Total Control
                if (totalControl)
                    ControlledReports.Add(new Report(system.Name, "Total Control", system.Assets.Count + " assets controlled.", states));
                // Unclaimed Assets
                else
                    PartialReports.Add(new Report(system.Name, "Unclamed Assets", system.Assets.Count(e => e.Faction.ToLower() != myFaction.ToLower()) + " of " + system.Assets.Count + " assets unclaimed.", states));

                // Conquest opportunity
                if (!weControl)
                    OpportunityReports.Add(new Report(system.Name, "Conquest Opportunity", "inf gap : " + Math.Round(influences[0] - influences[1], 2), states));

                // Subfaction considerations
                foreach(var subFaction in system.SubFactions)
                {
                    // We're in retreat
                    if (subFaction.Name == faction.Name && subFaction.ActiveStates.Exists(e => e == "Retreat"))
                        CriticalReports.Add(new Report(system.Name, "Retreat", "We're in retreat!!!", states));
                    
                    // Other faction is in retreat
                    if (subFaction.Name != faction.Name && subFaction.ActiveStates.Exists(e => e == "Retreat"))
                        OpportunityReports.Add(new Report(system.Name, "Retreat Opportunity", "Other minor faction is in retreat.", states));
                }
            }

            CriticalGrid.DataContext = CriticalReports;
            CriticalGrid.Items.Refresh();
            WarningGrid.DataContext = WarningReports;
            WarningGrid.Items.Refresh();
            OpportunitiesGrid.DataContext = OpportunityReports;
            OpportunitiesGrid.Items.Refresh();
            ControlledGrid.DataContext = ControlledReports;
            ControlledGrid.Items.Refresh();
            PartialGrid.DataContext = PartialReports;
            PartialGrid.Items.Refresh();
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

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri){ UseShellExecute = true });
            e.Handled = true;
        }
    }
}
