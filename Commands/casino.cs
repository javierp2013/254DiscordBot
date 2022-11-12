using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _254DiscordBot;
using Discord.Commands;

namespace CoreWaggles.Commands
{
    public class casino : ModuleBase<SocketCommandContext>
    {

        [Command("bet")]
        public async Task BetAsync(long amount)
        {
            string BalanceString = DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (BalanceString == "NONE" || BalanceString == "0")
            {
                await ReplyAsync("You dont have any money to bet!");
                return;
            }
            if (amount < 2)
            {
                await ReplyAsync("You have to bet more than that! Cheapskate.");
                return;
            }

            long SenderBal = long.Parse(BalanceString);
            if (amount > SenderBal || amount <= 0)
            {
                await ReplyAsync("Sorry, your balance of " + SenderBal + " Bits is too low!");
                return;
            }
            Random Rand = new Random();
            int ChosenNum = Rand.Next(0, 100);
            int RowsAffected = 0;
            //give it a 40% chance to succeed!
            if (ChosenNum < 41)
            {
                RowsAffected = DBCommands.PayMoney(Context.User.Id, amount, Context.Guild.Id);
                if (RowsAffected == 1)
                {
                    await ReplyAsync("Congrats! You won " + amount + " Bits, bringing your balance to " + (long.Parse(BalanceString) + amount) + "Bits!");
                }
                else
                {
                    await ReplyAsync("An error occurred! Contact Hoovier!");
                }
            }
            else
            {
                RowsAffected = DBCommands.PayMoney(Context.User.Id, -amount, Context.Guild.Id);
                if (RowsAffected == 1)
                {
                    await ReplyAsync("Oof. You lost " + amount + " Bits, bringing your balance to " + (long.Parse(BalanceString) - amount) + "Bits!");
                }
                else
                {
                    await ReplyAsync("An error occurred! Contact Hoovier!");
                }
            }
        }

        [Command("slots")]
        public async Task PlaySlots(long amount)
        {
            //check if user has the bits
            string BalanceString = DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (BalanceString == "NONE" || BalanceString == "0")
            {
                await ReplyAsync("You dont have any money to bet!");
                return;
            }
            if (amount < 2)
            {
                await ReplyAsync("You have to bet more than that! Cheapskate.");
                return;
            }
            long SenderBal = long.Parse(BalanceString);
            if (amount > SenderBal || amount <= 0)
            {
                await ReplyAsync("Sorry, your balance of " + SenderBal + " Bits is too low!");
                return;
            }
            //hard codes the emojis that will correspond with every random number
            Dictionary<int, string> EmojiDic = new Dictionary<int, string>
            {
                {0, ":lemon:" },
                {1, ":cherries:" },
                {2, ":seven:" },
                {3, ":star:" },
                {4, ":bell:" }
            };
            int RowsAffected;
            List<int> ChosenNumbers = new List<int>();
            Random Rand = new Random();
            string Response = "";
            for (int i = 0; i < 3; i++)
            {
                //gets a random number and stores it in the list
                ChosenNumbers.Add(Rand.Next(5));
                //gets random number generated from list.
                Response += EmojiDic[ChosenNumbers[i]] + " ";
            }
            //all match
            if (ChosenNumbers[0] == ChosenNumbers[1] && ChosenNumbers[2] == ChosenNumbers[1])
            {
                await ReplyAsync(Response);
                RowsAffected = DBCommands.PayMoney(Context.User.Id, amount * 3, Context.Guild.Id);
                await ReplyAsync("3 matches! You win " + (amount * 3) + " Bits, bringing your balance to " + (long.Parse(BalanceString) + (amount * 3)) + "Bits!");
                return;
            }
            // if 0 matches 1, if 1 matches 2, or 0 matches 2, but not all three
            else if (ChosenNumbers[0] == ChosenNumbers[1] || ChosenNumbers[2] == ChosenNumbers[1] || ChosenNumbers[0] == ChosenNumbers[2])
            {
                await ReplyAsync(Response);
                RowsAffected = DBCommands.PayMoney(Context.User.Id, amount, Context.Guild.Id);
                await ReplyAsync("2 matches! You win " + amount + " Bits, bringing your balance to " + (long.Parse(BalanceString) + amount) + "Bits!");
                return;
            }
            else
            {
                await ReplyAsync(Response);
                RowsAffected = DBCommands.PayMoney(Context.User.Id, -amount, Context.Guild.Id);
                await ReplyAsync("Whoops, you lost " + amount + "Bits, bringing your balance to " + (long.Parse(BalanceString) - amount) + "Bits!");
            }
        }



    }
}
