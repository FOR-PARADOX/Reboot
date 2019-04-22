using System.Windows.Controls;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per ConfigOtherOptionsGeneral.xaml
    /// </summary>
    public partial class ConfigOtherOptionsGeneral : Page
    {
        public ConfigOtherOptionsGeneral()
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
