using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using _254DiscordBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace _254DiscordBot
{
    // Modified by: Javier Perez
    // This class is the entry point for the program and provides the barebones functionality of the bot IE registration with Discord.
    // Courtesy of official Discord.net documentation, much of the code in this file is from their barebones command bot.
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                client.Log += LogAsync;
                client.ReactionRemoved += ClientReactionRemoved;
                client.ReactionAdded += ClientReactionAdded;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hard coding.
                string token = System.IO.File.ReadAllText("Token.txt");
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                //here we make a timer and make it wait to trigger every 10 seconds!
                System.Timers.Timer aTimer = new System.Timers.Timer(10000);
                System.Timers.Timer Stonktimer = new System.Timers.Timer(3600000);
                //this is the actual function that runs when the time runs out
                aTimer.Elapsed += async (object sender, ElapsedEventArgs e) =>
                {
                    //get all reminders!
                    List<ReminderObject> listOfReminders = DBCommands.GetReminders();

                    foreach (ReminderObject item in listOfReminders)
                    {
                        DateTime timeAdded = DateTime.Parse(item.TimeAdded);
                        DateTime now = DateTime.Now;
                        //get the amount of time passed since the reminder was added!
                        TimeSpan span = now - timeAdded;
                        //if enough time has passed to be equal to or greater than the specified time interval, go
                        if (span.TotalMinutes >= item.TimeInterval)
                        {
                            //gets the server, uses that to get the user, and then finally sends the message
                            try
                            {
                                await client.GetGuild(item.ServerID).GetUser(item.UserID).CreateDMChannelAsync().Result.SendMessageAsync(item.Title);
                            }
                            catch
                            {
                                Console.WriteLine("Reminder message failed! User: " + item.UserID + "Server: " + item.ServerID + " Reminder: " + item.Title);
                            }
                            DBCommands.RemoveReminder(item.Title, item.ServerID, item.UserID);
                        }
                    }
                };
                Stonktimer.AutoReset = true;
                Stonktimer.Enabled = true;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task ClientReactionAdded(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            //for role adding through reactions!
            ulong reactionRole = DBCommands.ReactionRoleExists(reaction.MessageId, reaction.Emote.ToString());
            if (reactionRole != 0)
            {
                var message = await cache.DownloadAsync();
                //get guild to find role!
                var chnl = message.Channel as SocketGuildChannel;
                var reactionUser = reaction.User.Value as IGuildUser;
                Console.WriteLine("Giving role to " + reaction.User.Value.Username + "!");
                try
                {
                    await reactionUser.AddRoleAsync(chnl.Guild.GetRole(reactionRole));
                    await reactionUser.SendMessageAsync("Hi " + reactionUser.Username + "! I gave you the " + chnl.Guild.GetRole(reactionRole).Name + " role!");
                }
                catch
                {
                    await reactionUser.SendMessageAsync("Sorry, something went wrong, I am either unable to modify roles or the chosen role is above me in settings. Contact an admin or Hoovier#4192 for assistance!");
                    Console.WriteLine("Something went wrong giving out a role! Server: " + chnl.Guild.Name);
                }
            }
        }

        private async Task ClientReactionRemoved(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> chan, SocketReaction reaction)
        {
            //for role removal after user unclicks a reaction.
            ulong reactionRole = DBCommands.ReactionRoleExists(reaction.MessageId, reaction.Emote.ToString());
            if (reactionRole != 0)
            {
                var message = await cache.DownloadAsync();
                //get guild to find role!
                var chnl = message.Channel as SocketGuildChannel;
                var reactionUser = reaction.User.Value as IGuildUser;
                Console.WriteLine("Removing role from " + reaction.User.Value.Username + "!");
                try
                {
                    await reactionUser.RemoveRoleAsync(chnl.Guild.GetRole(reactionRole));
                }
                catch
                {
                    await reactionUser.SendMessageAsync("Sorry, something went wrong, I am either unable to modify roles or the chosen role is above me in settings. Contact an admin or Javier#4192 for assistance!");
                    return;
                }
                await reactionUser.SendMessageAsync("Hi " + reactionUser.Username + "! I removed the " + chnl.Guild.GetRole(reactionRole).Name + " role!");
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                })
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .BuildServiceProvider();
        }
    }
}
