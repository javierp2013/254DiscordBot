using System;
using System.Collections.Generic;
using System.Text;

namespace _254DiscordBot
{
    internal static class Global
    {
        //holds the JSON for the last danbooru search in the channel
        internal static Dictionary<ulong, string> danbooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        internal static Dictionary<ulong, int> danbooruSearchIndex = new Dictionary<ulong, int>();
        //holds the JSON for the last sakugabooru search in the channel
        internal static Dictionary<ulong, string> sakugabooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        internal static Dictionary<ulong, int> sakugaSearchIndex = new Dictionary<ulong, int>();
        //holds the JSON for the last safebooru search in the channel
        internal static Dictionary<ulong, string> safeBooruSearches = new Dictionary<ulong, string>();
        //holds the index of last used element of JSON array in cache
        //probably come up with a better name for this
        internal static Dictionary<ulong, int> safebooruSearchIndex = new Dictionary<ulong, int>();
    }
}
