using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _254DiscordBot.Commands
{
    public class help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Alias("?")]
        public async Task helpCommand()
        {
            var builder = new Discord.EmbedBuilder();
            builder.WithTitle("TuffyBot Help");
            builder.AddField("What is TuffyBot?", "TuffyBot is a bot to assist with searching image boards such as Danbooru, Safebooru, and Sakugabooru.");
            builder.AddField("What can it do? It can:", "Play minigames!\nRun periodic reminders!\nAssign roles automatically!\nSearch image boards!\nMuch more!");
            builder.AddField("Further Help.", "For more details you can run !help <topic>. I.E. ``!help minigames``");
            builder.WithThumbnailUrl("https://pbs.twimg.com/profile_images/1349407933872828416/hL5mjY6z_400x400.png");
            builder.WithColor(Discord.Color.DarkOrange);
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("help")]
        [Alias("?")]
        public async Task helpCommand(string arg)
        {
            switch (arg)
            {
                case "minigames":
                    {
                        var builder = new Discord.EmbedBuilder();
                        builder.WithTitle("Minigame Help");
                        builder.AddField("Slot Machine", "Slots bet a certain amount of currency and returns up to 3x the bet if all 3 symbols match, or as little as 0.\n``!slots <betAmount>``");
                        builder.AddField("Stock Market", "Invest your money and let it ride, see how much your stock grows!\n``!stonks <stockName> <#ofShares>``");
                        builder.AddField("Bet", "Bet your money, hope for the best!\n``!bet <amount>``");
                        builder.WithThumbnailUrl("https://cdn3.iconfinder.com/data/icons/slot-machine-symbols-filled-outline/256/7-512.png");
                        builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, builder.Build());
                        break;
                    }
                case "reminders":
                case "reminder":
                    {
                        var builder = new Discord.EmbedBuilder();
                        builder.WithTitle("Reminders Help");
                        builder.AddField("RemindMe", "Have me remind you about something at a scheduled time!\n``!remindme \"to water the plants\" 2H 42M`` or ``~remindme homework 23h 55m``");
                        builder.AddField("Reminders", "List the reminders you have set so far!\n``!reminders``");
                        builder.AddField("Remove Reminder", "Remove a saved reminder from my memory!\n``!removereminder <Title of Reminder>``");
                        builder.WithThumbnailUrl("https://animalcare.umich.edu/sites/default/files/acu-purple-reminder-icon.png");
                        builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, builder.Build());
                        break;
                    }
                case "roles":
                case "rolereactions":
                    {
                        var builder = new Discord.EmbedBuilder();
                        builder.WithTitle("Reaction Roles Help");
                        builder.AddField("Reaction Roles", "This function allows users to assign roles to themselves by reacting to a message!\n``!addrole <@Role> <Emoji> <MessageID of message to react to>``");
                        builder.AddField("Remove Role", "Remove a saved reaction role from my memory!\n``!removerole <@Role>``");
                        builder.AddField("List Roles", "List the reaction roles you have registered so far!\n``!listroles``");
                        builder.WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/5146/5146077.png");
                        builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, builder.Build());
                        break;
                    }
                case "danbooru":
                case "dan":
                    {
                        var builder = new Discord.EmbedBuilder();
                        builder.WithTitle("DanBooru Help");
                        builder.AddField("DanBooru", "This function allows users to search for images with a given tag, or set of tags from the DanBooru imageboard!\n``!danbooru <tag>``");
                        builder.AddField("DanBooru Next", "Post next image in search results!\n``!danext <Number of images <= 5 to post>``");
                        builder.AddField("DanBooru Tags", "List the tags for the last image!\n``!dantags``");
                        builder.WithThumbnailUrl("https://static.wikia.nocookie.net/discord/images/8/8c/Danbooru_Discord_Icon.png/revision/latest/scale-to-width-down/500?cb=20200716084123");
                        builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, builder.Build());
                        break;
                    }
                case "sakugabooru":
                case "sk":
                    {
                        var builder = new Discord.EmbedBuilder();
                        builder.WithTitle("SakugaBooru Help");
                        builder.AddField("SakugaBooru", "This function allows users to search for images with a given tag, or set of tags from the SakugaBooru imageboard!\n``!sakugabooru <tag>``");
                        builder.AddField("SakugaBooru Next", "Post next image in search results!\n``!sknext <Number of images <= 5 to post>``");
                        builder.AddField("SakugaBooru Tags", "List the tags for the last image!\n``!sktags``");
                        builder.WithThumbnailUrl("https://pbs.twimg.com/profile_images/1164939893832855552/yPobwLMB_400x400.jpg");
                        builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, builder.Build());
                        break;
                    }
                case "safebooru":
                case "sb":
                    {
                        var builder = new Discord.EmbedBuilder();
                        builder.WithTitle("SafeBooru Help");
                        builder.AddField("SafeBooru", "This function allows users to search for images with a given tag, or set of tags from the SafeBooru imageboard!\n``!safebooru <tag>``");
                        builder.AddField("SafeBooru Next", "Post next image in search results!\n``!snext <Number of images <= 5 to post>``");
                        builder.AddField("SafeBooru Tags", "List the tags for the last image!\n``!stags``");
                        builder.WithThumbnailUrl("https://uxwing.com/wp-content/themes/uxwing/download/crime-security-military-law/safety-icon.png");
                        builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, builder.Build());
                        break;
                    }
                default:
                    await ReplyAsync("I don't recognize that command. Try '!help minigames', '!help booru', or '!help roles'!");
                    break;
            }
        }
    }
}
