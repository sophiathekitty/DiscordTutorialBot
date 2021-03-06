﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordTutorialBot.Core;
using DiscordTutorialBot.Core.UserAccounts;

namespace DiscordTutorialBot.Modules
{
    public class GamesManager : ModuleBase<SocketCommandContext>
    {
        private static List<GameInfo> games;
        private static Dictionary<ulong, PendingGame> pendingGames;
        private static int voteThreshold = 3;
        private static SocketGuildChannel gameChannel;
        private static SocketCategoryChannel gamesText, gamesVoice;
        private static ulong roleMessageId;

        private static readonly OverwritePermissions permissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
        private static bool ready = false;


        public GamesManager()
        {
            if (ready) return;
            Console.WriteLine("Games Manager Started");
            if (GlobalUtils.client == null)
                Console.WriteLine("Client not set yet! D:");
            else
            {
                GlobalUtils.client.ReactionAdded += OnReactionAdded;
                GlobalUtils.client.ReactionRemoved += OnReactionRemoved;
                GlobalUtils.client.Ready += OnReady;
                ready = true;
            }
        }

        private static void Save()
        {
            DataStorage.SaveGames(games);
            DataStorage.SavePendingGames(pendingGames);
            if(gameChannel != null)
                DataStorage.SetData("gameChannel", gameChannel.Id.ToString());
            if(gamesText != null)
                DataStorage.SetData("gamesText", gamesText.Id.ToString());
            if(gamesVoice != null)
                DataStorage.SetData("gamesVoice", gamesVoice.Id.ToString());
            DataStorage.SetData("gameRoleMessageId", roleMessageId.ToString());
        }
        private static void Load()
        {
            games = DataStorage.LoadGames();
            pendingGames = DataStorage.LoadPendingGames();
            if (GlobalUtils.client == null) return;

            ulong chanId = ulong.Parse(DataStorage.GetData("gameChannel"));
            gameChannel = (SocketGuildChannel)GlobalUtils.client.GetChannel(chanId);

            chanId = ulong.Parse(DataStorage.GetData("gamesText"));
            SocketGuild guild = GlobalUtils.client.Guilds.FirstOrDefault();
            var result = from a in guild.CategoryChannels
                         where a.Id == chanId
                         select a;
            gamesText = result.FirstOrDefault();

            chanId = ulong.Parse(DataStorage.GetData("gamesVoice"));
            result = from a in guild.CategoryChannels
                         where a.Id == chanId
                         select a;
            gamesVoice = result.FirstOrDefault();


            roleMessageId = ulong.Parse(DataStorage.GetData("gameRoleMessageId"));

        }
        private Task OnReady()
        {
            Load();
            return Task.CompletedTask;
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // remove vote
            if (pendingGames.ContainsKey(reaction.MessageId) && reaction.Emote.Name == "\u2705")
            {
                pendingGames[reaction.MessageId].votes--;
            }
            else if (reaction.MessageId == roleMessageId)
            {
                // they reacted to the correct role message
                IGuildChannel guildChannel = channel as IGuildChannel;
                SocketGuild guild = guildChannel.Guild as SocketGuild; //GlobalUtils.client.Guilds.FirstOrDefault();
                SocketUser user = GlobalUtils.client.GetUser(reaction.UserId);
                SocketGuildUser guser = guild.GetUser(user.Id);
                Console.WriteLine("Remove Game Role: "+guser.Nickname);
                for (int i = 0; i < games.Count && i < GlobalUtils.menu_emoji.Count<string>(); i++)
                {
                    if (GlobalUtils.menu_emoji[i] == reaction.Emote.Name)
                    {
                        Console.WriteLine("Emoji Found");
                        var result = from a in guild.Roles
                                     where a.Name == games[i].game
                                     select a;
                        SocketRole role = result.FirstOrDefault();
                        Console.WriteLine("Role: "+role.Name);
                        await guser.RemoveRoleAsync(role);
                    }
                }
            }

            Save();
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            IGuildChannel guildChannel = channel as IGuildChannel;
            SocketGuild guild = guildChannel.Guild as SocketGuild; //GlobalUtils.client.Guilds.FirstOrDefault();
            SocketUser user = GlobalUtils.client.GetUser(reaction.UserId);
            SocketGuildUser guser = guild.GetUser(user.Id);


            if (user.IsBot) return;// Task.CompletedTask;
            // add vote
            Console.WriteLine($"Emoji added: {reaction.Emote.Name}");
            if (pendingGames.ContainsKey(reaction.MessageId) && reaction.Emote.Name == "\u2705")
            {
                pendingGames[reaction.MessageId].votes++;
                if (pendingGames[reaction.MessageId].votes >= voteThreshold || user.Id == 346219993437831182)
                {
                    Console.WriteLine("Adding game: " + pendingGames[reaction.MessageId].game);
                    // add the game now!
                    RestRole gRole = await guild.CreateRoleAsync(pendingGames[reaction.MessageId].game);
                    Console.WriteLine("Game Role: " + gRole.Name);
                    
                    RestTextChannel txtChan = await guild.CreateTextChannelAsync(pendingGames[reaction.MessageId].game, x =>
                    {
                        x.CategoryId = gamesText.Id;
                    });
                    Console.WriteLine("Text Channel: " + txtChan.Name);
                    

                    await txtChan.AddPermissionOverwriteAsync(gRole,permissions);
                    RestVoiceChannel voiceChan = await guild.CreateVoiceChannelAsync(pendingGames[reaction.MessageId].game, x =>
                    {
                        x.CategoryId = gamesVoice.Id;
                    });
                    Console.WriteLine("Voice Channel: " + voiceChan.Name);
                    await voiceChan.AddPermissionOverwriteAsync(gRole, permissions);
                    games.Add(new GameInfo(pendingGames[reaction.MessageId].game,txtChan.Id,voiceChan.Id));

                    // remove poll message, add new game announcement and remove pending game
                    ISocketMessageChannel chan = gameChannel as ISocketMessageChannel;
                    await chan.DeleteMessageAsync(reaction.MessageId);
                    EmbedBuilder embed = new EmbedBuilder();
                    embed.WithTitle("New Game Added");
                    embed.WithDescription($"`{pendingGames[reaction.MessageId].game}`\n");
                    embed.WithColor(GlobalUtils.color);
                    await chan.SendMessageAsync("",false,embed.Build());
                    pendingGames.Remove(reaction.MessageId);

                    UpdateOrAddRoleMessage();
                }
            }
            Console.WriteLine($"Emoji added: {reaction.MessageId} == {roleMessageId} : {reaction.MessageId == roleMessageId}");
            Console.WriteLine("Add Game Role: " + guser.Nickname);
            if (reaction.MessageId == roleMessageId)
            {
                // they reacted to the correct role message
                Console.WriteLine("Add Game Role: " + guser.Nickname);
                for (int i = 0; i < games.Count && i < GlobalUtils.menu_emoji.Count<string>(); i++)
                {
                    if (GlobalUtils.menu_emoji[i] == reaction.Emote.Name)
                    {
                        Console.WriteLine("Emoji Found");
                        var result = from a in guild.Roles
                                     where a.Name == games[i].game
                                     select a;
                        SocketRole role = result.FirstOrDefault();
                        Console.WriteLine("Role: " + role.Name);
                        await guser.AddRoleAsync(role);
                    }
                }
            }
            Console.WriteLine("what?!");
            Save();
        }


