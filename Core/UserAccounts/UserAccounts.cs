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
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.Id == id
                         select a;
            UserAccount account = result.FirstOrDefault();
            if(account == null)
            {
                account = CreateUserAccount(id);
                accounts.Add(account);
                SaveAccounts();
            }
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            UserAccount newAccount = new UserAccount()
            {
                Id = id
            };
            return newAccount;
        }
    }
}
