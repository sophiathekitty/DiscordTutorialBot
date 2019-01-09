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
    public class GamerProfiles : ModuleBase<SocketCommandContext>
    {
        [Command("addGamerProfile")][Alias("addProfile")]
        public async Task AddGamerProfile(string platform, [Remainder]string url)
        {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            if(account.AddProfile(platform, url))
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}'s {platform} profile set to {url}");
            else
                await Context.Channel.SendMessageAsync("Failed to set profile.");
        }
        [Command("removeGamerProfile")][Alias("removeProfile")]
        public async Task RemoveGamerProfile(string platform)
        {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            if (account.RemoveProfile(platform))
                await Context.Channel.SendMessageAsync($"Removed {Context.User.Mention}'s {platform} profile.");
            else
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} {account.ErrorMessage}");
        }
        [Command("viewGamerProfiles")]
        [Alias("viewProfiles", "showProfiles", "showGamerProfiles", "gamerProfiles", "viewGamerProfile", "viewProfile", "showProfile", "showGamerProfile", "gamerProfile")]
        public async Task ViewGamerProfiles(string platform)
        {
            // find all the users with this platform
            List<UserAccount> accounts = UserAccounts.GetAccountsWithProfile(platform);
            string message = "";
            foreach (UserAccount account in accounts)
            {
                message += $"{account.Mention}: {account.Profiles[platform]}\n";
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(platform + " profiles");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("viewGamerProfiles")]
        [Alias("viewProfiles", "showProfiles", "showGamerProfiles", "gamerProfiles", "viewGamerProfile", "viewProfile", "showProfile", "showGamerProfile", "gamerProfile")]
        public async Task ViewGamerProfiles(IGuildUser user)
        {
            UserAccount account = UserAccounts.GetAccount(user as SocketUser);

            // find all the users with this platform
            string message = "";
            foreach (var profile in account.Profiles)
            {
                message += $"{profile.Key}: {profile.Value}\n";
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(user.Username + "'s profiles");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("viewGamerProfiles")]
        [Alias("viewProfiles", "showProfiles", "showGamerProfiles", "gamerProfiles", "viewGamerProfile", "viewProfile", "showProfile", "showGamerProfile", "gamerProfile")]
        public async Task ViewGamerProfiles()
        {
            List<UserAccount> accounts = UserAccounts.GetAccountsWithProfiles();
            // find all the users with this platform
            string message = "";
            foreach (UserAccount account in accounts)
            {
                message += $"{account.Mention}:\n";
                foreach (var profile in account.Profiles)
                {
                    message += $"{profile.Key}: {profile.Value}\n";
                }
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Gamer profiles");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 100, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