        [Command("addGame")]
        public async Task AddGame([Remainder]string arg = "")
        {
            var result = from a in games
                         where a.game == arg
                         select a;
            GameInfo game = result.FirstOrDefault();
            // see if the game already exists
            if (game != null)
            {
                await Context.Channel.SendMessageAsync($"`{arg}` already exists");
                return;
            }
            // make sure we can post the new game poll message
            if(gameChannel == null)
            {
                await Context.Channel.SendMessageAsync("Game Announcement Channel not set. please use `setGameChannel` to set channel for posting new game polls");
                return;
            }
            // create a new game poll message
            ISocketMessageChannel chan = gameChannel as ISocketMessageChannel;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("New Game Poll");
            embed.WithDescription(arg);
            embed.WithColor(GlobalUtils.color);
            RestUserMessage poll = await chan.SendMessageAsync("", false, embed.Build());
            Emoji emoji = new Emoji("\u2705");
            await poll.AddReactionAsync(emoji);
            pendingGames.Add(poll.Id, new PendingGame(arg));
            Save();
        }

        [Command("setGameText")]
        public async Task SetTextCat([Remainder]string arg = "")
        {
            var result = from a in Context.Guild.CategoryChannels
                         where a.Name == arg
                         select a;
            SocketCategoryChannel socketCategory = result.FirstOrDefault();
            if (socketCategory == null)
            {
                await Context.Channel.SendMessageAsync($"Couldn't find `{arg}` in Guild.CategoryChannels");
                return;
            }

            gamesText = socketCategory;
            Save();
            await Context.Channel.SendMessageAsync($"Gaming text channel category set to `{arg}`");
        }
        [Command("setGameVoice")]
        public async Task SetVoiceCat([Remainder]string arg = "")
        {
            var result = from a in Context.Guild.CategoryChannels
                         where a.Name == arg
                         select a;
            SocketCategoryChannel socketCategory = result.FirstOrDefault();
            if (socketCategory == null)
            {
                await Context.Channel.SendMessageAsync($"Couldn't find `{arg}` in Guild.CategoryChannels");
                return;
            }

            gamesVoice = socketCategory;
            Save();
            await Context.Channel.SendMessageAsync($"Gaming voice channel category set to `{arg}`");
        }

