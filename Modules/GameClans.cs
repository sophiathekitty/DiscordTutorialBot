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
    public class GameClans : ModuleBase<SocketCommandContext>
    {
        [Command("addClan")]
        [Alias("addGuild")]
        public async Task AddClan(string platform, [Remainder]string url)
        {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            if (account.AddClan(platform, url))
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}'s {platform} clan set to {url}");
            else
                await Context.Channel.SendMessageAsync("Failed to set clan.");
        }
        [Command("removeClan")]
        [Alias("removeGuild")]
        public async Task RemoveGamerClan(string platform)
        {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            if (account.RemoveClan(platform))
                await Context.Channel.SendMessageAsync($"Removed {Context.User.Mention}'s {platform} clan.");
            else
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} {account.ErrorMessage}");
        }
        [Command("viewClans")]
        [Alias("viewGuilds","showClans","showGuilds", "viewClan", "viewGuild", "showClan", "showGuild")]
        public async Task ViewGamerClans(string platform)
        {
            // find all the users with this platform
            List<UserAccount> accounts = UserAccounts.GetAccountsWithClan(platform);
            string message = "";
            foreach (UserAccount account in accounts)
            {
                message += $"{account.Mention}: {account.Clans[platform]}\n";
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(platform + " clans");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("viewClans")]
        [Alias("viewGuilds", "showClans", "showGuilds", "viewClan", "viewGuild", "showClan", "showGuild")]
        public async Task ViewGamerClans(IGuildUser user)
        {
            UserAccount account = UserAccounts.GetAccount(user as SocketUser);

            // find all the users with this platform
            string message = "";
            foreach (var clan in account.Clans)
            {
                message += $"{clan.Key}: {clan.Value}\n";
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(user.Username + "'s clans");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("viewClans")]
        [Alias("viewGuilds", "showClans", "showGuilds", "viewClan", "viewGuild", "showClan", "showGuild")]
        public async Task ViewGamerClans()
        {
            List<UserAccount> accounts = UserAccounts.GetAccountsWithClans();
            // find all the users with this platform
            string message = "";
            foreach (UserAccount account in accounts)
            {
                message += $"{account.Mention}:\n";
                foreach (var clan in account.Clans)
                {
                    message += $"{clan.Key}: {clan.Value}\n";
                }
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Gamer clans");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
