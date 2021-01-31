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
        private IEddbRepository eddbRepository;
        private IEddnRepository eddnRepository;
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
            eddnRepository = new EddnRepository();
            eddbRepository = new EddbRepository();
            assetsService = new AssetsService(eliteBgsRepository);
            factionsService = new FactionsService(eliteBgsRepository);
            solarSystemsService = new SolarSystemsService(eliteBgsRepository, eddbRepository);
            tickService = new TickService(eliteBgsRepository);
            situationReportsService = new SituationReportsService(assetsService, factionsService, solarSystemsService, tickService);
            fileSystemRepository = new FileSystemRepository();
            userSettingsService = new UserSettingsService(fileSystemRepository);

            CheckForUpdates();
            GetSituations();
            //eddnRepository.ListenToEddn();
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
            if (ErrorBanner.Visibility == Visibility.Visible)
                ErrorBanner.Visibility = Visibility.Collapsed;

            RefreshButton.Content = "Updating, Please Wait";
            GetSettings();
            ReportTitle.Text = situationReport.FactionName + " Situation Report";
            if (string.IsNullOrEmpty(situationReport.FactionName))
            {
                var popup = new Settings();
                popup.ShowDialog();
                RefreshButton.Content = "Refresh";
            }

            situationReport.CriticalReports.Clear();
            situationReport.WarningReports.Clear();
            situationReport.OpportunityReports.Clear();
            situationReport.DataReports.Clear();
            situationReport.ControlledReports.Clear();
            situationReport.PartialReports.Clear();

            try
            {
                situationReport = await situationReportsService.GenerateReport(situationReport);
            }
            catch (Exception ex)
            {
                ErrorBanner.Visibility = Visibility.Visible;
                ErrorBanner.Text = "An error has occurred, try again later. (" + ex.Message + ")";
                RefreshButton.Content = "Refresh";
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

            DataReportsGrid.DataContext = situationReport.DataReports;
            if (!situationReport.DataReports.Any())
            {
                DataReportsTitle.Visibility = Visibility.Collapsed;
                DataReportsGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                DataReportsTitle.Visibility = Visibility.Visible;
                DataReportsGrid.Visibility = Visibility.Visible;
            }
            DataReportsGrid.Items.Refresh();

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

        private void ExpansionButton_OnClick(object sender, RoutedEventArgs e)
        {
            var expansionPlanner = new ExpansionPlanner();
            expansionPlanner.Show();
        }
    }
}
