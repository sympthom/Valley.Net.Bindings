using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Valley.Net.Bindings
{
    public interface IBinarySerializer
    {
        INetworkPacket Deserialize(byte[] data, int index, int count);

        T Deserialize<T>(byte[] data, int index, int count) where T : INetworkPacket;

        INetworkPacket Deserialize(Stream stream);

        T Deserialize<T>(Stream stream) where T : INetworkPacket;

        INetworkPacket Deserialize(BinaryReader reader);

        T Deserialize<T>(BinaryReader reader) where T : INetworkPacket;

        int Serialize(INetworkPacket packet, Stream stream);

        int Serialize(INetworkPacket packet, BinaryWriter writer);
    }
}
