using System;
using System.Collections.Generic;
using System.Text;

namespace Valley.Net.Bindings
{
    public sealed class PacketEventArgs : EventArgs
    {
        public Guid CorrelationId { get; private set; } = Guid.NewGuid();

        public INetworkPacket Packet { get; private set; }

        public IEndPointBinding Binding { get; private set; }

        public PacketEventArgs(INetworkPacket packet, IEndPointBinding binding = null)
        {
            Packet = packet;
            Binding = binding;
        }
    }
}
