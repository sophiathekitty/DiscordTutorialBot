using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTutorialBot.Core
{
    internal static class GlobalUtils
    {
        internal static DiscordSocketClient client;
        internal static Color color = new Color(149, 237, 138);
        internal static Color null_color = new Color(149, 237, 138);
        internal static Color oblivion_color = new Color(138, 213, 237);
        internal static Color malic_color = new Color(237, 128, 131);
        internal static readonly string[] menu_emoji = { "\u0031\u20E3", "\u0032\u20E3", "\u0033\u20E3", "\u0034\u20E3", "\u0035\u20E3", "\u0036\u20E3", "\u0037\u20E3", "\u0038\u20E3", "\u0039\u20E3", "\u1F51F", "\u1F1E6", "\u1F1E7", "\u1F1E8", "\u1F1E9", "\u1F1EA", "\u1F1EB", "\u1F1EC", "\u1F1ED", "\u1F1EE", "\u1F1EF", "\u1F1F0", "\u1F1F1", "\u1F1F2", "\u1F1F3", "\u1F1F4", "\u1F1F5", "\u1F1F6", "\u1F1F7", "\u1F1F8", "\u1F1F9", "\u1F1FA", "\u1F1FB", "\u1F1FC", "\u1F1FD", "\u1F1FE", "\u1F1FF" };

    }
}
