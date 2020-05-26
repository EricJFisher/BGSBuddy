using Entities;
using Newtonsoft.Json;
using Repositories;
using System;
using System.Collections.Generic;
using System.IO;
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
        public Settings()
        {
            InitializeComponent();
            FactionName.Text = Properties.Settings.Default.Faction;
            OffLimits.Text = Properties.Settings.Default.OffLimits;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var repository = new FileSystemRepository();
            var settings = new UserSettings();
            settings.FactionName = FactionName.Text;
            settings.OffLimitsList = OffLimits.Text;

            var json = JsonConvert.SerializeObject(settings);
            repository.SaveJsonToFile(json, "Settings.txt").Wait();

            Properties.Settings.Default.Faction = FactionName.Text;
            Properties.Settings.Default.OffLimits = OffLimits.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
