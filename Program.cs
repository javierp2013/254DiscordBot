
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using _254DiscordBot.Services;

namespace _254DiscordBot
{
    class Program
    {
        // There is no need to implement IDisposable like before as we are
        // using dependency injection, which handles calling Dispose for us.
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                client.ReactionRemoved += Client_ReactionRemoved;
                client.ReactionAdded += Client_ReactionAdded;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hard coding.
                string token = System.IO.File.ReadAllText("Token.txt");
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            //for role adding through reactions!
            ulong reactionRole = DBCommands.reactionRoleExists(reaction.MessageId, reaction.Emote.ToString());
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

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> chan, SocketReaction reaction)
        {
            //for role removal after user unclicks a reaction.
            ulong reactionRole = DBCommands.reactionRoleExists(reaction.MessageId, reaction.Emote.ToString());
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
                    await reactionUser.SendMessageAsync("Sorry, something went wrong, I am either unable to modify roles or the chosen role is above me in settings. Contact an admin or Hoovier#4192 for assistance!");
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