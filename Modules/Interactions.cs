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
    public class Interactions : ModuleBase<SocketCommandContext>
    {
        private static Dictionary<string, List<string>> gifs;
        private static string GetRandomGifURL(string key)
        {
            Random random = new Random();
            if (gifs == null)
                gifs = DataStorage.LoadGifsData();
            if (!gifs.ContainsKey(key))
            {
                if(gifs.ContainsKey("404"))
                    return gifs["404"][random.Next(0, gifs["404"].Count)];
                return "https://media.giphy.com/media/l1J9EdzfOSgfyueLm/giphy.gif";
            }
            return gifs[key][random.Next(0, gifs[key].Count)];
        }

        private Embed SendGifAction(string key, string action)
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            EmbedBuilder embed = new EmbedBuilder();
            if(mentionedUser != null)
            {
                embed.WithDescription($"{Context.User.Mention} {action} {mentionedUser.Mention}");
                embed.WithImageUrl(GetRandomGifURL(key));
            }
            else
            {
                UserAccount account = UserAccounts.GetAccount(Context.User);
                embed.WithDescription($"{Context.User.Mention} {action} {account.themself}");
                embed.WithImageUrl(GetRandomGifURL(key+""));
            }

            embed.WithColor(new Color(0, 100, 255));
            return embed.Build();
        }

        [Command("reloadGifs")]
        public async Task ReloadGifs([Remainder]string arg = "")
        {
            gifs = DataStorage.LoadGifsData();
            await Context.Channel.SendMessageAsync("gifs.json reloaded");
        }
        [Command("poke")]
        [Alias("pokes")]
        public async Task Poke([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("poke", "pokes"));
        }

        [Command("hug")]
        [Alias("hugs")]
        public async Task Hug([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("hug", "hugs"));
        }

        [Command("cuddle")]
        [Alias("cuddles")]
        public async Task Cuddle([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("cuddle", "cuddles"));
        }

        [Command("dance")]
        [Alias("dances")]
        public async Task Dance([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("dance", "dances with"));
        }

        [Command("cry")]
        [Alias("cries")]
        public async Task Cry([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("cry", "cries over"));
        }

        [Command("laugh")]
        [Alias("laughs")]
        public async Task Laugh([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("laugh", "laughs at"));
        }

        [Command("blush")]
        [Alias("blushes")]
        public async Task Blush([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("blush", "blushes at"));
        }

        [Command("highfive")]
        [Alias("highfives")]
        public async Task Highfive([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("highfive", "high fives"));
        }

        [Command("kiss")]
        [Alias("kisses")]
        public async Task Kiss([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("kiss", "kisses"));
        }

        [Command("nom")]
        [Alias("noms")]
        public async Task Nom([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("nom", "noms"));
        }


        [Command("pat")]
        [Alias("pats")]
        public async Task Pat([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("pat", "pats"));
        }

        [Command("slap")]
        [Alias("slaps")]
        public async Task Slap([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("slap", "slaps"));
        }

        [Command("fistbump")]
        [Alias("fistbumps")]
        public async Task Fistbump([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("fistbump", "fist bumps"));
        }

        [Command("tickle")]
        [Alias("tickles")]
        public async Task Tickle([Remainder]string arg = "")
        {
            await Context.Channel.SendMessageAsync("", false, SendGifAction("tickle", "tickles"));
        }

        [Command("dead")]
        public async Task Dead([Remainder]string arg = "")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithDescription($"{Context.User.Mention} is dead");
            embed.WithImageUrl(GetRandomGifURL("dead"));
            embed.WithColor(new Color(0, 100, 255));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("tired")]
        public async Task Tired([Remainder]string arg = "")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithDescription($"{Context.User.Mention} is tired");
            embed.WithImageUrl(GetRandomGifURL("tired"));
            embed.WithColor(new Color(0, 100, 255));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("tabby")]
        public async Task Tabby([Remainder]string arg = "")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithImageUrl(GetRandomGifURL("tabby"));
            embed.WithColor(new Color(0, 100, 255));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
