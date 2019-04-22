using RuriLib.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per Configs.xaml
    /// </summary>
    public partial class Configs : Page
    {

        public ConfigManager ConfigManagerPage;
        public ConfigManager ConfigManagerPageX;
        public ConfigManager ConfigManagerPageS;
        public Stacker StackerPage;
        public ConfigOtherOptions OtherOptionsPage;
        public Convertion Converts;
        public ConfigViewModel CurrentConfig { get; set; }

        public Configs()
        {
            InitializeComponent();

            ConfigManagerPage = new ConfigManager(1);
            ConfigManagerPageX = new ConfigManager(2);
            ConfigManagerPageS = new ConfigManager(3);
            Globals.LogInfo(Components.ConfigManager, "Initialized Manager Page");

            menuOptionManager_MouseDown(this, null);
        }

        public void rere()
        {
            ConfigManagerPageX = new ConfigManager(2);
            ConfigManagerPageS = new ConfigManager(3);
        }

        public void searchrere(string hola)
        {
            ConfigManagerPageX.toSearcho(2, hola);
            ConfigManagerPageS.toSearcho(3, hola);
        }

        #region Menu Options
        private void menuOptionManager_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = ConfigManagerPage;
            menuOptionSelected(menuOptionManager);
        }

        public void menuOptionStacker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(CurrentConfig != null && StackerPage != null)
            {
                Main.Content = StackerPage;
                menuOptionSelected(menuOptionStacker);
            }
            else
            {
                Globals.LogError(Components.ConfigManager, "Cannot switch to stacker since no config is loaded or the loaded config isn't public");
            }
        }

        public void menuOptionConvert_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = StackerPage;
            menuOptionSelected(menuOptionConvert);
        }

        private void menuOptionOtherOptions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(CurrentConfig != null)
            {
                if (OtherOptionsPage == null)
                    OtherOptionsPage = new ConfigOtherOptions();    
                
                Main.Content = OtherOptionsPage;
                menuOptionSelected(menuOptionOtherOptions);
            }
            else
            {
                Globals.LogError(Components.ConfigManager, "Cannot switch to other options since no config is loaded");
            }
        }

        private void menuOptionSelected(object sender)
        {
            foreach (var child in topMenu.Children)
            {
                try
                {
                    //var a = "";
                    var c = (Label)child;
                    c.Foreground = Globals.GetBrush("ForegroundMain");
                }
                catch { }
            }
            ((Label)sender).Foreground = Globals.GetBrush("ForegroundCustom");
        }
        #endregion

    }
}
