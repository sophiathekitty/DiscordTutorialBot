using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordTutorialBot.Core;

namespace DiscordTutorialBot
{
    class Program
    {
        DiscordSocketClient client;
        CommandHandler handler;

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });
            GlobalUtils.client = client;
            client.Log += Log;

            await client.LoginAsync(Discord.TokenType.Bot, Config.bot.token);
            await client.StartAsync();

            handler = new CommandHandler();
            await handler.InitializeAsync(client);

            await Task.Delay(-1);
        }

        private async Task Log(Discord.LogMessage arg)
        {
            Console.WriteLine(arg.Message);
        }
    }
}
