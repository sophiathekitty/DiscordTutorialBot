using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTutorialBot.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong Id { get; set; }
        public string Mention;
        public Dictionary<string, string> Profiles = new Dictionary<string, string>();
        public Dictionary<string, string> Clans = new Dictionary<string, string>();
        public string ErrorMessage;

        public bool AddProfile(string platform, string url)
        {
            if (Profiles.ContainsKey(platform))
                Profiles[platform] = url;
            else
                Profiles.Add(platform, url);
            UserAccounts.SaveAccounts();
            return true;
        }

        public bool RemoveProfile(string platform)
        {
            if (Profiles.ContainsKey(platform))
                Profiles.Remove(platform);
            else
            {
                ErrorMessage = $"doesn't have {platform} profile";
                return false;
            }
            UserAccounts.SaveAccounts();
            return true;
        }

        public bool AddClan(string platform, string url)
        {
            if (Clans.ContainsKey(platform))
                Clans[platform] = url;
            else
                Clans.Add(platform, url);
            UserAccounts.SaveAccounts();
            return true;
        }
        public bool RemoveClan(string platform)
        {
            if (Clans.ContainsKey(platform))
                Clans.Remove(platform);
            else
            {
                ErrorMessage = $"doesn't have {platform} profile";
                return false;
            }
            UserAccounts.SaveAccounts();
            return true;
        }
    }
}
