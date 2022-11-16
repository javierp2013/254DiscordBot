using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using ImageList = System.Collections.Generic.List<_254DiscordBot.DanbooruResponse.Image>;

namespace _254DiscordBot.Commands
{
    // Modified by: Javier Perez
    // This class are all the Danbooru related commands.
    public class Danbooru : ModuleBase<SocketCommandContext>
    {
        [Command("dan")]
        [Alias("danbooru", "danb")]
        public async Task DanbooruSearchSort([Remainder] string srch)
        {
            await Context.Channel.TriggerTypingAsync();
            string Url = $"https://danbooru.donmai.us/posts.json?tags={srch}+Rating%3Asafe&limit=50";
            string Response = DanbooruResponse.GetJSON(Url).Result;
            if (Response == "failure")
            {
                await ReplyAsync("An error occurred! It is possible your search returned 0 results or the results are filtered out due to channel.");
                return;
            }
            ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Response);
            if (ResponseList.Count == 0)
                await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
            else
            {
                Global.s_DanbooruSearches[Context.Channel.Id] = Response;
                Random Rand = new Random();
                Global.s_DanbooruSearchIndex[Context.Channel.Id] = Rand.Next(0, ResponseList.Count);
                DanbooruResponse.Image ChosenImage = ResponseList[Global.s_DanbooruSearchIndex[Context.Channel.Id]];
                int loopCounter = 0;
                while (ChosenImage.File_Url == null)
                {
                    loopCounter++;
                    if (loopCounter > ResponseList.Count)
                    {
                        await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                        return;
                    }
                    Global.s_DanbooruSearchIndex[Context.Channel.Id] = Rand.Next(0, ResponseList.Count);
                    ChosenImage = ResponseList[Global.s_DanbooruSearchIndex[Context.Channel.Id]];
                }

                await ReplyAsync(ChosenImage.File_Url + "\n" + ChosenImage.Tag_String_Artist.Replace(" ", ", "));
            }
        }

