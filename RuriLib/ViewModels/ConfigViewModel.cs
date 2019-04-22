using Newtonsoft.Json;
using System;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Reflection;

namespace RuriLib.ViewModels
{
    /// <summary>
    /// An observable wrapper around a Config object.
    /// </summary>
    public class ConfigViewModel : ViewModelBase
    {
        /// <summary>The actual Config object.</summary>
        public Config Config { get; set; }
        
        private string category = "Default";
        /// <summary>The Category of the config.</summary>
        public string Category { get { return category; } set { category = value; OnPropertyChanged(); } }

        private string path = "";
        /// <summary>The path of the config file on disk.</summary>
        public string Path { get { return path; } set { path = value; OnPropertyChanged(); } }

        /// <summary>The name of the config.</summary>
        public string Name { get { return Config.Settings.Name; } }

        /// <summary>
        /// Constructs an instance of the ConfigViewModel class.
        /// </summary>
        /// <param name="path">The path of the config file on disk</param>
        /// <param name="category">The category of the config</param>
        /// <param name="config">The actual Config object</param>
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static string getLoliKey()
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
            return str2;
        }

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

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void option1(string path, string category, Config config, int option)
        {
            if (option == 1)
            {
                Path = path;
                Category = category;
                Config = config;
                return;
            }
            else if (option == 2)
            {

               
                Path = path;
                Category = "loliX";
                Config = config;
                string Namiz = config.Settings.Name;
                bool flag = false;
                string userLoliKeyHere = getLoliKey();
                hola = userLoliKeyHere;
                if (!string.IsNullOrEmpty(config.Settings.KeysDB))
                {
                    string html = "0";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(config.Settings.KeysDB);
                    request.Proxy = null;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                    if (html.Contains("{" + Namiz + "}") && html.Contains("{!" + Namiz + "}"))
                    {
                        string value = Regex.Match(html, "{" + Namiz + "}" + "(.*?)" + "{!" + Namiz + "}").Groups[0].Value;
                       
                        if (!value.Contains(userLoliKeyHere) && value.Contains("OB-"))
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        if (!html.Contains(userLoliKeyHere) && html.Contains("OB-"))
                        {
                            flag = true;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(config.Settings.LocalKeysDB) && config.Settings.LocalKeysDB.Contains("OB-"))
                {
                    if (!config.Settings.LocalKeysDB.Contains(userLoliKeyHere))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    if (!isWarned)
                    {
                        Thread t = new Thread(() => MessageBox.Show($"You can't use some configs because you're not whitelisted.", "OpenBullet License"));
                        t.SetApartmentState(ApartmentState.STA);
                        t.Start();
                        isWarned = true;
                    }
                    throw new Exception($"You can't use the config '{config.Settings.Name}' because you're not whitelisted.");
                    //throw new LicenseshitException(string.Format("You can't use this config"));
                }
            }
        }

        public ConfigViewModel(string path, string category, Config config, int option)
        {
            if (config.Settings.AllowedWordlist1 == null)
            {
                config.Settings.AllowedWordlist1 = "MailPass";
            }
            option1(path, category, config, option);
        }
        public static bool isWarned = false;
        public static string hola = "0";
        public class LicenseshitException : Exception
        {
            public LicenseshitException(string message)
               : base(message)
            {
            }
        }

    }
}
