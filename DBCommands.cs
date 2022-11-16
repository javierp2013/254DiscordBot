using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using _254DiscordBot.Commands;
using Discord.WebSocket;

namespace _254DiscordBot
{
    // Modified by: Javier Perez
    // This class provides generic functions for accessing the database, useful for returning and writing data to the database for each command.
    internal static class DBCommands
    {
        //cs is connection string for DB
        private static readonly string connectionString = @"URI=file:Database.db; foreign keys=true;";

        //sourced from previous project:
        public static int GetErrorID(string errMessage)
        {
            if (errMessage.Contains("FOREIGN KEY"))
                return 0;
            else if (errMessage.Contains("UNIQUE"))
                return 1;
            else
                return -1;
        }
        public static int UpdateUserList(List<SocketGuildUser> listOfUsers, ulong serverID)
        {
            int rowsAffected = 0;
            //for all users in server, add them to Users DB, and also add them to relational table that tells us what server they are in
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            {
                foreach (SocketGuildUser user in listOfUsers)
                {
                    cmd.CommandText = $"INSERT INTO Users VALUES(@UserID, @Username) ON CONFLICT(ID) DO NOTHING; INSERT INTO In_Server VALUES(@UserID, @ServerID) ON CONFLICT DO NOTHING; ";
                    cmd.Parameters.AddWithValue("@UserID", user.Id);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@ServerID", serverID);
                    if (cmd.ExecuteNonQuery() == 2)
                    {
                        rowsAffected++;
                    }
                }
            }

            return rowsAffected;
        }

        //this adds a server to Servers Table, to keep track of servers that bot is in, and for the In_Server table to have a valid foreign key for serverID
        public static void AddOrUpdateServer(ulong serverID, string name)
        {
            //Use prepared statement in order to assure the correct insertion and prevent SQL injection.
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"INSERT INTO Servers VALUES({serverID}, @serverName) " +
                $"ON CONFLICT(ID) DO UPDATE SET Name = '@serverName';";
                cmd.Parameters.AddWithValue("@serverName", name);
            }
        }

