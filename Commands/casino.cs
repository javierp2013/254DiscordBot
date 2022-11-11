using _254DiscordBot;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace CoreWaggles.Commands
{
    public class casino : ModuleBase<SocketCommandContext>
    {

        [Command("bet")]
        public async Task BetAsync(long amount)
        {
            string balanceString = DBCommands.getMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (balanceString == "NONE" || balanceString == "0")
            {
                await ReplyAsync("You dont have any money to bet!");
                return;
            }
            if (amount < 2)
            {
                await ReplyAsync("You have to bet more than that! Cheapskate.");
                return;
            }
            
            long senderBal = long.Parse(balanceString);
            if (amount > senderBal || amount <= 0)
            {
                await ReplyAsync("Sorry, your balance of " + senderBal + " Bits is too low!");
                return;
            }
            Random rand = new Random();
            int chosenNum = rand.Next(0, 100);
            int rowsAffected = 0;
            //give it a 40% chance to succeed!
            if (chosenNum < 41)
            {
                rowsAffected = DBCommands.payMoney(Context.User.Id, amount, Context.Guild.Id);
                if (rowsAffected == 1)
                {
                    await ReplyAsync("Congrats! You won " + amount + " Bits, bringing your balance to " + (long.Parse(balanceString) + amount) + "Bits!");
                }
                else
                {
                    await ReplyAsync("An error occurred! Contact Hoovier!");
                }
            }
            else
            {
                rowsAffected = DBCommands.payMoney(Context.User.Id, -amount, Context.Guild.Id);
                if (rowsAffected == 1)
                {
                    await ReplyAsync("Oof. You lost " + amount + " Bits, bringing your balance to " + (long.Parse(balanceString) - amount) + "Bits!");
                }
                else
                {
                    await ReplyAsync("An error occurred! Contact Hoovier!");
                }
            }
        }

        [Command("slots")]
        public async Task playSlots(long amount)
        {
            //check if user has the bits
            string balanceString = DBCommands.getMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (balanceString == "NONE" || balanceString == "0")
            {
                await ReplyAsync("You dont have any money to bet!");
                return;
            }
            if (amount < 2)
            {
                await ReplyAsync("You have to bet more than that! Cheapskate.");
                return;
            }
            long senderBal = long.Parse(balanceString);
            if (amount > senderBal || amount <= 0)
            {
                await ReplyAsync("Sorry, your balance of " + senderBal + " Bits is too low!");
                return;
            }
            //hard codes the emojis that will correspond with every random number
            Dictionary<int, string> emojiDic = new Dictionary<int, string>
            {
                {0, ":lemon:" },
                {1, ":cherries:" },
                {2, ":seven:" },
                {3, ":star:" },
                {4, ":bell:" }
            };
            int rowsAffected;
            List<int> chosenNumbers = new List<int>();
            Random rand = new Random();
            string response = "";
            for (int i = 0; i < 3; i++)
            {
                //gets a random number and stores it in the list
                chosenNumbers.Add(rand.Next(5));
                //gets random number generated from list.
                response += emojiDic[chosenNumbers[i]] + " ";
            }
            //all match
            if (chosenNumbers[0] == chosenNumbers[1] && chosenNumbers[2] == chosenNumbers[1])
            {
                await ReplyAsync(response);
                rowsAffected = DBCommands.payMoney(Context.User.Id, amount * 3, Context.Guild.Id);
                await ReplyAsync("3 matches! You win " + (amount * 3) + " Bits, bringing your balance to " + (long.Parse(balanceString) + (amount * 3)) + "Bits!");
                return;
            }
            // if 0 matches 1, if 1 matches 2, or 0 matches 2, but not all three
            else if (chosenNumbers[0] == chosenNumbers[1] || chosenNumbers[2] == chosenNumbers[1] || chosenNumbers[0] == chosenNumbers[2])
            {
                await ReplyAsync(response);
                rowsAffected = DBCommands.payMoney(Context.User.Id, amount, Context.Guild.Id);
                await ReplyAsync("2 matches! You win " + amount + " Bits, bringing your balance to " + (long.Parse(balanceString) + amount) + "Bits!");
                return;
            }
            else
            {
                await ReplyAsync(response);
                rowsAffected = DBCommands.payMoney(Context.User.Id, -amount, Context.Guild.Id);
                await ReplyAsync("Whoops, you lost " + amount + "Bits, bringing your balance to " + (long.Parse(balanceString) - amount) + "Bits!");
            }
        }

       

        }
}
