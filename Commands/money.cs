using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

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
            string timestamp = DBCommands.getMoneyTimeStamp(Context.User.Id, Context.Guild.Id);
            DateTime localDate = DateTime.Now;
            string dateString = localDate.ToString("yyyy-MM-dd.HH:mm:ss");
            //if there is no record of this user, insert into the table!
            if (timestamp == "NONE")
            {
                DBCommands.addUsertoMoney(Context.User.Id, 250, Context.Guild.Id, dateString);
                await ReplyAsync("Youve joined the rat race with a grant of 250 Bits! Come back every 8 hours for more!");
                return;
            }
            //figure out how long its been since the last ~daily
            DateTime stamp = DateTime.ParseExact(timestamp, "yyyy-MM-dd.HH:mm:ss", CultureInfo.InvariantCulture);
            TimeSpan span = localDate - stamp;

            if (span.TotalHours > 1)
            {
                DBCommands.giveMoney(Context.User.Id, 100, Context.Guild.Id, dateString);
                string bal = DBCommands.getMoneyBalance(Context.User.Id, Context.Guild.Id);
                await ReplyAsync("Congrats youve been gifted 100 Bits, your new balance is " + bal);
            }
            else
            {
                double remaining = Math.Round(60 - span.TotalMinutes);
                if (remaining < 61)
                {
                    await ReplyAsync("Sorry, wait " + remaining.ToString() + " Minutes!");
                }
                else
                {
                    int hours = (int)remaining / 60;
                    int mins = (int)remaining % 60;
                    await ReplyAsync("Sorry, wait " + hours + " hours, " + mins + " minutes for your next bonus!");
                }

            }
        }

        [Command("balance")]
        [Alias("bal")]

        public async Task BalanceAsync()
        {
            if (Context.IsPrivate)
            {
                string response = DBCommands.getMoneyBalanceDMs(Context.User.Id);
                await ReplyAsync(response);
                return;
            }
            string bal = DBCommands.getMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (bal == "NONE")
            {
                await ReplyAsync("You are poor and destitute, not a cent to your name! Use ~bonus to beg for some.");
            }
            else
            {
                await ReplyAsync(bal + " Bits!");
            }
        }
        [Command("balance")]
        [Alias("bal")]
        public async Task BalanceAsync(SocketGuildUser user)
        {
            if (Context.IsPrivate)
            {
                string response = DBCommands.getMoneyBalanceDMs(user.Id);
                await ReplyAsync(response);
                return;
            }
            string bal = DBCommands.getMoneyBalance(user.Id, Context.Guild.Id);
            if (bal == "NONE")
            {
                await ReplyAsync("They are poor and destitute, not a cent to their name! Consider gifting them some cash, or paying them with ~pay!");
            }
            else
            {
                await ReplyAsync(bal + " Bits!");
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
            string leaderString = DBCommands.getMoneyLeaders(Context.Guild.Id);

            await ReplyAsync(leaderString);
        }

        [Command("pay")]
        public async Task PayAsync(SocketGuildUser user, long amount)
        {
            string balanceString = DBCommands.getMoneyBalance(Context.User.Id, Context.Guild.Id);
            if (balanceString == "NONE")
            {
                return;
            }
            long senderBal = long.Parse(balanceString);
            if (amount > senderBal || amount <= 0)
            {
                await ReplyAsync("Sorry, your balance of " + senderBal + " Bits is too low!");
                return;
            }
            if (DBCommands.payMoney(user.Id, amount, Context.Guild.Id) == 1)
            {
                DBCommands.payMoney(Context.User.Id, -amount, Context.Guild.Id);
                await ReplyAsync("Payment succesful! Your balance is now " + (senderBal - amount) + " Bits!");
            }
        }
    }
}