        //just runs SQL from two functions above to DB
        public static int InsertData(string statement)
        {
            int rowsChanged = 0;
            using var con = new SQLiteConnection(connectionString);
            con.Open();

            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = statement;
                rowsChanged = cmd.ExecuteNonQuery();
            }
            return rowsChanged;
        }
        public static string GetMoneyBalanceDMs(ulong userID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT COUNT(ServerID) FROM In_Server WHERE UserID={userID}", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            rdr.Read();
            int numberOfServers = rdr.GetInt32(0);
            if (numberOfServers == 0)
            {
                return "NONE";
            }
            rdr.Close();

            string response = "Server: Balance\n";
            string bal = "0";
            commd.CommandText = $"SELECT NAME, ServerID FROM In_Server JOIN Servers ON ServerID = ID WHERE UserID = {userID};";
            using SQLiteDataReader msgs = commd.ExecuteReader();
            while (msgs.Read())
            {
                bal = GetMoneyBalance(userID, (ulong)msgs.GetInt64(1));
                if (bal == "NONE")
                { bal = "0"; }
                response = response + "**" + msgs.GetString(0) + ":** " + bal + " Bits\n";
            }
            return response;
        }
        public static void AddUsertoMoney(ulong UserID, int amount, ulong serverID, string timestamp)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //Adds users to table with set money as well as a timestamp to determine when they can run their next ~daily
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"INSERT INTO Money(UserID, Amount, ServerID, TimeStamp) VALUES({UserID}, {amount}, {serverID}, @timestamp);";
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.ExecuteNonQuery();
            }
        }
        public static void GiveMoney(ulong UserID, int amount, ulong serverID, string timestamp)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //Adds users to table with set money as well as a timestamp to determine when they can run their next ~daily
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"UPDATE Money SET Amount = Amount + {amount}, TimeStamp = @timestamp WHERE UserID = {UserID} AND ServerID = {serverID};";
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.ExecuteNonQuery();
            }
        }

        public static int PayMoney(ulong userID, long amount, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            int rowsAffected = 0;
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"UPDATE Money SET Amount = Amount + {amount} WHERE UserID = {userID} AND ServerID = {serverID};";
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected;
        }
        public static string GetMoneyTimeStamp(ulong userID, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT Count(TimeStamp) FROM Money WHERE UserID={userID} AND ServerID= {serverID}", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            rdr.Read();
            int numberOfTimestamps = rdr.GetInt32(0);
            if (numberOfTimestamps == 0)
            {
                return "NONE";
            }
            rdr.Close();
            commd.CommandText = $"SELECT TimeStamp FROM Money WHERE UserID={userID} AND ServerID= {serverID}";
            using SQLiteDataReader msgs = commd.ExecuteReader();
            msgs.Read();
            return msgs.GetString(0);
        }

        public static string GetMoneyBalance(ulong userID, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT Count(Amount) FROM Money WHERE UserID={userID} AND ServerID= {serverID}", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            rdr.Read();
            long numberOfMoney = rdr.GetInt64(0);
            if (numberOfMoney == 0)
            {
                return "NONE";
            }
            rdr.Close();
            commd.CommandText = $"SELECT Amount FROM Money WHERE UserID={userID} AND ServerID= {serverID}";
            using SQLiteDataReader msgs = commd.ExecuteReader();
            msgs.Read();
            return msgs.GetInt64(0).ToString();
        }

        public static string GetMoneyLeaders(ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            string board = "**Leaderboard:**\n```";
            using var commd = new SQLiteCommand($"SELECT Amount, Users.Username FROM Money JOIN Users ON Users.ID = Money.UserID WHERE ServerID={serverID} GROUP BY Users.Username ORDER BY Amount DESC LIMIT 10", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                board = board + rdr.GetString(1) + ": " + rdr.GetInt64(0) + " Bits\n";
            }

            return board + "```";
        }
        public static void AddStonk(string name, int numOfShares, int price, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //use prepared statement to make sure user provided data doesn't cause issues
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"INSERT INTO Stonks VALUES(@name, @shares, {price}, {serverID});";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@shares", numOfShares);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Stonk> GetStonkObj(ulong serverID)
        {
            List<Stonk> temp = new List<Stonk>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT Name, NumberOfShares, Price FROM Stonks WHERE ServerID = {serverID};", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                temp.Add(new Stonk(rdr.GetString(0), rdr.GetInt32(1), rdr.GetInt32(2), serverID));
            }
            return temp;
        }

        public static string GetStonks(ulong serverID)
        {
            Dictionary<string, int> availStonks = GetPurchasedStonks(serverID);
            int availableStonks = 0;
            using var con = new SQLiteConnection(connectionString);
            string response = "``Current Stonks:``\n";
            con.Open();
            using var commd = new SQLiteCommand($"SELECT Name, NumberOfShares, Price FROM Stonks WHERE ServerID = {serverID};", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                if (availStonks.ContainsKey(rdr.GetString(0)))
                {
                    availableStonks = rdr.GetInt32(1) - availStonks[rdr.GetString(0)];
                }
                else
                {
                    availableStonks = rdr.GetInt32(1);
                }
                response = response + $"__{rdr.GetString(0)}__\n" + $"**Max:** {rdr.GetInt32(1)}".PadRight(15, ' ') + $"**Available for purchase:** {availableStonks}".PadRight(40, ' ') + $" **Price:** ${rdr.GetInt32(2)}\n";
            }
            return response;
        }

        public static Dictionary<string, int> GetPurchasedStonks(ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            Dictionary<string, int> temp = new Dictionary<string, int>();
            con.Open();
            using var commd = new SQLiteCommand($"SELECT StonkName, SUM(NumOfShares) FROM Stonk_Record WHERE ServerID = {serverID} GROUP BY StonkName;", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                temp.Add(rdr.GetString(0), rdr.GetInt32(1));
            }
            return temp;
        }

        public static List<string> GetStonkInfo(string name)
        {
            using var con = new SQLiteConnection(connectionString);
            List<string> temp = new List<string>();
            con.Open();
            using var commd = new SQLiteCommand($"SELECT Name, NumberOfShares, Price FROM Stonks WHERE Name = @name;", con);
            commd.Parameters.AddWithValue("@name", name);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                temp.Add(rdr.GetString(0));
                temp.Add((rdr.GetInt32(1)).ToString());
                temp.Add((rdr.GetInt32(2)).ToString());
            }
            return temp;
        }

        public static void EditStonkShares(string name, int numOfShares)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //use prepared statement to make sure user provided data doesn't cause issues
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"UPDATE Stonks SET NumberOfShares = @shares WHERE Name = @name ;";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@shares", numOfShares);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EditStonkPrice(string name, int price, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //use prepared statement to make sure user provided data doesn't cause issues
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"UPDATE Stonks SET Price = @price WHERE Name = @name AND ServerID = {serverID} ;";
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddStonkPurchase(string name, int numOfShares, ulong userID, ulong serverID, string date)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var command = con.CreateCommand();
            command.CommandText =
            $"INSERT INTO Stonk_Record(StonkName, UserID, ServerID, NumOfShares) VALUES(@name, @userID, @serverID, @shares) ON CONFLICT(UserID, ServerID, StonkName) DO UPDATE SET NumOfShares= NumOfShares + {numOfShares};";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@userID", userID);
            command.Parameters.AddWithValue("@serverID", serverID);
            command.Parameters.AddWithValue("@shares", numOfShares);
            command.ExecuteNonQuery();
        }

        public static List<int> GetMaxShares(string name, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            List<int> temp = new List<int>();
            using var commd = new SQLiteCommand($"SELECT NumberOfShares, Price FROM Stonks WHERE Name=@name AND ServerID={serverID};", con);
            commd.Parameters.AddWithValue("@name", name);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            if (rdr.Read())
            {
                temp.Add(rdr.GetInt32(0));
                temp.Add(rdr.GetInt32(1));

            }
            else
            {
                return temp;
            }
            rdr.Close();
            int purchasedShares = 0;
            commd.CommandText = "SELECT NumOfShares FROM Stonk_Record WHERE StonkName = @name AND ServerID = @serverID";
            commd.Parameters.AddWithValue("@name", name);
            commd.Parameters.AddWithValue("@serverID", serverID);
            using SQLiteDataReader reader = commd.ExecuteReader();
            while (reader.Read())
            {
                purchasedShares = purchasedShares + reader.GetInt32(0);
            }
            temp.Add(purchasedShares);
            return temp;
        }

        public static string GetOwnedStonks(ulong userID, ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            string response = "``Owned Stonks:``\n";
            con.Open();
            using var commd = new SQLiteCommand($"SELECT StonkName, NumOfShares FROM Stonk_Record WHERE UserID = {userID} AND ServerID = {serverID} GROUP BY StonkName", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr.GetInt32(1) != 0)
                {
                    response = response + $"**{rdr.GetString(0)}** Owned: {rdr.GetInt32(1)}\n";
                }
            }
            return response;
        }
        public static bool HasEnoughStonk(ulong userID, ulong serverID, string stonkName, int amount)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT NumOfShares FROM Stonk_Record WHERE UserID = {userID} AND ServerID = {serverID} AND StonkName= @name GROUP BY StonkName", con);
            commd.Parameters.AddWithValue("@name", stonkName);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr.GetInt32(0) >= amount)
                {
                    return true;
                }
            }
            return false;
        }

        public static void SellStonk(ulong userID, ulong serverID, string stonkName, int amount)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"UPDATE Stonk_Record SET NumOfShares = NumOfShares - {amount} WHERE StonkName = @name AND ServerID = {serverID} AND UserID = {userID}", con);
            commd.Parameters.AddWithValue("@name", stonkName);
            commd.ExecuteNonQuery();
        }

        public static void StonkConfigSetup(ulong serverID, ulong channelID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //use prepared statement to make sure user provided data doesn't cause issues
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"INSERT INTO StonkConfig(ServerID, ChannelID) VALUES({serverID}, {channelID}) ON CONFLICT(ServerID) DO UPDATE SET ChannelID={channelID};";
                cmd.ExecuteNonQuery();
            }
        }

        public static ulong GetStonkChannel(ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT Count(ServerID) FROM StonkConfig WHERE ServerID= {serverID}", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            rdr.Read();
            int numberOfChannels = rdr.GetInt32(0);
            if (numberOfChannels == 0)
            {
                return 0;
            }
            rdr.Close();
            commd.CommandText = $"SELECT ChannelID FROM StonkConfig WHERE ServerID= {serverID}";
            using SQLiteDataReader msgs = commd.ExecuteReader();
            msgs.Read();
            return (ulong)msgs.GetInt64(0);
        }

        public static void AddReminder(ulong userID, ulong serverID, string title, int interval, string timeAdded)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //Adds a reminder to the table
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"INSERT INTO Reminders(UserID, ServerID, Title, TimeInterval, TimeAdded) VALUES({userID}, {serverID}, @title, @interval, @timeAdded);";
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@interval", interval);
                cmd.Parameters.AddWithValue("@timeAdded", timeAdded);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<ReminderObject> GetReminders()
        {
            using var con = new SQLiteConnection(connectionString);
            List<ReminderObject> temp = new List<ReminderObject>();
            con.Open();
            using var commd = new SQLiteCommand($"SELECT TimeAdded, Title, TimeInterval, UserID, ServerID FROM Reminders;", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                temp.Add(new ReminderObject(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2), (ulong)rdr.GetInt64(3), (ulong)rdr.GetInt64(4)));
            }
            return temp;
        }
        public static List<ReminderObject> GetReminders(ulong userID)
        {
            using var con = new SQLiteConnection(connectionString);
            List<ReminderObject> temp = new List<ReminderObject>();
            con.Open();
            using var commd = new SQLiteCommand($"SELECT TimeAdded, Title, TimeInterval, UserID, ServerID FROM Reminders WHERE UserID = {userID};", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            while (rdr.Read())
            {
                temp.Add(new ReminderObject(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2), (ulong)rdr.GetInt64(3), (ulong)rdr.GetInt64(4)));
            }
            return temp;
        }
        public static int RemoveReminder(string title, ulong serverID, ulong userID)
        {
            int rowsAffected = 0;
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"DELETE FROM Reminders WHERE title= @title AND ServerID = {serverID} AND UserID= {userID};";
                cmd.Parameters.AddWithValue("@title", title);
                rowsAffected = cmd.ExecuteNonQuery();
            }
            if (rowsAffected == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Succesfully removed stored reminder!");
                Console.ResetColor();
                return rowsAffected;
            }
            if (rowsAffected == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to remove stored reminder!");
                Console.ResetColor();
                return rowsAffected;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong with removing a reminder!!");
                Console.ResetColor();
                return rowsAffected;
            }
        }
        public static void SetReactionRole(ulong roleID, ulong serverID, string emojiName, ulong messageID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            //Adds a reaction and role pair to the DB
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"INSERT INTO Reaction_Roles(RoleID, ServerID, Emoji, MessageID) VALUES({roleID}, {serverID}, @emoji, @messageID);";
                cmd.Parameters.AddWithValue("@emoji", emojiName);
                cmd.Parameters.AddWithValue("@messageID", messageID);
                cmd.ExecuteNonQuery();
            }
        }
        public static ulong ReactionRoleExists(ulong messageID, string emojiName)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var commd = new SQLiteCommand($"SELECT RoleID FROM Reaction_Roles WHERE MessageID={messageID} AND Emoji = '{emojiName}'", con);
            using SQLiteDataReader rdr = commd.ExecuteReader();
            rdr.Read();
            if (!rdr.HasRows)
            {
                return 0;
            }
            ulong roleID = (ulong)rdr.GetInt64(0);
            rdr.Close();
            //return the roleID so bot can turn it into a role!
            return roleID;
        }

        public static string RemoveRole(ulong roleID)
        {
            int rowsAffected = 0;
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            {
                cmd.CommandText = $"DELETE FROM Reaction_Roles WHERE RoleID= {roleID};";
                rowsAffected = cmd.ExecuteNonQuery();
            }
            if (rowsAffected == 1)
                return "Succesfully removed role from list!";
            if (rowsAffected == 0)
                return "An error occured, no role removed. Maybe the role was never saved!";
            else
                return "ERROR! Code: " + rowsAffected;
        }

        public static Dictionary<ulong, string> ListRoles(ulong serverID)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            Dictionary<ulong, string> temp = new Dictionary<ulong, string>();
            using var commd = new SQLiteCommand($"SELECT RoleID, Emoji FROM Reaction_Roles WHERE ServerID={serverID}", con);
            string response = $"**__Role Reactions for this server:__** \n";
            using SQLiteDataReader msgs = commd.ExecuteReader();
            while (msgs.Read())
            {
                temp.Add((ulong)msgs.GetInt64(0), msgs.GetString(1));
            }

            if (temp.Count == 0)
            {
                temp.Add(0, "No Roles to show!");
            }
            return temp;
        }
    }
}
