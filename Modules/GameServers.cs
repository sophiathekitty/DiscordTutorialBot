using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordTutorialBot.Core.UserAccounts;

namespace DiscordTutorialBot.Modules
{
    class GameServers : ModuleBase<SocketCommandContext>
    {
        [Command("addGameServer")]
        [Alias("addServer")]
        public async Task AddGameServer(string platform, [Remainder]string args)
        {
            string[] arguments = args.Split('|');
            Console.WriteLine(args);
            await Context.Channel.SendMessageAsync("Failed to set profile.");
        }

        public class GameServerInfo
        {
            public string game;
            public string url;
            public string password;
        }
    }
}
