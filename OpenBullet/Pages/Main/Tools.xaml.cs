using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per Tools.xaml
    /// </summary>
    public partial class Tools : Page
    {
        ToolsListGenerator ListGenerator;
        ToolsSeleniumTools SeleniumTools;
        OpenBullet.Pages.Main.Tools.ComboSuite ComboSuites;

        public Tools()
        {
            InitializeComponent();

            ListGenerator = new ToolsListGenerator();
            SeleniumTools = new ToolsSeleniumTools();
            ComboSuites = new OpenBullet.Pages.Main.Tools.ComboSuite();

            menuOptionListGenerator_MouseDown(this, null);
        }

        #region Menu Options
        private void menuOptionListGenerator_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = ListGenerator;
            menuOptionSelected(menuOptionListGenerator);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private void menuOptionDevs(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Only OB Developers are allowed to use this!", "Smart boi");
        }

        private void menuOptionSeleniumUtilities_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = SeleniumTools;
            menuOptionSelected(menuOptionSeleniumTools);
        }

        private void menuOptionComboSuite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = ComboSuites;
            menuOptionSelected(menuOptionComboSuite);
        }

        private void menuOptionSelected(object sender)
        {
            foreach (var child in topMenu.Children)
            {
                try
                {
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
