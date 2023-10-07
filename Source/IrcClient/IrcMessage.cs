using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicIrcClient
{
    //insomnyawolf!insomnyawolf@insomnyawolf.tmi.twitch.tv PRIVMSG #insomnyawolf :TestMessage
    public class IrcMessage
    {
        public string Channel { get; }
        public string UserName { get; }
        public string Content { get; }
        public IrcAction Type { get; }
        public string HostMask { get; }

        public IrcMessage(string rawData)
        {
            var data = rawData.Split('!', 2);
            //insomnyawolf
            //insomnyawolf@insomnyawolf.tmi.twitch.tv PRIVMSG #insomnyawolf :TestMessage
            UserName = data[0];

            if (data.Length < 2)
            {
                return;
            }

            data = data[1].Split(' ', 2);
            //insomnyawolf@insomnyawolf.tmi.twitch.tv
            //PRIVMSG #insomnyawolf :TestMessage
            HostMask = data[0];

            if (data.Length < 2)
            {
                return;
            }
            data = data[1].Split(' ', 2);
            //PRIVMSG
            //#insomnyawolf :TestMessage
            if (!Enum.TryParse(data[0], out IrcAction Type))
            {
                throw new Exception($"Command type '{data[0]}' is not valid");
            }
            this.Type = Type;

            if (data.Length < 2)
            {
                return;
            }
            data = data[1].Split(' ', 2);
            //#insomnyawolf
            //:TestMessage
            Channel = data[0];

            if (data.Length > 1)
            {
                Content = data[1].Substring(1);
            }
        }
    }
}