        [Command("setGameChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RoleChannel([Remainder]string arg = "")
        {

            gameChannel = Context.Message.MentionedChannels.FirstOrDefault();
            if (gameChannel != null)
                await Context.Channel.SendMessageAsync($"Game Announcements Channel set to `{gameChannel.Name}`");
            else
                await Context.Channel.SendMessageAsync($"Channel `{arg}` not found");
            Save();
        }
        [Command("gameMessageTest")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GameMessageTest([Remainder]string arg = "")
        {
            string[] gameInfos = arg.Split(',');
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(gameInfos[0]);
            if (gameInfos.Length > 1)
                embed.WithThumbnailUrl(gameInfos[1]);
            if (gameInfos.Length > 2)
                embed.WithDescription(gameInfos[2]);
            if (gameInfos.Length > 3)
                embed.WithUrl(gameInfos[3]);

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder();
            fieldBuilder.WithName("Test Field");
            fieldBuilder.WithValue(123456);

            embed.WithFields(fieldBuilder);
            await Context.Channel.SendMessageAsync("",false,embed.Build());
        }




        private async void UpdateOrAddRoleMessage()
        {
            if (RoleManager.roleChannel == null) return;
            if (games.Count == 0) return;

            ISocketMessageChannel chan = RoleManager.roleChannel as ISocketMessageChannel;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Game Roles");

            string roles_txt = "";
            List<IEmote> emotes = new List<IEmote>();
            for (int i = 0; i < games.Count && i < GlobalUtils.menu_emoji.Count<string>(); i++)
            {
                Emoji emoji = new Emoji(GlobalUtils.menu_emoji[i]);
                emotes.Add(emoji);
                roles_txt += $"{GlobalUtils.menu_emoji[i]} {games[i].game}\n";
            }

            embed.WithDescription(roles_txt);
            embed.WithColor(GlobalUtils.color);

            if (roleMessageId == 0)
            {
                RestUserMessage msg = await chan.SendMessageAsync("", false, embed.Build());
                roleMessageId = msg.Id;
                await msg.AddReactionsAsync(emotes.ToArray());
            }
            else
            {

                if (!(await chan.GetMessageAsync(roleMessageId) is RestUserMessage msg))
                {
                    RestUserMessage msg2 = await chan.SendMessageAsync("", false, embed.Build());
                    roleMessageId = msg2.Id;
                    await msg2.AddReactionsAsync(emotes.ToArray());
                }
                else
                {
                    await msg.ModifyAsync(x =>
                    {
                        x.Embed = embed.Build();
                    });
                    await msg.RemoveAllReactionsAsync();
                    await msg.AddReactionsAsync(emotes.ToArray());
                }
            }
            Save();
        }






        public class PendingGame
        {
            public PendingGame(string g)
            {
                game = g;
            }
            public int votes;
            public string game;
        }
        public class GameInfo
        {
            public string game;
            public ulong text;
            public ulong voice;
            public string description;
            public string thumbUrl;
            public string gameUrl;
            public List<GameServerInfo> servers;
            public GameInfo(string g, ulong t,ulong v)
            {
                game = g; text = t; voice = v;
            }
        }
        public class GameServerInfo
        {
            public string url;
            public string password;
        }
    }
}
