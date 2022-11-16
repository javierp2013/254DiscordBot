using System;
using System.Collections.Generic;
using System.Text;

namespace _254DiscordBot
{
    // Modified by: Javier Perez
    // This class is used as a static dictionary of data that the bot must keep in memory, such as the last search in each channel.
    //it is a simple channelID -> data relationship
    internal static class Global
    {
        //holds the JSON for the last danbooru search in the channel
        internal static Dictionary<ulong, string> s_DanbooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        internal static Dictionary<ulong, int> s_DanbooruSearchIndex = new Dictionary<ulong, int>();
        //holds the JSON for the last sakugabooru search in the channel
        internal static Dictionary<ulong, string> s_SakugaBooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        internal static Dictionary<ulong, int> s_SakugaSearchIndex = new Dictionary<ulong, int>();
        //holds the JSON for the last safebooru search in the channel
        internal static Dictionary<ulong, string> s_SafeBooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        //probably come up with a better name for this
        internal static Dictionary<ulong, int> s_SafebooruSearchIndex = new Dictionary<ulong, int>();
    }
}
