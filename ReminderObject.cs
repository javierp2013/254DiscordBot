using System;
using System.Collections.Generic;
using System.Text;

namespace _254DiscordBot
{
    public class ReminderObject
    {
        public string TimeAdded;
        public string Title;
        public int TimeInterval;
        public ulong UserID;
        public ulong ServerID;
        public ReminderObject(string _timeAdded, string _title, int _timeInterval, ulong _userID, ulong _serverID)
        {
            TimeAdded = _timeAdded;
            Title = _title;
            TimeInterval = _timeInterval;
            UserID = _userID;
            ServerID = _serverID;
        }
    }
}
