using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using ImageList = System.Collections.Generic.List<_254DiscordBot.SakugaBooruResponse>;

namespace _254DiscordBot.Commands
{
    // Modified by: Javier Perez
    // This class are all the SakugaBooru related commands.
    public class Sakugabooru : ModuleBase<SocketCommandContext>
    {
        [Command("sakuga")]
        [Alias("saku", "sk")]
        public async Task SakugaSearchSort([Remainder] string srch)
        {
            await Context.Channel.TriggerTypingAsync();
            string Url = $"https://www.sakugabooru.com/post.json?limit=50&tags={srch}";
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
                Global.s_SakugaBooruSearches[Context.Channel.Id] = Response;
                Random Rand = new Random();
                Global.s_SakugaSearchIndex[Context.Channel.Id] = Rand.Next(0, ResponseList.Count);
                SakugaBooruResponse ChosenImage = ResponseList[Global.s_SakugaSearchIndex[Context.Channel.Id]];
                int LoopCounter = 0;
                while (ChosenImage.File_Url == null)
                {
                    LoopCounter++;
                    if (LoopCounter > ResponseList.Count)
                    {
                        await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                        return;
                    }
                    Global.s_SakugaSearchIndex[Context.Channel.Id] = Rand.Next(0, ResponseList.Count);
                    ChosenImage = ResponseList[Global.s_SakugaSearchIndex[Context.Channel.Id]];
                }

                await ReplyAsync(ChosenImage.File_Url);
            }
        }

        //basically a copy of ~redditnext
        [Command("sakuga")]
        [Alias("saknext", "sanext")]
        public async Task SakugaBooruNext()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.s_SakugaBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_SakugaBooruSearches[Context.Channel.Id]);
                if (ResponseList.Count - 1 == Global.s_SakugaSearchIndex[Context.Channel.Id])
                {
                    if (ResponseList.Count == 2)
                    {
                        await ReplyAsync("Only 2 results in this search!\n" + ResponseList[0].File_Url);
                    }
                    Global.s_SakugaSearchIndex[Context.Channel.Id] = 0;
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
                Global.s_SakugaSearchIndex[Context.Channel.Id]++;
                while (ResponseList.ElementAt(Global.s_SakugaSearchIndex[Context.Channel.Id]).File_Url == null)
                {
                    LoopCounter++;
                    if (LoopCounter > ResponseList.Count)
                    {
                        await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                        return;
                    }
                    if (Global.s_SakugaSearchIndex[Context.Channel.Id] + 1 == ResponseList.Count)
                    {
                        //if there are only 2 results, this glitches and only shows the second image, this will catch that edge case and spit them both out. 
                        if (ResponseList.Count == 2)
                        {
                            await ReplyAsync("Only 2 results in this search!\n" + ResponseList[1].File_Url
                                + "\n" + ResponseList[0].File_Url);
                            return;
                        }
                        Global.s_SakugaSearchIndex[Context.Channel.Id] = 0;
                    }
                    else
                    {
                        Global.s_SakugaSearchIndex[Context.Channel.Id]++;
                    }
                }
                await ReplyAsync(ResponseList[Global.s_SakugaSearchIndex[Context.Channel.Id]].File_Url);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        //basically a copy of ~redditnext
        [Command("sakun")]
        [Alias("sakunext", "sknext")]
        public async Task SakuNextSpecific(int amount = 1)
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
            if (Global.s_SakugaBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList responseList = JsonConvert.DeserializeObject<ImageList>(Global.s_SakugaBooruSearches[Context.Channel.Id]);
                if (responseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }
                if (responseList.Count == 1)
                {
                    await ReplyAsync("Only one result to show! \n" + responseList.ElementAt(0));
                    return;
                }
                else if (responseList.Count - 1 < (Global.s_SakugaSearchIndex[Context.Channel.Id] + amount))
                {
                    await ReplyAsync("Reached end of results, resetting index. Use ~dnext to start again.");
                    Global.s_SakugaSearchIndex[Context.Channel.Id] = 0;
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
                        if (responseList.Count < Global.s_SakugaSearchIndex[Context.Channel.Id] + 1)
                        {
                            await ReplyAsync("Reached end of results, resetting index. Use ~dnext to start again.");
                            Global.s_SakugaSearchIndex[Context.Channel.Id] = 0;
                        }
                        //if everythings fine, increase index by 1
                        else
                        {
                            Global.s_SakugaSearchIndex[Context.Channel.Id]++;
                        }
                        while (responseList.ElementAt(Global.s_SakugaSearchIndex[Context.Channel.Id]).File_Url == null)
                        {
                            LoopCounter++;
                            if (LoopCounter > responseList.Count)
                            {
                                await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                                return;
                            }
                            if (Global.s_SakugaSearchIndex[Context.Channel.Id] + 1 == responseList.Count)
                            {
                                Global.s_SakugaSearchIndex[Context.Channel.Id] = 0;
                            }
                            else
                            {
                                Global.s_SakugaSearchIndex[Context.Channel.Id]++;
                            }
                        }
                        Response = Response + responseList[Global.s_SakugaSearchIndex[Context.Channel.Id]].File_Url + "\n";
                    }
                }

                await ReplyAsync(Response);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        [Command("sakutags")]
        [Alias("sktags", "sakugaboorutags")]
        public async Task SakugaBooruTags()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.s_SakugaBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_SakugaBooruSearches[Context.Channel.Id]);
                SakugaBooruResponse Chosen = ResponseList.ElementAt(Global.s_SakugaSearchIndex[Context.Channel.Id]);
                if (ResponseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }

                await ReplyAsync($"```{Chosen.Tags}```");
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }

    }
}
