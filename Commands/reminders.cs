using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace _254DiscordBot.Commands
{
    // Modified by: Vincent Nguyen
    // [Part 2] Modified by: Jared De Los Santos
    //          Inclusion of seconds in case of more specific timed reminders in a short duration.
    // This class are all the reminder related commands.
    public class reminders : ModuleBase<SocketCommandContext>
    {
        [Command("remindme")]
        [Alias("remind me", "rm", "addreminder")]
        public async Task AddReminderAsync(string title, [Remainder] string interval)
        {
            string[] ArrayOfTime = interval.Split(" ");
            double Time = 0;
            int Hours = 0, mins = 0;
            double secs = 0;
            string Temp = "";
            foreach (var item in ArrayOfTime)
            {
                if (!(item.Contains("H") || item.Contains("h") || item.Contains("M") || item.Contains("m") || item.Contains("S") || item.Contains("s")))
                {
                    await ReplyAsync("That's not gonna work! Make sure to format your response like ``~remindme \"to water the plants\" 2H 42M`` or ``~remindme homework 23h 55m``!");
                    return;
                }
                if (item.Contains("H") || item.Contains("h"))
                {
                    Temp = item.Trim(new char[] { 'H', 'h' });
                    Hours = int.Parse(Temp);
                    Time = Time + (Hours * 60);
                }
                else if (item.Contains("M") || item.Contains("m"))
                {
                    Temp = item.Trim(new char[] { 'M', 'm' });
                    mins = int.Parse(Temp);
                    Time = Time + (mins);
                }
                else if (item.Contains("S") || item.Contains("s"))
                {
                    Temp = item.Trim(new char[] { 'S', 's' });
                    secs = int.Parse(Temp);
                    Time = Time + (secs / 60);
                }

            }
            if (Time < 1)
            {
                await ReplyAsync("Sorry, your reminder has to be longer than 1 minute at least! I'm not a time traveler.");
            }
            else
            {
                try
                {
                    DBCommands.AddReminder(Context.User.Id, Context.Guild.Id, title, Convert.ToInt32(Time), DateTime.Now.ToString("yyyy-MM-dd.HH:mm:ss"));
                    await ReplyAsync("I'll remind you in " + Hours + " hour(s) and " + mins + " minute(s) and " + secs + " second(s)!");
                }
                catch (SQLiteException ex)
                {
                    int ErrID = DBCommands.GetErrorID(ex.Message);
                    switch (ErrID)
                    {
                        //if its a foreign key problem, server is not in the Servers Table, so add it and then try again
                        case 0:
                            Console.WriteLine("Foreign key failure when adding Reminder!");
                            await ReplyAsync("Sorry, something went wrong!");

                            break;
                        //if its a duplicate problem, 
                        case 1:
                            Console.WriteLine("Attempted to add Reminder, user already had one Reminder with that name! Server: " + Context.Guild.Name);
                            await ReplyAsync("You already have a Reminder with that title in this server!");
                            break;
                        //unknown error, spit out an error for me. 69 for obvious reasons.
                        case -1:
                            Console.WriteLine("SQL Error: " + ex.Message + "\nErrorNum:" + ex.ErrorCode);
                            await ReplyAsync("Something went wrong, contact Javier with error code: " + ex.ErrorCode);
                            break;
                    }
                }
            }
        }

        [Command("reminders")]
        public async Task ListRemindersAsync()
        {
            List<ReminderObject> Reminders = DBCommands.GetReminders(Context.User.Id);
            string Response = "**__Scheduled Reminders:__**\n";

            foreach (ReminderObject item in Reminders)
            {

                DateTime TimeAdded = DateTime.Parse(item.TimeAdded);
                DateTime Now = DateTime.Now;
                //get the amount of time passed since the reminder was added!
                TimeSpan Span = Now - TimeAdded;
                //amount of minutes * 600000000 = ticks for constructor.
                //this holds the amount of time that needs to have passed from when the reminder was added to when it expires
                TimeSpan TimeTilReminder = new TimeSpan((long)item.TimeInterval * 600000000);
                //this gets the difference between how much time should pass, and how much has actually passed.
                TimeSpan TimePassed = TimeTilReminder - Span;

                Response = Response + "**" + item.Title + ":** in " + TimePassed.Hours + " Hours and " + TimePassed.Minutes + " Minutes and " + TimePassed.Seconds + " Seconds.\n";
            }
            await ReplyAsync(Response);
        }

        [Command("removeReminder")]
        public async Task RemoveReminderAsync([Remainder] string title)
        {
            int Rows = DBCommands.RemoveReminder(title, Context.Guild.Id, Context.User.Id);
            if (Rows == 1)
            {
                await ReplyAsync("Reminder removed!");
            }
            else if (Rows == 0)
            {
                await ReplyAsync("No reminder removed, maybe no reminder with that title exists! ");
            }
            else
            {
                await ReplyAsync("Something went wrong, ask Javier for help! Error: " + Rows);
            }
        }
    }
}
