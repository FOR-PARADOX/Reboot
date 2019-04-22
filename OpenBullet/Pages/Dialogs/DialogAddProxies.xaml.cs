using Extreme.Net;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per DialogAddProxies.xaml
    /// </summary>
    public partial class DialogAddProxies : Page
    {
        public object Caller { get; set; }

        public static string PSHTTP = "0 minutes ago";
        public static string PSSOCKS4 = "0 minutes ago";
        public static string PSSOCKS5 = "0 minutes ago";

        public DialogAddProxies(object caller)
        {
            InitializeComponent();
            Caller = caller;
            foreach (string i in Enum.GetNames(typeof(ProxyType)))
                if(i != "Chain") proxyTypeCombobox.Items.Add(i);
            proxyTypeCombobox.SelectedIndex = 0;
            CheckLaestPSupdates();
            HTTPupdate.Content = PSHTTP;
            SOCKS4update.Content = PSSOCKS4;
            SOCKS5update.Content = PSSOCKS4;
        }

        private void CheckLaestPSupdates()
        {
            try
            {


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://proxyscrape.com/api/?request=lastupdated&proxytype=http");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    PSHTTP = reader.ReadToEnd();
                }

                request = (HttpWebRequest)WebRequest.Create("https://proxyscrape.com/api/?request=lastupdated&proxytype=socks4");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    PSSOCKS4 = reader.ReadToEnd();
                }

                request = (HttpWebRequest)WebRequest.Create("https://proxyscrape.com/api/?request=lastupdated&proxytype=socks5");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    PSSOCKS5 = reader.ReadToEnd();
                }

            }
            catch
            {
                PSHTTP = "5 minutes ago";
                PSSOCKS4 = "10 minutes ago";
                PSSOCKS5 = "12 minutes ago";

            }

        }

        private void loadProxiesButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Proxy files | *.txt";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();
            locationTextbox.Text = ofd.FileName;
        }
        
        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (Caller.GetType() == typeof(ProxyManager))
            {
                ((ProxyManager)Caller).AddProxies(locationTextbox.Text,
               (ProxyType)Enum.Parse(typeof(ProxyType), proxyTypeCombobox.Text),
               proxiesBox.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList());
            }
            ((MainDialog)Parent).Close();
        }

        private void HTTPButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                string html = "0";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://proxyscrape.com/api/?request=displayproxies&proxytype=http");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                proxiesBox.Text = html;
                proxyTypeCombobox.SelectedIndex = 0;
            }
            catch
            {
                Globals.LogError(Components.ConfigManager, "Could not scrape proxies from the host");
            }
        }

        private void SOCKS4Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string html = "0";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://proxyscrape.com/api/?request=displayproxies&proxytype=socks4");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                proxiesBox.Text = html;
                proxyTypeCombobox.SelectedIndex = 1;
            }
            catch
            {
                Globals.LogError(Components.ConfigManager, "Could not scrape proxies from the host");
            }
        }

        private void SOCKS5Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string html = "0";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://proxyscrape.com/api/?request=displayproxies&proxytype=socks5");
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                proxiesBox.Text = html;
                proxyTypeCombobox.SelectedIndex = 3;
            }
            catch
            {
                Globals.LogError(Components.ConfigManager, "Could not scrape proxies from the host");
            }
        }

        private void ScrapeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(APIUrl.Text))
                    return;

                string html = "0";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrl.Text);
                request.Proxy = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                proxiesBox.Text = html;
            }
            catch
            {
                Globals.LogError(Components.ConfigManager, "Could not scrape proxies from the host");
            }

        }
    }
}
