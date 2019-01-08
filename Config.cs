using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DiscordTutorialBot
{
    class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        private static string configPath
        {
            get
            {
                return configFolder + "/" + configFile;
            }
        }

        public static BotConfig bot;

        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);
            if (!File.Exists(configPath))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot,Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            else
            {
                string json = File.ReadAllText(configPath);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string token;
        public string cmdPrefix;
    }
}
