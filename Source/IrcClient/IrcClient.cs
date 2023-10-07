using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BasicIrcClient
{
    // https://dev.twitch.tv/docs/irc/guide
    // https://osu.ppy.sh/wiki/cs/Internet_Relay_Chat
    public class IrcClient
    {
        public string BotName { get; }
        public string Host { get; }

        private readonly string PrefixedHost;
        private readonly TcpClient TcpClient;
        private readonly StreamReader InputStream;
        private readonly StreamWriter OutputStream;
        private readonly ILogger Logger;
        private bool IsListening;

        public delegate Task IrcEvent(Context Context);

        public event IrcEvent OnMessageRecived;
        public event IrcEvent OnChannelJoin;
        public event IrcEvent OnChannelLeave;

        public IrcClient(IrcClientConfig IrcClientConfig, ILoggerFactory LoggerFactory) : this(IrcClientConfig.Host, IrcClientConfig.Port, IrcClientConfig.UserName, IrcClientConfig.Password, LoggerFactory)
        {
        }

        public IrcClient(string Host, int Port, string UserName, string Password, ILoggerFactory LoggerFactory)
        {
            BotName = UserName;

            Logger = LoggerFactory.CreateLogger($"{nameof(IrcClient)}=>{Host}=>{BotName}");

            TcpClient = new TcpClient(Host, Port);
            InputStream = new StreamReader(TcpClient.GetStream());
            OutputStream = new StreamWriter(TcpClient.GetStream());

            OutputStream.WriteLine($"PASS {Password}");
            OutputStream.WriteLine($"NICK {UserName}");
            OutputStream.WriteLine($"USER {UserName} 8 ^ :{UserName}");
            OutputStream.Flush();

            var firstMessage = InputStream.ReadLine();

            Console.WriteLine(firstMessage);

            Logger.LogTrace(firstMessage);

            var Command = new IrcCommand(firstMessage);

            PrefixedHost = Command.Host;

            this.Host = PrefixedHost.StartsWith(':') ? PrefixedHost.Substring(1) : PrefixedHost;
        }

        public async Task JoinChannelAsync(string channel)
        {
            await SendCommandAsync(IrcAction.JOIN, $"{channel}");
        }

        public async Task LeaveChannelAsync(string channel)
        {
            await SendCommandAsync(IrcAction.PART, $"{channel}");
        }

        public async Task SendCommandAsync(IrcAction command, string channel)
        {
            var commandRaw = Enum.GetName(typeof(IrcAction), command);
            await SendPacketAsync($"{commandRaw} {channel}");
        }

        private async Task SendPacketAsync(string packet)
        {
            await OutputStream.WriteLineAsync(packet);
            await OutputStream.FlushAsync();
        }

        private async Task StartListeningAsync()
        {
            while (IsListening)
            {
                var rawData = await InputStream.ReadLineAsync();

                //// No deberia ser necesario
                if (rawData is null)
                {
                    continue;
                }

                Logger.LogTrace(rawData);

                if (rawData.StartsWith("PING"))
                {
                    await SendPacketAsync(rawData.Replace("PING", "PONG"));
                }
                else if (rawData.StartsWith(PrefixedHost))
                {
                    var command = new IrcCommand(rawData);
                    Logger.LogDebug(command.ToJsonString());
                    // Process Command Events Here
                }
                else
                {
                    rawData = rawData.Substring(1);

                    var message = new IrcMessage(rawData);

                    Logger.LogDebug(message.ToJsonString());

                    var context = new Context(message, this);

                    if (message.Type == IrcAction.PRIVMSG)
                    {
                        await OnMessageRecived?.Invoke(context);
                    }
                    else if (message.Type == IrcAction.JOIN)
                    {
                        await OnChannelJoin?.Invoke(context);
                    }
                    else if (message.Type == IrcAction.PART)
                    {
                        await OnChannelLeave?.Invoke(context);
                    }
                }
            }
        }

        public void StartListening()
        {
            // Esto se usa para evitar que se inicie la escucha de paquetes multiples veces
            if (IsListening)
            {
                // Si te salta este error es que estas haciendo algo mal
                throw new Exception($"This {nameof(IrcClient)} instance is already listening.");
            }

            IsListening = true;

            // Es el comportamiento intencionado, para que la libreria pueda escuchar peticiones pero no bloquee el hilo principal del programa
            Task.Run(StartListeningAsync);
        }

#warning No para de escuchar instantaneamente, solo permite salir de el loop de escucha, necesario busscar mejor manera
        public void StopListening()
        {
            IsListening = false;
        }

        public async Task SendMessageAsync(string channel, string content)
        {
            var message = $"{IrcAction.PRIVMSG} {channel} :{content}";
            await OutputStream.WriteLineAsync(message);
            await OutputStream.FlushAsync();
        }
    }

    public class Context
    {
        public IrcMessage RecivedMessage { get; }
        private IrcClient IrcClient;

        public Context(IrcMessage RecivedMessage, IrcClient IrcClient)
        {
            this.RecivedMessage = RecivedMessage;
            this.IrcClient = IrcClient;
        }

        public async Task SendMessageAsync(string content)
        {
            if (RecivedMessage.Channel == IrcClient.BotName)
            {
                await IrcClient.SendMessageAsync(RecivedMessage.UserName, content);
                return;
            }
            await IrcClient.SendMessageAsync(RecivedMessage.Channel, content);
        }
    }

    public enum IrcAction
    {
        JOIN,
        PART,
        PRIVMSG,
        QUIT,
        MODE
    }
}










