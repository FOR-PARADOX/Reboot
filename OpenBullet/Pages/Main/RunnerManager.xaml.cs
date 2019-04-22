using OpenBullet.ViewModels;
using RuriLib.Runner;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per RunnerManager.xaml
    /// </summary>
    public partial class RunnerManager : Page
    {
        public RunnerManagerViewModel vm = new RunnerManagerViewModel();
        private bool DelegateCalled { get; set; } = false;

        public delegate void StartRunnerEventHandler(object sender, EventArgs e);
        public event StartRunnerEventHandler StartRunner;
        protected virtual void OnStartRunner()
        {
            StartRunner?.Invoke(this, EventArgs.Empty);
        }

        private void startAllRunnersButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var runner in vm.Runners.Where(r => !r.Runner.Busy))
            {
                StartRunner += runner.Page.OnStartRunner;
                OnStartRunner();
                StartRunner -= runner.Page.OnStartRunner;
            }
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static void LoadUtilities()
        {
            string html = "0";
            string shiot = Globals.LK;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/ForlaxPy/openbullet-1.2/master/obsauce/banned");
            request.Proxy = null;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (System.IO.Stream stream = response.GetResponseStream())
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            if (html.Contains("{" + shiot + "}") && html != "0")
            {
                string html2 = "1";
                request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/ForlaxPy/openbullet-1.2/master/obsauce/banned");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    html2 = reader.ReadToEnd();
                }
                if (html2.Contains("{" + shiot + "}") && html2 != "1")
                {
                    MessageBoxResult result = MessageBox.Show("You're banned from using this OpenBullet version contact us in discord if you wanna appeal", "OpenBullet CyberSecurity Team", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start("https://discord.gg/eYHDs6z");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void CheckUpdates()
        {
            //check ban firstly 
            File.WriteAllText("version.txt", Globals.obVersion);
            string html = "0";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/FOR-PARADOX/Reboot/master/obsauce/version.txt");
            request.Proxy = null;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (System.IO.Stream stream = response.GetResponseStream())
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            string OBversion = Regex.Match(html, ":version:(.*?);version;").Groups[1].Value;
            OBversion = "1.2.2.7";
            string OBdownload = Regex.Match(html, ":download:(.*?);download;").Groups[1].Value;
            string OBpr = Regex.Match(html, ":priority:(.*?);priority;").Groups[1].Value;
            if (Globals.obVersion != OBversion)
            {



            }   
            }
        

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void RunnerUtils(bool createFirst)
        {
            RunUtils();
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void RunUtils()
        {
            InitializeComponent();
            DataContext = vm;
            CheckUpdates();
            LoadUtilities();
            addRunnerButton_Click(this, null);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void BlackBullet()
        {
            try
            {
                bool shiot = true;
                Globals.spWindow = new SplashWindow();
                Globals.spWindow.Show();
                Thread t = new Thread((delegate ()
                {
                    while (shiot)
                    {
                        if (SplashWindow.reg == "OMGBBISBACK")
                        {
                            shiot = false;
                            RunUtils();
                        }
                        Thread.Sleep(2000);
                    }
                }));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            catch { }
        }

        public RunnerManager(bool createFirst)
        {
            RunnerUtils(createFirst);
        }

        private void addRunnerButton_Click(object sender, RoutedEventArgs e)
        {
            vm.CreateRunner();
            helpMessageLabel.Visibility = Visibility.Collapsed;
        }

        private void removeRunnerButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (int)((Button)e.OriginalSource).Tag;
            if (vm.GetRunnerById(id).Runner.Master.Status != WorkerStatus.Idle)
            {
                MessageBox.Show("The Runner is active! Please stop it before removing it.");
                return;
            }
            vm.RemoveRunnerById(id);
        }

        private void startRunnerButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (int)((Button)e.OriginalSource).Tag;
            var runner = vm.GetRunnerById(id);

            StartRunner += runner.Page.OnStartRunner;
            OnStartRunner();
            StartRunner -= runner.Page.OnStartRunner;
        }

        private void runnerInstanceGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DelegateCalled)
            {
                DelegateCalled = false;
                return;
            }

            if (sender.GetType() == typeof(Grid))
            {
                var id = (int)(sender as Grid).Tag;
                Globals.mainWindow.ShowRunner(vm.GetRunnerById(id).Page);
            }
        }
        private void runnerInstanceGrid_MouseDown2()
        {
            if (DelegateCalled)
            {
                DelegateCalled = false;
                return;
            }

                var id = 1;
                Globals.mainWindow.ShowRunner(vm.GetRunnerById(id).Page);
        }

        #region Quick Access Setters
        private void selectConfig_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var id = (int)(FindParent<Grid>(sender as DependencyObject)).Tag;
            var runner = Globals.mainWindow.RunnerManagerPage.vm.GetRunnerById(id);

            if (!runner.Runner.Busy)
            {
                DelegateCalled = true;
                (new MainDialog(new DialogSelectConfig(runner.Page, 3), "Select Config")).ShowDialog();
            }
        }

        private void selectWordlist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var id = (int)(FindParent<Grid>(sender as DependencyObject)).Tag;
            var runner = Globals.mainWindow.RunnerManagerPage.vm.GetRunnerById(id);

            if (!runner.Runner.Busy)
            {
                DelegateCalled = true;
                (new MainDialog(new DialogSelectWordlist(runner.Page), "Select Wordlist")).ShowDialog();
            }
        }

        private void selectProxies_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var id = (int)(FindParent<Grid>(sender as DependencyObject)).Tag;
            var runner = Globals.mainWindow.RunnerManagerPage.vm.GetRunnerById(id);

            if (!runner.Runner.Busy)
            {
                DelegateCalled = true;
                (new MainDialog(new DialogSetProxies(runner.Page), "Set Proxies")).ShowDialog();
            }
        }

        private void selectBots_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var id = (int)(FindParent<Grid>(sender as DependencyObject)).Tag;
            var runner = Globals.mainWindow.RunnerManagerPage.vm.GetRunnerById(id);

            if (!runner.Runner.Busy)
            {
                DelegateCalled = true;
                (new MainDialog(new DialogSelectBots(runner.Page, runner.Runner.BotsNumber), "Select Bots Number")).ShowDialog();
            }
        }
        #endregion

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null) return parent;
            else return FindParent<T>(parentObject);
        }

        private void stopAllRunnersButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var runner in vm.Runners.Where(r => r.Runner.Busy))
            {
                StartRunner += runner.Page.OnStartRunner;
                OnStartRunner();
                StartRunner -= runner.Page.OnStartRunner;
            }
        }

        private void removeAllRunnersButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to remove all Runners?", 
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var list = vm.Runners.Where(r => !r.Runner.Busy).ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    vm.Runners.Remove(list[i]);
                }
            }
        }
    }
}
