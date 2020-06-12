using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using Newtonsoft.Json;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BGSBuddy
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private IFileSystemRepository fileSystemRepository;
        private IUserSettingsService userSettingsService;

        public Settings()
        {
            InitializeComponent();

            fileSystemRepository = new FileSystemRepository();
            userSettingsService = new UserSettingsService(fileSystemRepository);

            var settings = userSettingsService.Load().Result;
            FactionName.Text = settings.FactionName;
            OffLimits.Text = string.Join(",", settings.OffLimits);

            Properties.Settings.Default.Faction = FactionName.Text;
            Properties.Settings.Default.OffLimits = OffLimits.Text;
            Properties.Settings.Default.Save();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new UserSettings();
            settings.FactionName = FactionName.Text;
            settings.OffLimits = OffLimits.Text.Split(',').ToList();
            userSettingsService.Save(settings).Wait();

            Properties.Settings.Default.Faction = FactionName.Text;
            Properties.Settings.Default.OffLimits = OffLimits.Text;
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
