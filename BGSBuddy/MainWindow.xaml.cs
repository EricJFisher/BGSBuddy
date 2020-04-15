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
        private Faction faction;

        public MainWindow()
        {
            InitializeComponent();
            faction = GetTestData();
        }

        private Faction GetTestData()
        {
            return new Faction {
                Name = "Alliance Rapid-Reaction Corps",
                UpdatedOn = DateTime.UtcNow,
                SolarSystems = new List<SolarSystem>
                {
                    new SolarSystem
                    {
                        Name = "TestA",
                        UpdatedOn = DateTime.UtcNow,
                        ConflictStatus = string.Empty,
                        ConflictType = string.Empty,
                        ControllingFaction = "Alliance Rapid-Reaction Corps",
                        Assets = new List<Asset>
                        {
                            new Asset
                            {
                                Name = "StationOne",
                                SolarSystem = "TestA",
                                Faction = "Alliance Rapid-Reaction Corps"
                            }
                        }
                    }
                }
            };
        }
    }
}
