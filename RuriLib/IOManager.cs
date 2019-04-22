using Newtonsoft.Json;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Reflection;
using System.Management;

namespace RuriLib
{
    /// <summary>
    /// Static Class used to access serialization and deserialization of objects.
    /// </summary>
    public static class IOManager
    {
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        private static Random randomY = new Random();
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private static string RandomString(int length)
        {
            const string chars = ".;ù$£Ù>R9œ¾4Äš:bþƒ@~Ñ`WŒ°YßË˜©ý”ñ]ÃúÞABCNOPQRUVWYZabcdefgnopqrstuvwzy123456789=/";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[randomY.Next(s.Length)]).ToArray());
        }
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private static string EncryptX(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

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

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private static string DecryptX(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        /// <summary>
        /// Saves the RuriLib settings to a file.
        /// </summary>
        /// <param name="settingsFile">The file you want to save to</param>
        /// <param name="settings">The RuriLib settings object</param>
        public static void SaveSettings(string settingsFile, RLSettingsViewModel settings)
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        /// <summary>
        /// Loads the RuriLib settings from a file.
        /// </summary>
        /// <param name="settingsFile">The file you want to load from</param>
        /// <returns>An instance of RLSettingsViewModel</returns>
        public static RLSettingsViewModel LoadSettings(string settingsFile)
        {
            return JsonConvert.DeserializeObject<RLSettingsViewModel>(File.ReadAllText(settingsFile));
        }

        /// <summary>
        /// Serializes a block to a JSON string.
        /// </summary>
        /// <param name="block">The block to serialize</param>
        /// <returns>The JSON-encoded BlockBase object with TypeNameHandling on</returns>
        public static string SerializeBlock(BlockBase block)
        {
            return JsonConvert.SerializeObject(block, Formatting.None, settings);
        }

        /// <summary>
        /// Deserializes a Block from a JSON string.
        /// </summary>
        /// <param name="block">The JSON-encoded string with TypeNameHandling on</param>
        /// <returns>An instance of BlockBase</returns>
        public static BlockBase DeserializeBlock(string block)
        {
            return JsonConvert.DeserializeObject<BlockBase>(block, settings);
        }

        /// <summary>
        /// Serializes a list of blocks to a JSON string.
        /// </summary>
        /// <param name="blocks">The list of blocks to serialize</param>
        /// <returns>The JSON-encoded List of BlockBase objects with TypeNameHandling on</returns>
        public static string SerializeBlocks(List<BlockBase> blocks)
        {
            return JsonConvert.SerializeObject(blocks, Formatting.None, settings);
        }

        /// <summary>
        /// Deserializes a list of blocks from a JSON string.
        /// </summary>
        /// <param name="blocks">The JSON-encoded string with TypeNameHandling on</param>
        /// <returns>A list of instances of BlockBase</returns>
        public static List<BlockBase> DeserializeBlocks(string blocks)
        {
            return JsonConvert.DeserializeObject<List<BlockBase>>(blocks, settings);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static string BellaCiao(string helpme, int op)
        {
            if (op == 1)
            {
                // cifrado de la mderda
                string ToReturn = "";
                string _key = "ay$a5%&jwrtmnh;lasjdf98787OMGFORLAX";
                string _iv = "abc@98797hjkas$&asd(*$%GJMANIGE";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(helpme);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());

                    var sb = new StringBuilder();
                    var bytes = Encoding.UTF8.GetBytes(ToReturn);
                    foreach (var t in bytes)
                    {
                        sb.Append(t.ToString("X2"));
                    }
                    ToReturn = sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"

                }
                return ToReturn;
            }
            else
            {
                string ToReturn = "";
                string _key = "ay$a5%&jwrtmnh;lasjdf98787OMGFORLAX";
                string _iv = "abc@98797hjkas$&asd(*$%GJMANIGE";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;

                var bytes = new byte[helpme.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(helpme.Substring(i * 2, 2), 16);
                }
                helpme = Encoding.UTF8.GetString(bytes);
                byte[] inputbyteArray = new byte[helpme.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(helpme.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
        }

        
        /// <summary>
        /// Serializes a Config object to the loli-formatted string.
        /// </summary>
        /// <param name="config">The Config to serialize</param>
        /// <returns>The loli-formatted string</returns>
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private static string SerializeConfigX(Config config)
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("[SETTINGS]");
            writer.WriteLine(JsonConvert.SerializeObject(config.Settings, Formatting.Indented));
            writer.WriteLine("");
            writer.WriteLine("[SCRIPT]");
            writer.Write(config.Script);
            string thing = writer.ToString();
            string thing2 = EncryptX(thing, "0THISISOBmodedByForlaxNIGGAs");
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(RandomString(randomY.Next(32, 60)) + "x0;" + thing2 + "0;x" + RandomString(randomY.Next(32, 60)));
            string plainText = Convert.ToBase64String(byt);
            return "{ \n    \"ID\": \"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(getLoliKey())) + "\"\n    \"Body\": \"" + BellaCiao(plainText, 1) + "\" \n}";
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private static string SerializeConfigBBC(Config config)
        {
            return JsonConvert.SerializeObject(config, Formatting.Indented, settings);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static Config DeserializeConfigBBC(string config)
        {
            return JsonConvert.DeserializeObject<Config>(config, settings);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static string SerializeConfig(Config config)
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("[SETTINGS]");
            writer.WriteLine(JsonConvert.SerializeObject(config.Settings, Formatting.Indented));
            writer.WriteLine("");
            writer.WriteLine("[SCRIPT]");
            writer.Write(config.Script);
            return writer.ToString();

        }

        /// <summary>
        /// Deserializes a Config object from a loli-formatted string.
        /// </summary>
        /// <param name="config">The loli-formatted string</param>
        /// <returns>An instance of the Config object</returns>
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static Config DeserializeConfig(string config)
        {
            var split = config.Split(new string[] { "[SETTINGS]", "[SCRIPT]" }, StringSplitOptions.RemoveEmptyEntries);
            return new Config(JsonConvert.DeserializeObject<ConfigSettings>(split[0]), split[1].TrimStart('\r', '\n'));
        }
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static Config DeserializeConfigX(string config)
        {
            if (config.Contains("Body") && config.Contains("ID"))
            {
                string config23 = Regex.Match(config, "\"Body\": \"(.*?)\"").Groups[1].Value;
                string omg = BellaCiao(config23, 2);
                byte[] b = Convert.FromBase64String(omg);
                config = System.Text.Encoding.UTF8.GetString(b);
                string config2 = Regex.Match(config, "x0;(.*?)0;x").Groups[1].Value;
                config = DecryptX(config2, "0THISISOBmodedByForlaxNIGGAs");
                var split = config.Split(new string[] { "[SETTINGS]", "[SCRIPT]" }, StringSplitOptions.RemoveEmptyEntries);
                return new Config(JsonConvert.DeserializeObject<ConfigSettings>(split[0]), split[1].TrimStart('\r', '\n'));
            }
            else
            {
                byte[] b = Convert.FromBase64String(config);
                config = System.Text.Encoding.UTF8.GetString(b);
                string config2 = Regex.Match(config, "x0;(.*?)0;x").Groups[1].Value;
                config = DecryptX(config2, "0THISISOBmodedByForlaxNIGGAs");
                var split = config.Split(new string[] { "[SETTINGS]", "[SCRIPT]" }, StringSplitOptions.RemoveEmptyEntries);
                return new Config(JsonConvert.DeserializeObject<ConfigSettings>(split[0]), split[1].TrimStart('\r', '\n'));
            }
        }

        /// <summary>
        /// Serializes a list of proxies to a JSON string.
        /// </summary>
        /// <param name="proxies">The list of proxies to serialize</param>
        /// <returns>The JSON-encoded list of CProxy objects with TypeNameHandling on</returns>
        public static string SerializeProxies(List<CProxy> proxies)
        {
            return JsonConvert.SerializeObject(proxies, Formatting.None);
        }

        /// <summary>
        /// Deserializes a list of proxies from a JSON string.
        /// </summary>
        /// <param name="proxies">The JSON-encoded list of proxies with TypeNameHandling on</param>
        /// <returns>A list of CProxy objects</returns>
        public static List<CProxy> DeserializeProxies(string proxies)
        {
            return JsonConvert.DeserializeObject<List<CProxy>>(proxies);
        }

        /// <summary>
        /// Loads a Config object from a .loli file.
        /// </summary>
        /// <param name="fileName">The config file</param>
        /// <returns>A Config object</returns>
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static Config LoadConfig(string fileName)
        {
            return DeserializeConfig(File.ReadAllText(fileName));
        }
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static Config LoadConfigX(string fileName)
        {
            return DeserializeConfigX(File.ReadAllText(fileName));
        }

        /// <summary>
        /// Saves a Config object to a .loli file.
        /// </summary>
        /// <param name="config">The viewmodel of the config to save</param>
        /// <param name="fileName">The path of the file where the Config will be saved</param>
        /// <returns>Whether the file has been saved successfully</returns>
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static bool SaveConfig(Config config, string fileName)
        {
            try
            {
                File.WriteAllText(fileName, SerializeConfig(config));
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Saves a Config object to a .lolix file.
        /// </summary>
        /// <param name="config">The viewmodel of the config to save</param>
        /// <param name="fileName">The path of the file where the Config will be saved</param>
        /// <returns>Whether the file has been saved successfully</returns>
        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static bool SaveConfigX(Config config, string fileName)
        {
            string backup = config.Settings.Author;
            if (!config.Settings.Author.Contains("(loliX Encrypted)"))
            {
                config.Settings.Author = config.Settings.Author + " (loliX Encrypted)";
            }
            try
            {
                File.WriteAllText(fileName, SerializeConfigX(config));
                config.Settings.Author = backup;
                return true;
            }
            catch
            {
                config.Settings.Author = backup;
                return false;
            }
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public static bool SaveConfigBBC(Config config, string fileName)
        {
            try
            {
                File.WriteAllText(fileName, SerializeConfigBBC(config));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clones a Config object by serializing and deserializing it.
        /// </summary>
        /// <param name="config">The object to clone</param>
        /// <returns>The cloned Config object</returns>
        public static Config CloneConfig(Config config)
        {
            return DeserializeConfig(SerializeConfig(config));
        }

        /// <summary>
        /// Clones a BlockBase object by serializing and deserializing it.
        /// </summary>
        /// <param name="block">The object to clone</param>
        /// <returns>The cloned BlockBase object</returns>
        public static BlockBase CloneBlock(BlockBase block)
        {
            return DeserializeBlock(SerializeBlock(block));
        }

        /// <summary>
        /// Clones a list of proxies by serializing and deserializing it.
        /// </summary>
        /// <param name="proxies">The list of proxies to clone</param>
        /// <returns>The cloned list of proxies</returns>
        public static List<CProxy> CloneProxies(List<CProxy> proxies)
        {
            return DeserializeProxies(SerializeProxies(proxies));
        }

        /// <summary>
        /// Parses the EnvironmentSettings from a file.
        /// </summary>
        /// <param name="envFile">The .ini file of the settings</param>
        /// <returns>The loaded EnvironmentSettings object</returns>
        public static EnvironmentSettings ParseEnvironmentSettings(string envFile)
        {
            var env = new EnvironmentSettings();
            var lines = File.ReadAllLines(envFile).Where(l => !string.IsNullOrEmpty(l)).ToArray();
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i];
                if (line.StartsWith("#")) continue;
                else if (line.StartsWith("["))
                {
                    Type type;
                    var header = line;
                    switch (line.Trim())
                    {
                        case "[WLTYPE]": type = typeof(WordlistType); break;
                        case "[CUSTOMKC]": type = typeof(CustomKeychain); break;
                        case "[EXPFORMAT]": type = typeof(ExportFormat); break;
                        default: throw new Exception("Unrecognized ini header");
                    }

                    var parameters = new List<string>();
                    int j = i + 1;
                    for (; j < lines.Count(); j++)
                    {
                        line = lines[j];
                        if (line.StartsWith("[")) break;
                        else if (line.Trim() == "" || line.StartsWith("#")) continue;
                        else parameters.Add(line);
                    }

                    switch (header)
                    {
                        case "[WLTYPE]": env.WordlistTypes.Add((WordlistType)ParseObjectFromIni(type, parameters)); break;
                        case "[CUSTOMKC]": env.CustomKeychains.Add((CustomKeychain)ParseObjectFromIni(type, parameters)); break;
                        case "[EXPFORMAT]": env.ExportFormats.Add((ExportFormat)ParseObjectFromIni(type, parameters)); break;
                        default: break;
                    }

                    i = j - 1;
                }
            }

            return env;
        }

        private static object ParseObjectFromIni(Type type, List<string> parameters)
        {
            object obj = Activator.CreateInstance(type);
            foreach (var pair in parameters
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Split(new char[] { '=' }, 2)))
            {
                var prop = type.GetProperty(pair[0]);
                var propObj = prop.GetValue(obj);
                dynamic value = null;
                var ts = new TypeSwitch()
                    .Case((String x) => value = pair[1])
                    .Case((Int32 x) => value = Int32.Parse(pair[1]))
                    .Case((Boolean x) => value = Boolean.Parse(pair[1]))
                    .Case((List<String> x) => value = pair[1].Split(',').ToList())
                    .Case((Color x) => value = Color.FromRgb(
                        System.Drawing.Color.FromName(pair[1]).R,
                        System.Drawing.Color.FromName(pair[1]).G,
                        System.Drawing.Color.FromName(pair[1]).B
                    ))
                ;

                ts.Switch(propObj);
                prop.SetValue(obj, value);
            }
            return obj;
        }
    }
}
