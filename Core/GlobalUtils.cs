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
        internal static readonly string[] menu_emoji = { "\u0031\u20E3", "\u0032\u20E3", "\u0033\u20E3", "\u0034\u20E3", "\u0035\u20E3", "\u0036\u20E3", "\u0037\u20E3", "\u0038\u20E3", "\u0039\u20E3", "🔟", "🇦", "🇧", "🇨", "🇩", "🇪", "🇫", "🇬", "🇭", "🇮", "🇯"  };

    }
}
