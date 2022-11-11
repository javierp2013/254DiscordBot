using Discord.Commands;
using Discord.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageList = System.Collections.Generic.List<_254DiscordBot.safeBooruUtil.safeOBJ>;

namespace _254DiscordBot.Commands
{
    public class Safebooru : ModuleBase<SocketCommandContext>
    {

        [Command("safe")]
        [Alias("safebooru", "sb")]
        public async Task safebooruSearchSort([Remainder] string srch)
        {
            await Context.Channel.TriggerTypingAsync();
            string url = $"https://safebooru.org/index.php?page=dapi&s=post&q=index&pid=1&tags={srch}&limit=50&json=1";
            string respond = safeBooruUtil.getJSON(url).Result;
            if (respond == "failure")
            {
                await ReplyAsync("An error occurred! " + url);
                return;
            }
            ImageList responseList = JsonConvert.DeserializeObject<ImageList>(respond);
            if (responseList.Count == 0)
                await ReplyAsync("No results! The tag may be misspelled.");
            else
            {
                Global.safeBooruSearches[Context.Channel.Id] = respond;
                Random rand = new Random();
                Global.safebooruSearchIndex[Context.Channel.Id] = rand.Next(0, responseList.Count);
                safeBooruUtil.safeOBJ chosenImage = responseList[Global.safebooruSearchIndex[Context.Channel.Id]];
                string imgLink = $"https://safebooru.org//images/{chosenImage.directory}/{chosenImage.image}";
                await Context.Channel.SendMessageAsync(imgLink);
            }
        }
        [Command("sn")]
        [Alias("snext", "sne")]
        public async Task e621Next()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.safeBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList responseList = JsonConvert.DeserializeObject<ImageList>(Global.safeBooruSearches[Context.Channel.Id]);
                if (responseList.Count - 1 == Global.safebooruSearchIndex[Context.Channel.Id])
                {
                    Global.safebooruSearchIndex[Context.Channel.Id] = 0;
                }
                if (responseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }
                if (responseList.Count == 1)
                {
                    await ReplyAsync("Only one result to show!");
                    return;
                }
                Global.safebooruSearchIndex[Context.Channel.Id]++;
                safeBooruUtil.safeOBJ chosenImage = responseList.ElementAt(Global.safebooruSearchIndex[Context.Channel.Id]);
                string imgLink = $"https://safebooru.org//images/{chosenImage.directory}/{chosenImage.image}";
                await Context.Channel.SendMessageAsync(imgLink);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~sb <tag(s)>");
            }
        }

        [Command("stags")]
        [Alias("st", "safeboorutags")]
        public async Task e621Tags()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.safeBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList responseList = JsonConvert.DeserializeObject<ImageList>(Global.safeBooruSearches[Context.Channel.Id]);
                safeBooruUtil.safeOBJ chosen = responseList.ElementAt(Global.safebooruSearchIndex[Context.Channel.Id]);
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
