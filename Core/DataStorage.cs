using DiscordTutorialBot.Core.UserAccounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiscordTutorialBot.Modules.GamesManager;

namespace DiscordTutorialBot.Core
{
    class DataStorage
    {
        private const string dataFolder = "Resources";
        private const string dataFile = "data.json";
        private const string usersFile = "accounts.json";
        private const string gamesFile = "games.json";
        private const string pendingGamesFile = "pendingGames.json";
        private static string DataPath { get { return dataFolder + "/" + dataFile; } }
        private static string UsersPath { get { return dataFolder + "/" + usersFile; } }
        private static string GamesPath { get { return dataFolder + "/" + gamesFile; } }
        private static string PendingGamesPath { get { return dataFolder + "/" + pendingGamesFile; } }

        private static Dictionary<string, string> data = new Dictionary<string, string>();
        public static string GetData(string key)
        {
            if (data.ContainsKey(key))
                return data[key];
            return "";
        }
        public static void SetData(string key, string value)
        {
            //Console.WriteLine($"DataStorage.SetData({key},{value});");
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

            string json = File.ReadAllText(DataPath);
            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        public static void SaveData()
        {
            // save data
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(DataPath, json);
        }

        private static bool ValidateStorageFile()
        {
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);
            if (!File.Exists(DataPath))
            {
                File.WriteAllText(DataPath, "");
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
            if (!File.Exists(UsersPath))
                return false;
            return true;
        }


        // save all useraccounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts)
        {
            string json = JsonConvert.SerializeObject(accounts,Formatting.Indented);
            File.WriteAllText(UsersPath, json);
        }
        public static List<UserAccount> LoadUserAccounts()
        {
            if (!File.Exists(UsersPath)) return null;
            string json = File.ReadAllText(UsersPath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }


        // load gifs data
        public static Dictionary<string,List<string>> LoadGifsData()
        {
            string json = File.ReadAllText("SystemLang/gifs.json");
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        }


        //
        // pending games
        public static Dictionary<ulong, PendingGame> LoadPendingGames()
        {
            if (!File.Exists(PendingGamesPath)) return new Dictionary<ulong, PendingGame>();
            string json = File.ReadAllText(PendingGamesPath);
            return JsonConvert.DeserializeObject<Dictionary<ulong, PendingGame>>(json);
        }
        public static void SavePendingGames(Dictionary<ulong, PendingGame> games)
        {
            string json = JsonConvert.SerializeObject(games, Formatting.Indented);
            File.WriteAllText(PendingGamesPath, json);
        }
        //
        // games
        public static List<string> LoadGames()
        {
            if (!File.Exists(GamesPath)) return new List<string>();
            string json = File.ReadAllText(GamesPath);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }
        public static void SaveGames(List<string> games)
        {
            string json = JsonConvert.SerializeObject(games, Formatting.Indented);
            File.WriteAllText(GamesPath, json);
        }
    }
}
