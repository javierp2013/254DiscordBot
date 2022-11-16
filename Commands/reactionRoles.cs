using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace _254DiscordBot.Commands
{
    // Modified by: Vincent Nguyen
    // This class are all the reaction roles related commands.
    public class reactionRoles : ModuleBase<SocketCommandContext>
    {
        [Command("addrole")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task AddRole(Discord.IRole role, string emoji, ulong messageID)
        {
            //try to give user role in order to test if perms are present
            IGuildUser User = (IGuildUser)Context.Message.Author;
            try
            {
                //if user has it, remove then add back, this prevents removal of roles from people who already had a role
                if (User.RoleIds.Contains(role.Id))
                {
                    await User.RemoveRoleAsync(role);
                    await User.AddRoleAsync(role);
                }
                //if they dont, add it and then remove again
                else
                {
                    await User.AddRoleAsync(role);
                    await User.RemoveRoleAsync(role);
                }
                Console.WriteLine("Successful Role addition test. Permissions ok.");
                //add a task to list
                DBCommands.SetReactionRole(role.Id, Context.Guild.Id, emoji, messageID);
                await ReplyAsync("Success! Reacting with ``" + emoji + "`` will give the user the ``" + role.Name + "`` role!");
            }
            catch
            {
                await ReplyAsync("Sorry! Something went wrong! Make sure that I have permission to modify roles and that my role is higher than the selected one!");
            }

        }

        [Command("removerole")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task RemoveRole(Discord.IRole role)
        {
            //remove a task from the database!
            string Result = DBCommands.RemoveRole(role.Id);
            await ReplyAsync(Result);
        }

        [Command("listrole")]
        [Alias("listroles", "lr")]

        public async Task ListRoles()
        {
            string Response = "**__Role Reactions for this server:__** \n";
            Dictionary<ulong, string> Result = DBCommands.ListRoles(Context.Guild.Id);
            if (Result.ContainsKey(0))
            {
                await ReplyAsync(Response + Result[0]);
                return;
            }
            foreach (KeyValuePair<ulong, string> pair in Result)
            {
                //get role name!
                Response = Response + Context.Guild.GetRole(pair.Key).Name + " " + pair.Value + "\n";
            }
            await ReplyAsync(Response);
        }
    }
}
