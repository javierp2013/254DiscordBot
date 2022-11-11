using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageList = System.Collections.Generic.List<_254DiscordBot.SakugaBooruResponse>;

namespace _254DiscordBot.Commands
{
    public class Sakugabooru : ModuleBase<SocketCommandContext>
    {
        [Command("sakuga")]
        [Alias("saku", "sk")]
        public async Task sakugaSearchSort([Remainder] string srch)
        {
            await Context.Channel.TriggerTypingAsync();
            string url = $"https://www.sakugabooru.com/post.json?limit=50&tags={srch}";
            string respond = DanbooruResponse.getJSON(url).Result;
            if (respond == "failure")
            {
                await ReplyAsync("An error occurred! It is possible your search returned 0 results or the results are filtered out due to channel.");
                return;
            }
            ImageList responseList = JsonConvert.DeserializeObject<ImageList>(respond);
            if (responseList.Count == 0)
                await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
            else
            {
                Global.sakugabooruSearches[Context.Channel.Id] = respond;
                Random rand = new Random();
                Global.sakugaSearchIndex[Context.Channel.Id] = rand.Next(0, responseList.Count);
                SakugaBooruResponse chosenImage = responseList[Global.sakugaSearchIndex[Context.Channel.Id]];
                int loopCounter = 0;
                while (chosenImage.file_url == null)
                {
                    loopCounter++;
                    if (loopCounter > responseList.Count)
                    {
                        await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                        return;
                    }
                    Global.sakugaSearchIndex[Context.Channel.Id] = rand.Next(0, responseList.Count);
                    chosenImage = responseList[Global.sakugaSearchIndex[Context.Channel.Id]];
                }

                await ReplyAsync(chosenImage.file_url);
            }
        }

        //basically a copy of ~redditnext
        [Command("sakuga")]
        [Alias("saknext", "sanext")]
        public async Task sakugabooruNext()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.sakugabooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList responseList = JsonConvert.DeserializeObject<ImageList>(Global.sakugabooruSearches[Context.Channel.Id]);
                if (responseList.Count - 1 == Global.sakugaSearchIndex[Context.Channel.Id])
                {
                    if (responseList.Count == 2)
                    {
                        await ReplyAsync("Only 2 results in this search!\n" + responseList[0].file_url);
                    }
                    Global.sakugaSearchIndex[Context.Channel.Id] = 0;
                }
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
                //counter to keep track of how many times the While loop goes, make sure it doesnt keep loopinging in on itself
                int loopCounter = 0;
                //increment counter by 1
                Global.sakugaSearchIndex[Context.Channel.Id]++;
                while (responseList.ElementAt(Global.sakugaSearchIndex[Context.Channel.Id]).file_url == null)
                {
                    loopCounter++;
                    if (loopCounter > responseList.Count)
                    {
                        await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                        return;
                    }
                    if (Global.sakugaSearchIndex[Context.Channel.Id] + 1 == responseList.Count)
                    {
                        //if there are only 2 results, this glitches and only shows the second image, this will catch that edge case and spit them both out. 
                        if (responseList.Count == 2)
                        {
                            await ReplyAsync("Only 2 results in this search!\n" + responseList[1].file_url
                                + "\n" + responseList[0].file_url);
                            return;
                        }
                        Global.sakugaSearchIndex[Context.Channel.Id] = 0;
                    }
                    else
                    {
                        Global.sakugaSearchIndex[Context.Channel.Id]++;
                    }
                }
                await ReplyAsync(responseList[Global.sakugaSearchIndex[Context.Channel.Id]].file_url);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        //basically a copy of ~redditnext
        [Command("sakun")]
        [Alias("sakunext", "sknext")]
        public async Task sakuNextSpecific(int amount = 1)
        {
            await Context.Channel.TriggerTypingAsync();
            string response = $"Posting {amount} links:\n";
            //check user provided amount
            if (amount < 1 || amount > 5)
            {
                await ReplyAsync("Pick a number between 1 and 5!");
                return;
            }
            //if dictionary has an entry for channel, proceed
            if (Global.sakugabooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList responseList = JsonConvert.DeserializeObject<ImageList>(Global.sakugabooruSearches[Context.Channel.Id]);
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
                else if (responseList.Count - 1 < (Global.sakugaSearchIndex[Context.Channel.Id] + amount))
                {
                    await ReplyAsync("Reached end of results, resetting index. Use ~dnext to start again.");
                    Global.sakugaSearchIndex[Context.Channel.Id] = 0;
                    return;
                }
                //if all fail, proceed!
                else
                {
                    //counter to keep track of how many times the While loop goes, make sure it doesnt keep loopinging in on itself
                    int loopCounter = 0;
                    //loop through user provided amount
                    for (int counter = 0; counter < amount; counter++)
                    {
                        if (responseList.Count < Global.sakugaSearchIndex[Context.Channel.Id] + 1)
                        {
                            await ReplyAsync("Reached end of results, resetting index. Use ~dnext to start again.");
                            Global.sakugaSearchIndex[Context.Channel.Id] = 0;
                        }
                        //if everythings fine, increase index by 1
                        else
                        {
                            Global.sakugaSearchIndex[Context.Channel.Id]++;
                        }
                        while (responseList.ElementAt(Global.sakugaSearchIndex[Context.Channel.Id]).file_url == null)
                        {
                            loopCounter++;
                            if (loopCounter > responseList.Count)
                            {
                                await ReplyAsync("No returned images had valid links, you may have searched a hidden tag such as loli. Make a new search.");
                                return;
                            }
                            if (Global.sakugaSearchIndex[Context.Channel.Id] + 1 == responseList.Count)
                            {
                                Global.sakugaSearchIndex[Context.Channel.Id] = 0;
                            }
                            else
                            {
                                Global.sakugaSearchIndex[Context.Channel.Id]++;
                            }
                        }
                        response = response + responseList[Global.sakugaSearchIndex[Context.Channel.Id]].file_url + "\n";
                    }
                }

                await ReplyAsync(response);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        [Command("sakutags")]
        [Alias("sktags", "sakugaboorutags")]
        public async Task sakugabooruTags()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.sakugabooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList responseList = JsonConvert.DeserializeObject<ImageList>(Global.sakugabooruSearches[Context.Channel.Id]);
                SakugaBooruResponse chosen = responseList.ElementAt(Global.sakugaSearchIndex[Context.Channel.Id]);
                if (responseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }

                await ReplyAsync($"```{chosen.tags}```");
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~e <tag(s)>");
            }
        }
        
    }
}
