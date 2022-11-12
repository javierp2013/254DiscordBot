using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace _254DiscordBot.Commands
{
    public class money : ModuleBase<SocketCommandContext>
    {
        [Command("bonus")]
        [Alias("daily")]
        public async Task DailyAsync()
        {
            if (Context.IsPrivate)
            {
                await ReplyAsync("Please run this command within the server of your choice!");
                return;
            }
            string Timestamp = DBCommands.GetMoneyTimeStamp(Context.User.Id, Context.Guild.Id);
            DateTime LocalDate = DateTime.Now;
            string DateString = LocalDate.ToString("yyyy-MM-dd.HH:mm:ss");
            //if there is no record of this user, insert into the table!
            if (Timestamp == "NONE")
            {
                DBCommands.AddUsertoMoney(Context.User.Id, 250, Context.Guild.Id, DateString);
                await ReplyAsync("Youve joined the rat race with a grant of 250 Bits! Come back every 8 hours for more!");
                return;
            }
            //figure out how long its been since the last ~daily
            DateTime Stamp = DateTime.ParseExact(Timestamp, "yyyy-MM-dd.HH:mm:ss", CultureInfo.InvariantCulture);
            TimeSpan Span = LocalDate - Stamp;

            if (Span.TotalHours > 1)
            {
                DBCommands.GiveMoney(Context.User.Id, 100, Context.Guild.Id, DateString);
                string Bal = DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id);
                await ReplyAsync("Congrats youve been gifted 100 Bits, your new balance is " + Bal);
            }
            else
            {
                double Remaining = Math.Round(60 - Span.TotalMinutes);
                if (Remaining < 61)
                {
                    await ReplyAsync("Sorry, wait " + Remaining.ToString() + " Minutes!");
                }
                else
                {
                    int Hours = (int)Remaining / 60;
                    int Mins = (int)Remaining % 60;
                    await ReplyAsync("Sorry, wait " + Hours + " hours, " + Mins + " minutes for your next bonus!");
                }

            }
        }

        [Command("balance")]
        [Alias("bal")]

        public async Task BalanceAsync()
        {
            if (Context.IsPrivate)
            {
                string Response = DBCommands.GetMoneyBalanceDMs(Context.User.Id);
                await ReplyAsync(Response);
                return;
            }
            string Bal = DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (Bal == "NONE")
            {
                await ReplyAsync("You are poor and destitute, not a cent to your name! Use ~bonus to beg for some.");
            }
            else
            {
                await ReplyAsync(Bal + " Bits!");
            }
        }
        [Command("balance")]
        [Alias("bal")]
        public async Task BalanceAsync(SocketGuildUser user)
        {
            if (Context.IsPrivate)
            {
                string Response = DBCommands.GetMoneyBalanceDMs(user.Id);
                await ReplyAsync(Response);
                return;
            }
            string Bal = DBCommands.GetMoneyBalance(user.Id, Context.Guild.Id);
            if (Bal == "NONE")
            {
                await ReplyAsync("They are poor and destitute, not a cent to their name! Consider gifting them some cash, or paying them with ~pay!");
            }
            else
            {
                await ReplyAsync(Bal + " Bits!");
            }
        }

        [Command("leaders")]
        [Alias("leaderboard")]
        public async Task MoneyLeadersAsync()
        {
            if (Context.IsPrivate)
            {
                await ReplyAsync("Sorry, this command is for server use only.");
                return;
            }
            string LeaderString = DBCommands.GetMoneyLeaders(Context.Guild.Id);

            await ReplyAsync(LeaderString);
        }

        [Command("pay")]
        public async Task PayAsync(SocketGuildUser user, long amount)
        {
            string BalanceString = DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (BalanceString == "NONE")
            {
                return;
            }
            long SenderBal = long.Parse(BalanceString);
            if (amount > SenderBal || amount <= 0)
            {
                await ReplyAsync("Sorry, your balance of " + SenderBal + " Bits is too low!");
                return;
            }
            if (DBCommands.PayMoney(user.Id, amount, Context.Guild.Id) == 1)
            {
                DBCommands.PayMoney(Context.User.Id, -amount, Context.Guild.Id);
                await ReplyAsync("Payment succesful! Your balance is now " + (SenderBal - amount) + " Bits!");
            }
        }
    }
}
