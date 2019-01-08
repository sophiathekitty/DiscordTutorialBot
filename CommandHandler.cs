using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordTutorialBot
{
    class CommandHandler
    {
        DiscordSocketClient client;
        CommandService service;
        IServiceProvider serviceProvider;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            this.client = client;
            service = new CommandService();
            await service.AddModulesAsync(Assembly.GetEntryAssembly(),serviceProvider);
            client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            if (msg.Channel is SocketDMChannel) { return; }
            if (msg.Author.IsBot) { return; }

            var context = new SocketCommandContext(client, msg);
            int argPos = 0;
            if(msg.HasStringPrefix(Config.bot.cmdPrefix,ref argPos)
                || msg.HasMentionPrefix(client.CurrentUser,ref argPos))
            {
                var result = await service.ExecuteAsync(context, argPos, serviceProvider);
                if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
