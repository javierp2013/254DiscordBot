using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _254DiscordBot.Commands
{
    public class admin : ModuleBase<SocketCommandContext>
    {
        [Command("updateusers")]
        public async Task updateUserList()
        {
            var server = Context.Guild;
            List<SocketGuildUser> userList = server.Users.ToList();
            DBCommands.addOrUpdateServer(server.Id, server.Name);
            int usersAdded = DBCommands.updateUserList(userList, server.Id);
            await ReplyAsync(usersAdded + " Users added to DB!");
        }
    }
}
