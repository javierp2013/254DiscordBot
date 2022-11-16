using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace _254DiscordBot.Commands
{
    // Modified by: Javier Perez
    // This class is just a command to manually update the database User table.
    public class admin : ModuleBase<SocketCommandContext>
    {
        [Command("updateusers")]
        public async Task UpdateUserList()
        {
            var Server = Context.Guild;
            List<SocketGuildUser> UserList = Server.Users.ToList();
            DBCommands.AddOrUpdateServer(Server.Id, Server.Name);
            int UsersAdded = DBCommands.UpdateUserList(UserList, Server.Id);
            await ReplyAsync(UsersAdded + " Users added to DB!");
        }
    }
}
