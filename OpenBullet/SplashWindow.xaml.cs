using System;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public static string reg;
        public SplashWindow()
        { 
            InitializeComponent();
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void login(object sender, RoutedEventArgs e)
        {
            if (usernameTextbox.Text == "Guest")
            {
                Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    resultLabel.Content = "Logged in successfully";
                }));
                reg = "OMGBBISBACK";
            }
            else
            {
                MessageBox.Show("Username not found in our database", "OpenBullet 1.2");
            }
           
        }

        private void HandleInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void register(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Locked for now", "OpenBullet 1.2");
        }

        private void reset(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Locked for now", "OpenBullet 1.2");
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

        private void quitImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
