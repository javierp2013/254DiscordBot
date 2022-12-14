using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace _254DiscordBot.Commands
{
    // Modified by: Javier Perez
    // [Part 2] Modified by: Jared De Los Santos
    //          To aid with clarity when doing !help
    // This class are all the Help Menu related commands.
    public class help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Alias("?")]
        public async Task HelpCommand()
        {
            var Builder = new Discord.EmbedBuilder();
            Builder.WithTitle("TuffyBot Help");
            Builder.AddField("What is TuffyBot?", "TuffyBot is a bot to assist with searching image boards such as Danbooru, Safebooru, and Sakugabooru.");
            Builder.AddField("What can it do? It can:", "Play minigames!\nRun periodic reminders!\nAssign roles automatically!\nSearch image boards!\nMuch more!");
            Builder.AddField("Further Help.", "For more details you can run !help <topic>. I.E. ``!help minigames``");
            Builder.AddField("Available commands checkable using \"!help <topic>\": ",
                           "1. ``minigames``\n" +
                           "2. ``reminders/reminder``\n" +
                           "3. ``roles/rolereactions``\n" +
                           "4. ``danbooru/dan``\n" +
                           "5. ``sakugabooru/sk``\n" +
                           "6. ``safebooru/sb``");
            Builder.WithThumbnailUrl("https://pbs.twimg.com/profile_images/1349407933872828416/hL5mjY6z_400x400.png");
            Builder.WithColor(Discord.Color.DarkOrange);
            await Context.Channel.SendMessageAsync("", false, Builder.Build());
        }

        [Command("help")]
        [Alias("?")]
        public async Task HelpCommand(string arg)
        {
            switch (arg)
            {
                case "minigames":
                    {
                        var Builder = new Discord.EmbedBuilder();
                        Builder.WithTitle("Minigame Help");
                        Builder.AddField("Slot Machine", "Slots bet a certain amount of currency and returns up to 3x the bet if all 3 symbols match, or as little as 0.\n``!slots <betAmount>``");
                        Builder.AddField("Stock Market", "Invest your money and let it ride, see how much your stock grows!\n``!stonks <stockName> <#ofShares>``");
                        Builder.AddField("Bet", "Bet your money, hope for the best!\n``!bet <amount>``");
                        Builder.WithThumbnailUrl("https://cdn3.iconfinder.com/data/icons/slot-machine-symbols-filled-outline/256/7-512.png");
                        Builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, Builder.Build());
                        break;
                    }
                case "reminders":
                case "reminder":
                    {
                        var Builder = new Discord.EmbedBuilder();
                        Builder.WithTitle("Reminders Help");
                        Builder.AddField("RemindMe", "Have me remind you about something at a scheduled time!\n``!remindme \"to water the plants\" 2H 42M`` or ``~remindme homework 23h 55m``");
                        Builder.AddField("Reminders", "List the reminders you have set so far!\n``!reminders``");
                        Builder.AddField("Remove Reminder", "Remove a saved reminder from my memory!\n``!removereminder <Title of Reminder>``");
                        Builder.WithThumbnailUrl("https://animalcare.umich.edu/sites/default/files/acu-purple-reminder-icon.png");
                        Builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, Builder.Build());
                        break;
                    }
                case "roles":
                case "rolereactions":
                    {
                        var Builder = new Discord.EmbedBuilder();
                        Builder.WithTitle("Reaction Roles Help");
                        Builder.AddField("Reaction Roles", "This function allows users to assign roles to themselves by reacting to a message!\n``!addrole <@Role> <Emoji> <MessageID of message to react to>``");
                        Builder.AddField("Remove Role", "Remove a saved reaction role from my memory!\n``!removerole <@Role>``");
                        Builder.AddField("List Roles", "List the reaction roles you have registered so far!\n``!listroles``");
                        Builder.WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/5146/5146077.png");
                        Builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, Builder.Build());
                        break;
                    }
                case "danbooru":
                case "dan":
                    {
                        var Builder = new Discord.EmbedBuilder();
                        Builder.WithTitle("DanBooru Help");
                        Builder.AddField("DanBooru", "This function allows users to search for images with a given tag, or set of tags from the DanBooru imageboard!\n``!danbooru <tag>``");
                        Builder.AddField("DanBooru Next", "Post next image in search results!\n``!danext <Number of images <= 5 to post>``");
                        Builder.AddField("DanBooru Tags", "List the tags for the last image!\n``!dantags``");
                        Builder.WithThumbnailUrl("https://static.wikia.nocookie.net/discord/images/8/8c/Danbooru_Discord_Icon.png/revision/latest/scale-to-width-down/500?cb=20200716084123");
                        Builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, Builder.Build());
                        break;
                    }
                case "sakugabooru":
                case "sk":
                    {
                        var Builder = new Discord.EmbedBuilder();
                        Builder.WithTitle("SakugaBooru Help");
                        Builder.AddField("SakugaBooru", "This function allows users to search for images with a given tag, or set of tags from the SakugaBooru imageboard!\n``!sakugabooru <tag>``");
                        Builder.AddField("SakugaBooru Next", "Post next image in search results!\n``!sknext <Number of images <= 5 to post>``");
                        Builder.AddField("SakugaBooru Tags", "List the tags for the last image!\n``!sktags``");
                        Builder.WithThumbnailUrl("https://pbs.twimg.com/profile_images/1164939893832855552/yPobwLMB_400x400.jpg");
                        Builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, Builder.Build());
                        break;
                    }
                case "safebooru":
                case "sb":
                    {
                        var Builder = new Discord.EmbedBuilder();
                        Builder.WithTitle("SafeBooru Help");
                        Builder.AddField("SafeBooru", "This function allows users to search for images with a given tag, or set of tags from the SafeBooru imageboard!\n``!safebooru <tag>``");
                        Builder.AddField("SafeBooru Next", "Post next image in search results!\n``!snext <Number of images <= 5 to post>``");
                        Builder.AddField("SafeBooru Tags", "List the tags for the last image!\n``!stags``");
                        Builder.WithThumbnailUrl("https://uxwing.com/wp-content/themes/uxwing/download/crime-security-military-law/safety-icon.png");
                        Builder.WithColor(Discord.Color.DarkOrange);
                        await Context.Channel.SendMessageAsync("", false, Builder.Build());
                        break;
                    }
                default:
                    await ReplyAsync("I don't recognize that command. Try '!help minigames', '!help booru', or '!help roles'!");
                    break;
            }
        }
    }
}
