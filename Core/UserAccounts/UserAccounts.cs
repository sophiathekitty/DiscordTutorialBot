using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTutorialBot.Core.UserAccounts
{
    public static class UserAccounts
    {
        private static List<UserAccount> accounts;

        static UserAccounts()
        {
            if(DataStorage.ValidateAccountsFile())
               accounts = DataStorage.LoadUserAccounts();
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id,user.Mention);
        }




        public static List<UserAccount> GetAccountsWithProfile(string platform)
        {
            var result = from a in accounts
                         where a.Profiles.ContainsKey(platform)
                         select a;
            return result.ToList<UserAccount>();

        }
        public static List<UserAccount> GetAccountsWithProfiles()
        {
            var result = from a in accounts
                         where a.Profiles.Count > 0
                         select a;
            return result.ToList<UserAccount>();

        }




        public static List<UserAccount> GetAccountsWithClan(string platform)
        {
            var result = from a in accounts
                         where a.Clans.ContainsKey(platform)
                         select a;
            return result.ToList<UserAccount>();

        }
        public static List<UserAccount> GetAccountsWithClans()
        {
            var result = from a in accounts
                         where a.Clans.Count > 0
                         select a;
            return result.ToList<UserAccount>();

        }






        private static UserAccount GetOrCreateAccount(ulong id, string mention)
        {
            var result = from a in accounts
                         where a.Id == id
                         select a;
            UserAccount account = result.FirstOrDefault();
            if(account == null)
            {
                account = CreateUserAccount(id,mention);
                accounts.Add(account);
                SaveAccounts();
            }
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id, string mention)
        {
            UserAccount newAccount = new UserAccount()
            {
                Id = id,
                Mention = mention
            };
            return newAccount;
        }
    }
}
