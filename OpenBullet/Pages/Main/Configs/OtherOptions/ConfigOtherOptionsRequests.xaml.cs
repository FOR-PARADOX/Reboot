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

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per ConfigOtherOptionsRequests.xaml
    /// </summary>
    public partial class ConfigOtherOptionsRequests : Page
    {
        public ConfigOtherOptionsRequests()
        {
            InitializeComponent();
            try
            {
                DataContext = Globals.mainWindow.ConfigsPage.CurrentConfig.Config.Settings;
            }
            catch
            {
                Globals.LogError(Components.ConfigManager, "You can't edit this.", true);
            }
        }
    }
}
