﻿using System;
using System.Collections.Generic;
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
            Properties.Settings.Default.Faction = FactionName.Text;
            Properties.Settings.Default.OffLimits = OffLimits.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
