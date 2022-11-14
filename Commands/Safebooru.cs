using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using ImageList = System.Collections.Generic.List<_254DiscordBot.SafeBooruUtil.SafeOBJ>;

namespace _254DiscordBot.Commands
{
    public class Safebooru : ModuleBase<SocketCommandContext>
    {

        [Command("safe")]
        [Alias("safebooru", "sb")]
        public async Task SafebooruSearchSort([Remainder] string srch)
        {
            await Context.Channel.TriggerTypingAsync();
            string Url = $"https://safebooru.org/index.php?page=dapi&s=post&q=index&pid=1&tags={srch}&limit=50&json=1";
            string Respond = SafeBooruUtil.GetJSON(Url).Result;
            if (Respond == "failure")
            {
                await ReplyAsync("An error occurred! " + Url);
                return;
            }
            ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Respond);
            if (ResponseList.Count == 0)
                await ReplyAsync("No results! The tag may be misspelled.");
            else
            {
                Global.s_SafeBooruSearches[Context.Channel.Id] = Respond;
                Random Rand = new Random();
                Global.s_SafebooruSearchIndex[Context.Channel.Id] = Rand.Next(0, ResponseList.Count);
                SafeBooruUtil.SafeOBJ ChosenImage = ResponseList[Global.s_SafebooruSearchIndex[Context.Channel.Id]];
                string imgLink = $"https://safebooru.org//images/{ChosenImage.Directory}/{ChosenImage.Image}";
                await Context.Channel.SendMessageAsync(imgLink);
            }
        }
        [Command("sn")]
        [Alias("snext", "sne")]
        public async Task SafeNext()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.s_SafeBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_SafeBooruSearches[Context.Channel.Id]);
                if (ResponseList.Count - 1 == Global.s_SafebooruSearchIndex[Context.Channel.Id])
                {
                    Global.s_SafebooruSearchIndex[Context.Channel.Id] = 0;
                }
                if (ResponseList.Count == 0)
                {
                    await ReplyAsync("No results! The tag may be misspelled, or the results could be filtered out due to channel!");
                    return;
                }
                if (ResponseList.Count == 1)
                {
                    await ReplyAsync("Only one result to show!");
                    return;
                }
                Global.s_SafebooruSearchIndex[Context.Channel.Id]++;
                SafeBooruUtil.SafeOBJ ChosenImage = ResponseList.ElementAt(Global.s_SafebooruSearchIndex[Context.Channel.Id]);
                string ImgLink = $"https://safebooru.org//images/{ChosenImage.Directory}/{ChosenImage.Image}";
                await Context.Channel.SendMessageAsync(ImgLink);
            }
            else
            {
                await ReplyAsync("You have to make a search first! Try running ~sb <tag(s)>");
            }
        }

        [Command("stags")]
        [Alias("st", "safeboorutags")]
        public async Task SafeTags()
        {
            await Context.Channel.TriggerTypingAsync();
            if (Global.s_SafeBooruSearches.ContainsKey(Context.Channel.Id))
            {
                ImageList ResponseList = JsonConvert.DeserializeObject<ImageList>(Global.s_SafeBooruSearches[Context.Channel.Id]);
                SafeBooruUtil.SafeOBJ Chosen = ResponseList.ElementAt(Global.s_SafebooruSearchIndex[Context.Channel.Id]);
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
