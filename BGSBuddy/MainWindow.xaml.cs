using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using Repositories;
using Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace BGSBuddy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IFileSystemRepository fileSystemRepository;
        private IUserSettingsService userSettingsService;

        public SituationReport situationReport = new SituationReport();

        public MainWindow()
        {
            InitializeComponent();

            fileSystemRepository = new FileSystemRepository();
            userSettingsService = new UserSettingsService(fileSystemRepository);

            CheckForUpdates();
            GetSituations();
        }

        private void GetSettings()
        {
            var userSettings =  userSettingsService.Load().Result;
            situationReport.FactionName = userSettings.FactionName;
            situationReport.OffLimits = userSettings.OffLimits;
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
            ReportTitle.Text = situationReport.FactionName + " Situation Report";
            if (String.IsNullOrEmpty(situationReport.FactionName))
            {
                var popup = new Settings();
                popup.ShowDialog();
                RefreshButton.Content = "Refresh";
            }

            var repository = new EliteBgsRepository();
            situationReport.LastTick = await repository.GetTick();
            var faction = await repository.GetFaction(situationReport.FactionName, situationReport.LastTick);

            situationReport.CriticalReports.Clear();
            situationReport.WarningReports.Clear();
            situationReport.OpportunityReports.Clear();
            situationReport.ControlledReports.Clear();
            situationReport.PartialReports.Clear();

            foreach(var system in faction.SolarSystems)
            {
                if (situationReport.OffLimits.Any(e => e.ToLower() == system.Name.ToLower()))
                    continue;

                var influences = system.SubFactions.OrderByDescending(e => e.Influence).Select(e => e.Influence).ToList();

                bool weControl = false;
                bool totalControl = true;
                bool closeToConflict = false; 
                string states = string.Empty;

                if (system.ControllingFaction.Equals(situationReport.FactionName, StringComparison.OrdinalIgnoreCase))
                    weControl = true;
                if (influences[0] - 10 <= influences[1])
                    closeToConflict = true;
                if (system.Assets.Any(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()))
                    totalControl = false;
                if (system.States != null)
                    states = string.Join(",", system.States);

                // Stale Data
                if (system.UpdatedOn <= DateTime.UtcNow.AddDays(-2))
                    situationReport.WarningReports.Add(new Report(system.Name, "Stale Data", "Over " + (DateTime.UtcNow - system.UpdatedOn).Days.ToString() + " days old", states));

                // In or Pending Conflict
                if (!String.IsNullOrEmpty(system.ConflictType) && !String.IsNullOrEmpty(system.ConflictStatus))
                    situationReport.CriticalReports.Add(new Report(system.Name, system.ConflictType, system.ConflictStatus, states));
               
                // Asset Reallocation opportunity
                if (!totalControl && closeToConflict)
                    situationReport.OpportunityReports.Add(new Report(system.Name, "Asset Reallocation Opportunity", system.Assets.FirstOrDefault(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()).Faction, states));
                
                // Pointless conflict risk
                else if(closeToConflict && string.IsNullOrEmpty(system.ConflictType))
                    situationReport.WarningReports.Add(new Report(system.Name,"Pointless Conflict Risk","inf gap : " + Math.Round(influences[0] - influences[1], 2), states));
                
                // Total Control
                if (totalControl)
                    situationReport.ControlledReports.Add(new Report(system.Name, "Total Control", system.Assets.Count + " assets controlled.", states));
                // Unclaimed Assets
                else
                    situationReport.PartialReports.Add(new Report(system.Name, "Unclamed Assets", system.Assets.Count(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()) + " of " + system.Assets.Count + " assets unclaimed.", states));

                // Conquest opportunity
                if (!weControl)
                    situationReport.OpportunityReports.Add(new Report(system.Name, "Conquest Opportunity", "inf gap : " + Math.Round(influences[0] - influences[1], 2), states));

                // Subfaction considerations
                foreach(var subFaction in system.SubFactions)
                {
                    // We're in retreat
                    if (subFaction.Name == faction.Name && subFaction.ActiveStates.Exists(e => e == "Retreat"))
                        situationReport.CriticalReports.Add(new Report(system.Name, "Retreat", "We're in retreat!!!", states));
                    
                    // Other faction is in retreat
                    if (subFaction.Name != faction.Name && subFaction.ActiveStates.Exists(e => e == "Retreat"))
                        situationReport.OpportunityReports.Add(new Report(system.Name, "Retreat Opportunity", "Other minor faction is in retreat.", states));
                }
            }

            CriticalGrid.DataContext = situationReport.CriticalReports;
            if (!situationReport.CriticalReports.Any())
            {
                CriticalTitle.Visibility = Visibility.Collapsed;
                CriticalGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                CriticalTitle.Visibility = Visibility.Visible;
                CriticalGrid.Visibility = Visibility.Visible;
            }
            CriticalGrid.Items.Refresh();

            WarningGrid.DataContext = situationReport.WarningReports;
            if (!situationReport.WarningReports.Any())
            {
                WarningTitle.Visibility = Visibility.Collapsed;
                WarningGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                WarningTitle.Visibility = Visibility.Visible;
                WarningGrid.Visibility = Visibility.Visible;
            }
            WarningGrid.Items.Refresh();

            OpportunitiesGrid.DataContext = situationReport.OpportunityReports;
            if (!situationReport.OpportunityReports.Any())
            {
                OpportunitiesTitle.Visibility = Visibility.Collapsed;
                OpportunitiesGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                OpportunitiesTitle.Visibility = Visibility.Visible;
                OpportunitiesGrid.Visibility = Visibility.Visible;
            }
            OpportunitiesGrid.Items.Refresh();

            ControlledGrid.DataContext = situationReport.ControlledReports;
            if (!situationReport.ControlledReports.Any())
            {
                ControlledTitle.Visibility = Visibility.Collapsed;
                ControlledGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                ControlledTitle.Visibility = Visibility.Visible;
                ControlledGrid.Visibility = Visibility.Visible;
            }
            ControlledGrid.Items.Refresh();

            PartialGrid.DataContext = situationReport.PartialReports;
            if (!situationReport.PartialReports.Any())
            {
                PartialTitle.Visibility = Visibility.Collapsed;
                PartialGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                PartialTitle.Visibility = Visibility.Visible;
                PartialGrid.Visibility = Visibility.Visible;
            }
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
