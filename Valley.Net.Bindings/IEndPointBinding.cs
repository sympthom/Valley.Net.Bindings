using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Valley.Net.Bindings
{
    public interface IEndPointBinding
    {
        event EventHandler<PacketEventArgs> PacketReceived;

        bool ListenAsync(IPEndPoint endpoint);

        Task ConnectAsync(IPEndPoint endpoint);

        Task SendAsync(INetworkPacket packet);

        Task DisconnectAsync();
    }
}
