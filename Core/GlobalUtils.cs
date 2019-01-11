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
    }
}
