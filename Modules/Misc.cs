using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordTutorialBot.Core;
using DiscordTutorialBot.Core.UserAccounts;

namespace DiscordTutorialBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(Context.User.Username + " said");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);

            Random random = new Random();
            string pick = options[random.Next(0, options.Length)];

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(Context.User.Username+", RNG has chosen");
            embed.WithDescription(pick);
            embed.WithColor(new Color(0, 100, 255));
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("help")]
        public async Task Help([Remainder]string arg = "")
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("Sorry... you're on your own for now");
        }
    }
}