        //basically a copy of ~redditnext
        [Command("dn")]
        [Alias("danext", "dnext")]
        public async Task DanbooruNext()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.s_DanbooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_DanbooruSearches[Context.Channel.Id]);
                if (ResponseList.Count - 1 == Global.s_DanbooruSearchIndex[Context.Channel.Id])
                {
                    if (ResponseList.Count == 2)
                    {
                        await ReplyAsync("Only 2 results in this search!\n" + ResponseList[0].File_Url + "\n" + ResponseList[0].Tag_String_Artist);
                    }
                    Global.s_DanbooruSearchIndex[Context.Channel.Id] = 0;
                }
                if (ResponseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }
                if (ResponseList.Count == 1)
                {
                    await ReplyAsync("Only one result to show! \n" + ResponseList.ElementAt(0));
                    return;
                }
                //counter to keep track of how many times the While loop goes, make sure it doesnt keep loopinging in on itself
                int LoopCounter = 0;
                //increment counter by 1
                Global.s_DanbooruSearchIndex[Context.Channel.Id]++;
                while (ResponseList.ElementAt(Global.s_DanbooruSearchIndex[Context.Channel.Id]).File_Url == null)
                {
                    LoopCounter++;
                    if (LoopCounter > ResponseList.Count)
                    {
                        await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                        return;
                    }
                    if (Global.s_DanbooruSearchIndex[Context.Channel.Id] + 1 == ResponseList.Count)
                    {
                        //if there are only 2 results, this glitches and only shows the second image, this will catch that edge case and spit them both out. 
                        if (ResponseList.Count == 2)
                        {
                            await ReplyAsync("Only 2 results in this search!\n" + ResponseList[1].File_Url + "\n" + ResponseList[1].Tag_String_Artist
                                + "\n" + ResponseList[0].File_Url + "\n" + ResponseList[0].Tag_String_Artist);
                            return;
                        }
                        Global.s_DanbooruSearchIndex[Context.Channel.Id] = 0;
                    }
                    else
                    {
                        Global.s_DanbooruSearchIndex[Context.Channel.Id]++;
                    }
                }
                await ReplyAsync(ResponseList[Global.s_DanbooruSearchIndex[Context.Channel.Id]].File_Url + "\n"
                    + ResponseList[Global.s_DanbooruSearchIndex[Context.Channel.Id]].Tag_String_Artist);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        //basically a copy of ~redditnext
        [Command("dn")]
        [Alias("dnext", "danext")]
        public async Task DanbooruNextSpecific(int amount)
        {
            await Context.Channel.TriggerTypingAsync();
            string Response = $"Posting {amount} links:\n";
            //check user provided amount
            if (amount < 1 || amount > 5)
            {
                await ReplyAsync("Pick a number between 1 and 5!");
                return;
            }
            //if dictionary has an entry for channel, proceed
            if (Global.s_DanbooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_DanbooruSearches[Context.Channel.Id]);
                if (ResponseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }
                if (ResponseList.Count == 1)
                {
                    await ReplyAsync("Only one result to show! \n" + ResponseList.ElementAt(0));
                    return;
                }
                else if (ResponseList.Count - 1 < (Global.s_DanbooruSearchIndex[Context.Channel.Id] + amount))
                {
                    await ReplyAsync("Reached end of results, resetting index. Use ~dnext to start again.");
                    Global.s_DanbooruSearchIndex[Context.Channel.Id] = 0;
                    return;
                }
                //if all fail, proceed!
                else
                {
                    //counter to keep track of how many times the While loop goes, make sure it doesnt keep loopinging in on itself
                    int LoopCounter = 0;
                    //loop through user provided amount
                    for (int counter = 0; counter < amount; counter++)
                    {
                        if (ResponseList.Count < Global.s_DanbooruSearchIndex[Context.Channel.Id] + 1)
                        {
                            await ReplyAsync("Reached end of results, resetting index. Use ~dnext to start again.");
                            Global.s_DanbooruSearchIndex[Context.Channel.Id] = 0;
                        }
                        //if everythings fine, increase index by 1
                        else
                        {
                            Global.s_DanbooruSearchIndex[Context.Channel.Id]++;
                        }
                        while (ResponseList.ElementAt(Global.s_DanbooruSearchIndex[Context.Channel.Id]).File_Url == null)
                        {
                            LoopCounter++;
                            if (LoopCounter > ResponseList.Count)
                            {
                                await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                                return;
                            }
                            if (Global.s_DanbooruSearchIndex[Context.Channel.Id] + 1 == ResponseList.Count)
                            {
                                Global.s_DanbooruSearchIndex[Context.Channel.Id] = 0;
                            }
                            else
                            {
                                Global.s_DanbooruSearchIndex[Context.Channel.Id]++;
                            }
                        }
                        Response = Response + ResponseList[Global.s_DanbooruSearchIndex[Context.Channel.Id]].File_Url + "\n";
                    }
                }

                await ReplyAsync(Response);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        [Command("dantags")]
        [Alias("dant", "danboorutags")]
        public async Task DanbooruTags()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.s_DanbooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_DanbooruSearches[Context.Channel.Id]);
                DanbooruResponse.Image Chosen = ResponseList.ElementAt(Global.s_DanbooruSearchIndex[Context.Channel.Id]);
                if (ResponseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }

                await ReplyAsync(BuildDanbooruTags(Chosen));
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        //This dictionary returns the string corresponding to the rating member in an Image object.
        //ex: ratings[s] returns "Safe,"
        public readonly static Dictionary<string, string> Ratings = new Dictionary<string, string>()
        { { "s", "Safe, " }, { "q", "Questionable, " }, { "e", "Explicit, " } };
        /// <summary>Takes a danbooru.Image object and returns formatted tag string.</summary>
        /// <param name="img">A danbooru.Image object that will have tags extracted</param>
        /// <returns>Formatted tag and artist string.</returns>
        public static string BuildDanbooruTags(DanbooruResponse.Image img)
        {
            // Adds commas to tags, for easier reading.
            //put the artist and general tags string together.
            string AllTagsstring = img.Tag_String_General + " " + img.Tag_String_Copyright + " " + img.Tag_String_Character;
            string TagString = AllTagsstring.Replace(" ", ", ");
            // Create string with ratings and artist tags
            string ArtistAndRating = Ratings[img.Rating.ToString()] + "**Artist(s):** " + img.Tag_String_Artist;

            //return string with lots of formatting!
            return "**Info:** " + ArtistAndRating + "\n\n" + "All tags: \n```" + TagString + "```";
        }

    }
}
