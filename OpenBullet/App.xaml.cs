using System;
using System.Windows;
using System.Reflection;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        protected override void OnStartup(StartupEventArgs e)
        {
            bool flag = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "OpsThisIsByForlax")
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                MessageBox.Show("Please Run OB from the Launcher");
                Environment.Exit(0);
            }
        }
    }
}
