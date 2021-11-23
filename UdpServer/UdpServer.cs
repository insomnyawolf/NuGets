using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer
{
    public class UdpServer
    {
        public delegate Task PacketRecived(UdpReceiveResult udpPacket);
        public event PacketRecived OnPacketRecived;

        private readonly UdpClient Listener;
        private readonly UdpServerSettings UdpServerSettings;
        private readonly ILogger<UdpServer> Logger;

        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private bool IsListening;

        public UdpServer(IServiceProvider ServiceProvider = null, UdpServerSettings UdpServerSettings = null, ILogger<UdpServer> Logger = null)
        {
            this.UdpServerSettings = UdpServerSettings ?? ServiceProvider.GetService<IConfiguration>()?.GetSection(nameof(UdpServerSettings)).Get<UdpServerSettings>() ?? throw new InvalidOperationException("Must configure the server first");

            this.Logger = Logger ?? ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger<UdpServer>();

            Listener = new UdpClient(this.UdpServerSettings.Port, AddressFamily.InterNetwork);
            //Listener.AllowNatTraversal(true);
            Listener.DontFragment = true;
            Listener.EnableBroadcast = true;
        }

        public void StartListener()
        {
            if (IsListening)
            {
                Logger?.LogWarning("Already Listening");
                return;
            }

            IsListening = true;

            CancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                Logger?.LogInformation($"Started listening in {UdpServerSettings.Port}");
                try
                {
                    while (IsListening)
                    {
                        var udpPacket = await Listener.ReceiveAsync(CancellationTokenSource.Token);

                        _ = Task.Run(async () =>
                        {
                            Logger?.LogTrace($"Packet from => '{udpPacket.RemoteEndPoint}' data => '{Encoding.UTF8.GetString(udpPacket.Buffer)}'");

                            if (OnPacketRecived is not null)
                            {
                                await OnPacketRecived.Invoke(udpPacket);
                            }
                        });
                    }
                }
                catch (SocketException e)
                {
                    Logger?.LogError(e.ToString());
                }
                finally
                {
                    Listener.Close();
                    IsListening = false;
                    CancellationTokenSource.Cancel();
                }
            });
        }

        public void StopListener()
        {
            if (!IsListening)
            {
                Logger?.LogWarning("Not Listening");
                return;
            }

            Listener.Close();
            CancellationTokenSource.Cancel(true);

            IsListening = false;
        }
    }
}