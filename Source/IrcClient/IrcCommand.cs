namespace BasicIrcClient
{
#warning Shall Implement this http://www.networksorcery.com/enp/protocol/irc.htm
    //:tmi.twitch.tv 002 loliconmaster52 :Your host is tmi.twitch.tv
    public class IrcCommand
    {
        public string Host { get; }
        public string CommandId { get; }
        public string Username { get; }
        public string Data { get; }

        public IrcCommand(string rawData)
        {
            var data = rawData.Split(' ', 2);
            //:tmi.twitch.tv
            //002 loliconmaster52 :Your host is tmi.twitch.tv
            Host = data[0];

            if (data.Length < 2)
            {
                return;
            }
            data = data[1].Split(' ', 2);
            //002
            //loliconmaster52 :Your host is tmi.twitch.tv
            CommandId = data[0];

            if (data.Length < 2)
            {
                return;
            }
            data = data[1].Split(' ', 2);
            //loliconmaster52
            //:Your host is tmi.twitch.tv
            Username = data[0];

            if (data.Length > 1)
            {
                Data = data[1].Substring(1);
            }
        }
    }
}
