using System;
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
    public class RoleManager : ModuleBase<SocketCommandContext>
    {

        public static SocketGuildChannel roleChannel;
        private static ulong roleMessageId;
        private static List<string> roles = new List<string>();

        private static string[] menu_emoji = { "\u0031\u20E3", "\u0032\u20E3", "\u0033\u20E3", "\u0034\u20E3", "\u0035\u20E3", "\u0036\u20E3", "\u0037\u20E3", "\u0038\u20E3", "\u0039\u20E3" };

        private static bool ready = false;

        public RoleManager()
        {
            if (ready) return;
            Console.WriteLine("Role Manager Started");
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

        private Task OnReady()
        {
            Load();
            return Task.CompletedTask;
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            SocketUser user = GlobalUtils.client.GetUser(reaction.UserId);
            SocketGuildUser guser = user as SocketGuildUser;
            if (channel.Id != roleChannel.Id || user.IsBot) return;// Task.CompletedTask;
            if (reaction.MessageId == roleMessageId)
            {
                // they reacted to the correct role message
                SocketGuild guild = GlobalUtils.client.Guilds.FirstOrDefault();
                guser = guild.GetUser(user.Id);
                for(int i = 0; i < roles.Count && i < GlobalUtils.menu_emoji.Count<string>(); i++)
                {
                    if(GlobalUtils.menu_emoji[i] == reaction.Emote.Name)
                    {
                        var result = from a in guild.Roles
                                     where a.Name == roles[i]
                                     select a;
                        SocketRole role = result.FirstOrDefault();
                        await guser.RemoveRoleAsync(role);
                    }
                }
            }
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            SocketUser user = GlobalUtils.client.GetUser(reaction.UserId);
            SocketGuildUser guser = user as SocketGuildUser;
            if (channel.Id != roleChannel.Id || user.IsBot) return;// Task.CompletedTask;
            if (reaction.MessageId == roleMessageId)
            {
                // they reacted to the correct role message
                SocketGuild guild = GlobalUtils.client.Guilds.FirstOrDefault();
                guser = guild.GetUser(user.Id);
                for (int i = 0; i < roles.Count && i < 9; i++)
                {
                    if (menu_emoji[i] == reaction.Emote.Name)
                    {
                        var result = from a in guild.Roles
                                     where a.Name == roles[i]
                                     select a;
                        SocketRole role = result.FirstOrDefault();
                        await guser.AddRoleAsync(role);
                    }
                }
            }
        }

        private static void Load()
        {
            if (GlobalUtils.client == null) return;
            ulong chanId = ulong.Parse(DataStorage.GetData("roleChannel"));
            roleChannel = (SocketGuildChannel)GlobalUtils.client.GetChannel(chanId);
            roleMessageId = ulong.Parse(DataStorage.GetData("rolesMessage"));
            Console.WriteLine($"Role Messege: {roleMessageId}");
            string[] role_array = DataStorage.GetData("roles").Split(',');
            roles.Clear();
            foreach (string r in role_array)
                roles.Add(r);
            Console.WriteLine($"Roles Count: {roles.Count}");
        }
        private static void Save()
        {
            DataStorage.SetData("roleChannel", roleChannel.Id.ToString());
            DataStorage.SetData("rolesMessage", roleMessageId.ToString());
            if(roles.Count > 0)
            {
                string r = roles[0];
                for (int i = 1; i < roles.Count; i++)
                    r += $",{roles[i]}";
                DataStorage.SetData("roles", r);
            }

        }

        [Command("setRoleChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RoleChannel([Remainder]string arg = "")
        {
            
            roleChannel = Context.Message.MentionedChannels.FirstOrDefault();
            if(roleChannel != null)
                await Context.Channel.SendMessageAsync($"Role Channel set to `{roleChannel.Name}`");
            else
                await Context.Channel.SendMessageAsync($"Channel `{arg}` not found");
            Save();
        }

        [Command("addRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddRole([Remainder]string arg = "")
        {
            
            var result = from a in Context.Guild.Roles
                         where a.Name == arg
                         select a;
            SocketRole role = result.FirstOrDefault();
            if(role == null)
                await Context.Channel.SendMessageAsync($"Couldn't find `{arg}` in Guild.Roles");
            else
            {
                if(roles.Contains(role.Name))
                    await Context.Channel.SendMessageAsync($"`{role.Name}` already added");
                else
                {
                    roles.Add(role.Name);
                    await Context.Channel.SendMessageAsync($"Added `{role.Name}`");
                }
                UpdateOrAddRoleMessage();
            }
        }
        [Command("removeRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveRole([Remainder]string arg = "")
        {
            var result = from a in Context.Guild.Roles
                         where a.Name == arg
                         select a;
            SocketRole role = result.FirstOrDefault();
            if (role == null)
                await Context.Channel.SendMessageAsync($"Couldn't find `{arg}` in Guild.Roles");
            else
            {
                if (roles.Contains(role.Name))
                {
                    roles.Remove(role.Name);
                    await Context.Channel.SendMessageAsync($"`{role.Name}` removed");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"`{role.Name}` not in managed roles list");
                }
                UpdateOrAddRoleMessage();
            }
        }

        private async void UpdateOrAddRoleMessage()
        {
            if (roleChannel == null) return;
            if (roles.Count == 0) return;

            ISocketMessageChannel chan = roleChannel as ISocketMessageChannel;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("General Roles");

            string roles_txt = "";
            List<IEmote> emotes = new List<IEmote>();
            for (int i = 0; i < roles.Count && i < 9; i++)
            {
                Emoji emoji = new Emoji(menu_emoji[i]);
                emotes.Add(emoji);
                roles_txt += $"{menu_emoji[i]} {roles[i]}\n";
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



        public async Task ReactAsync(RestUserMessage userMsg, string emoteName)
        {
            var emote = GlobalUtils.client.Guilds
                    .SelectMany(x => x.Emotes)
                    .FirstOrDefault(x => x.Name.IndexOf(
                        emoteName, StringComparison.OrdinalIgnoreCase) != -1);
            if (emote == null) return;
            await userMsg.AddReactionAsync(emote);
        }

    }


}
