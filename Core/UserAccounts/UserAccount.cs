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
        public int XP;
        public Dictionary<string, string> Profiles = new Dictionary<string, string>();
        public Dictionary<string, string> Clans = new Dictionary<string, string>();
    }
}
