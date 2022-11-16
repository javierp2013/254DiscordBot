using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace _254DiscordBot.Commands
{
    // Modified by: Vincent Nguyen
    // This class are all the stock related commands.
    public class stonks : ModuleBase<SocketCommandContext>
    {
        [Command("addstonk")]
        public async Task AddStonk(string name, int numOfShares, int price)
        {
            if (Context.User.Id != 223651215337193472)
            {
                await ReplyAsync("Only Javier can run this command!");
            }
            else
            {
                DBCommands.AddStonk(name, numOfShares, price, Context.Guild.Id);
                await ReplyAsync($"Added ``{name}`` stonk!");
            }

        }
        [Command("editstonk shares")]
        public async Task EditStonkshares(string name, int numOfShares)
        {
            if (Context.User.Id != 223651215337193472)
            {
                await ReplyAsync("Only Javier can run this command!");
            }
            else
            {
                DBCommands.EditStonkShares(name, numOfShares);
                await ReplyAsync($"Edited ``{name}`` stonk!");
            }

        }
        [Command("editstonk price")]
        public async Task EditStonkprice(string name, int price)
        {
            if (Context.User.Id != 223651215337193472)
            {
                await ReplyAsync("Only Javier can run this command!");
            }
            else
            {
                DBCommands.EditStonkPrice(name, price, Context.Guild.Id);
                await ReplyAsync($"Edited ``{name}`` stonk!");
            }

        }

        [Command("stonks")]
        public async Task PostStonk()
        {
            await ReplyAsync(DBCommands.GetStonks(Context.Guild.Id));
        }

        [Command("mystonks")]
        public async Task MyStonks()
        {
            await ReplyAsync(DBCommands.GetOwnedStonks(Context.User.Id, Context.Guild.Id));
        }


        [Command("buystonk")]
        public async Task BuyStonk(string stonk, int amount)
        {
            stonk = stonk.ToUpper();
            //0 = MaxNumOfShares
            //1 = stonkPrice
            //2 = OwnedShares
            List<int> StonkInfo = DBCommands.GetMaxShares(stonk, Context.Guild.Id);
            long Balance = long.Parse(DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id));

            if (StonkInfo.Count == 0)
            {
                await ReplyAsync("Sorry, there is no stock with that name on the market right now!");
            }
            else if (StonkInfo[2] + amount > StonkInfo[0])
            {
                await ReplyAsync("Sorry! Not enough stonks to complete your purchase!");
            }
            else if (StonkInfo[1] * amount > Balance)
            {
                await ReplyAsync("Sorry! You don't have enough Bits for this. The price for " + amount + " shares of this stonk is " + StonkInfo[1] * amount);
            }
            else
            {
                DateTime LocalDate = DateTime.Now;
                string DateString = LocalDate.ToString("yyyy-MM-dd.HH:mm:ss");
                DBCommands.AddStonkPurchase(stonk, amount, Context.User.Id, Context.Guild.Id, DateString);
                long TotalPrice = StonkInfo[1] * amount;
                DBCommands.PayMoney(Context.User.Id, -TotalPrice, Context.Guild.Id);
                await ReplyAsync("Stonks purchased! Your new balance is " + (Balance - (StonkInfo[1] * amount)) + " Bits!");
            }

        }

        [Command("sellstonk")]
        public async Task SellStonk(string name, int amount)
        {
            name = name.ToUpper();
            //1 = name, 2 = numOfShares, 3 = price
            List<string> StonkInfo = DBCommands.GetStonkInfo(name);
            Console.WriteLine(StonkInfo[0] + " " + StonkInfo[1] + " " + StonkInfo[2]);
            long Balance = long.Parse(DBCommands.GetMoneyBalance(Context.User.Id, Context.Guild.Id));
            bool HasEnoughStonks = DBCommands.HasEnoughStonk(Context.User.Id, Context.Guild.Id, name, amount);
            if (HasEnoughStonks)
            {
                DBCommands.SellStonk(Context.User.Id, Context.Guild.Id, name, amount);
                DBCommands.PayMoney(Context.User.Id, amount * int.Parse(StonkInfo[2]), Context.Guild.Id);
                await ReplyAsync("Your stonks were sold and you made " + amount * int.Parse(StonkInfo[2]) + " bits!");
            }
            else
            {
                await ReplyAsync("Sorry! You dont have enough shares of that stonk to complete this transaction.");
            }
        }

        [Command("stonksetup")]
        public async Task StonkSetup(SocketGuildChannel channel)
        {
            DBCommands.StonkConfigSetup(channel.Guild.Id, channel.Id);
            await ReplyAsync("Added " + channel.Name + " as the channel for all stonk updates!");
        }
    }
}
