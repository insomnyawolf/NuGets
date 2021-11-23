using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpServer
{
    public static class Extensions
    {
        private static Dictionary<IPEndPoint, UdpClient> udpClients = new();
        public static async Task SendAnswerAsync(this UdpReceiveResult udpReceiveResult, byte[] data)
        {
            if (!udpClients.TryGetValue(udpReceiveResult.RemoteEndPoint, out var udpClient))
            {
                udpClient = new UdpClient(udpReceiveResult.RemoteEndPoint);
                udpClients.Add(udpReceiveResult.RemoteEndPoint, udpClient);
            }

            await udpClient.SendAsync(data, data.Length);
        }
    }
}
