using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using Repositories;
using Services;

namespace BGSBuddy
{
    public partial class ExpansionPlanner : Window
    {
        private IEliteBgsRepository eliteBgsRepository;
        private ISolarSystemsService solarSystemsService;
        private IFileSystemRepository fileSystemRepository;
        private IUserSettingsService userSettingsService;
        private ITickService tickService;

        private ExpansionReport expansionReport = new ExpansionReport();

        public ExpansionPlanner()
        {
            InitializeComponent();

            eliteBgsRepository = new EliteBgsRepository();
            solarSystemsService = new SolarSystemsService(eliteBgsRepository);
            fileSystemRepository = new FileSystemRepository();
            tickService = new TickService(eliteBgsRepository);
            userSettingsService = new UserSettingsService(fileSystemRepository);
            
            GetSettings();
            if (!string.IsNullOrEmpty(SystemNameTextBox.Text))
            {
                expansionReport.ExpandFromSystem = SystemNameTextBox.Text;
                GetExpansionReport();
            }
        }
        
        private void GetSettings()
        {
            var userSettings =  userSettingsService.Load().Result;
            expansionReport.FactionName = userSettings.FactionName;
            expansionReport.OffLimits = userSettings.OffLimits;
        }
        
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SystemNameTextBox.Text))
            {
                expansionReport.ExpandFromSystem = SystemNameTextBox.Text;
                GetExpansionReport();
            }
        }

        private void Override_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "State":
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case "ActiveStates":
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case "PendingStates":
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case "Assets":
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case "Conflicts":
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case "SubFactions":
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                default:
                    e.Column.Visibility = Visibility.Visible;
                    break;
            }
        }

        private async Task GetExpansionReport()
        {
            RefreshButton.Content = "Generating Report, please wait.";
            expansionReport.LastTick = await tickService.Get();
            var expansionTargets = await solarSystemsService.GetExpansionTargets(expansionReport);

            SpaceAvailableGrid.DataContext = expansionTargets.NeverRetreatedSystemsWithSpace;
            SpaceAvailableGrid.Items.Refresh();
            InvasionTargetGrid.DataContext = expansionTargets.InvasionTargets;
            InvasionTargetGrid.CanUserSortColumns = false;
            InvasionTargetGrid.Items.SortDescriptions.Add(new SortDescription("Influence", ListSortDirection.Ascending));
            InvasionTargetGrid.Items.Refresh();

            RefreshButton.Content = "Get Report";
        }
    }
}