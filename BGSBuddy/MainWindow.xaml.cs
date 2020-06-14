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
        private IAssetsService assetsService;
        private IEliteBgsRepository eliteBgsRepository;
        private IFactionsService factionsService;
        private IFileSystemRepository fileSystemRepository;
        private ISituationReportsService situationReportsService;
        private ISolarSystemsService solarSystemsService;
        private ITickService tickService;
        private IUserSettingsService userSettingsService;

        public SituationReport situationReport = new SituationReport();

        public MainWindow()
        {
            InitializeComponent();

            eliteBgsRepository = new EliteBgsRepository();
            assetsService = new AssetsService(eliteBgsRepository);
            factionsService = new FactionsService(eliteBgsRepository);
            solarSystemsService = new SolarSystemsService(eliteBgsRepository);
            tickService = new TickService(eliteBgsRepository);
            situationReportsService = new SituationReportsService(assetsService, factionsService, solarSystemsService, tickService);
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

            situationReport.CriticalReports.Clear();
            situationReport.WarningReports.Clear();
            situationReport.OpportunityReports.Clear();
            situationReport.ControlledReports.Clear();
            situationReport.PartialReports.Clear();

            situationReport = await situationReportsService.GenerateReport(situationReport);

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
