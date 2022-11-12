using System;
using System.Collections.Generic;
using System.Text;

namespace _254DiscordBot
{
    internal static class Global
    {
        //holds the JSON for the last danbooru search in the channel
        internal static Dictionary<ulong, string> DanbooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        internal static Dictionary<ulong, int> DanbooruSearchIndex = new Dictionary<ulong, int>();
        //holds the JSON for the last sakugabooru search in the channel
        internal static Dictionary<ulong, string> SakugaBooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        internal static Dictionary<ulong, int> SakugaSearchIndex = new Dictionary<ulong, int>();
        //holds the JSON for the last safebooru search in the channel
        internal static Dictionary<ulong, string> SafeBooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        //probably come up with a better name for this
        internal static Dictionary<ulong, int> SafebooruSearchIndex = new Dictionary<ulong, int>();
    }
}
