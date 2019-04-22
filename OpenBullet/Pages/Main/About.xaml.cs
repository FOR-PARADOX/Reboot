using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Management;
using System.Text;
using System;
using System.Security.Cryptography;
using System.Reflection;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per About.xaml
    /// </summary>
    public partial class About : Page
    {
        public static string myshit = getLoliKey();
        public About()
        {
            InitializeComponent();
            LoliKey.Text = getLoliKey();
        }
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private static string getLoliKey()
        {
            string temp2 = null;
            if (string.IsNullOrEmpty(temp2))
            {
                foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
                {
                    if (driveInfo.IsReady)
                    {
                        temp2 = driveInfo.RootDirectory.ToString();
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(temp2) && temp2.EndsWith(":\\"))

                temp2 = temp2.Substring(0, temp2.Length - 2);

            string str;
            using (ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + temp2 + ":\""))
            {
                managementObject.Get();
                str = managementObject["VolumeSerialNumber"].ToString();
            }
            using (MD5 md5Hash = MD5.Create())
            {
                str = GetMd5Hash(md5Hash, str);
            }
            string str2 = "OB-" + str + "-LoliKEY";
            myshit = str2;
            return str2;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.

        private void repoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Process.Start("https://github.com/FOR-PARADOX/OB");
        }

        private void docuButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://i.imgur.com/nd3Jzkz.png");
        }

        private void discordButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Process.Start("https://discord.gg/eYHDs6z");
        }
    }
}
