using System;

namespace Helper
{
    [Serializable]
    public class Message
    {
        public string Text;
        public string From;
        public string To;
        public DateTime TimeStamp;

        public Message(string text, string from, string to, DateTime time)
        {
            Text = text;
            From = from;
            To = to;
            TimeStamp = time;
        }
    }
}
