using DiscordTutorialBot.Core.UserAccounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTutorialBot.Core
{
    class DataStorage
    {
        private const string dataFolder = "Resources";
        private const string dataFile = "data.json";
        private const string usersFile = "accounts.json";
        private static string dataPath { get { return dataFolder + "/" + dataFile; } }
        private static string usersPath { get { return dataFolder + "/" + usersFile; } }

        private static Dictionary<string, string> data = new Dictionary<string, string>();
        public static string GetData(string key)
        {
            if (data.ContainsKey(key))
                return data[key];
            return "";
        }
        public static void SetData(string key, string value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
            SaveData();
        }

        static DataStorage()
        {
            // load data
            if (!ValidateStorageFile()) return;

            string json = File.ReadAllText(dataPath);
            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        public static void SaveData()
        {
            // save data
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(dataPath, json);
        }

        private static bool ValidateStorageFile()
        {
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);
            if (!File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, "");
                SaveData();
                return false;
            }
            return true;
        }

        // validate accounts file...
        public static bool ValidateAccountsFile()
        {
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);
            if (!File.Exists(usersPath))
                return false;
            return true;
        }


        // save all useraccounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts)
        {
            string json = JsonConvert.SerializeObject(accounts,Formatting.Indented);
            File.WriteAllText(usersPath, json);
        }
        public static List<UserAccount> LoadUserAccounts()
        {
            if (!File.Exists(usersPath)) return null;
            string json = File.ReadAllText(usersPath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }


        // load gifs data
        public static Dictionary<string,List<string>> LoadGifsData()
        {
            string json = File.ReadAllText("SystemLang/gifs.json");
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        }
    }
}
