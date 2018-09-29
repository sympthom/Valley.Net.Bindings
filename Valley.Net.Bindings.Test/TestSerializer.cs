using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valley.Net.Bindings.Test
{
    public sealed class TestSerializer : IPacketSerializer
    {
        public INetworkPacket Deserialize(byte[] data, int index, int count)
        {
            return new TestPacket();
        }

        public INetworkPacket Deserialize(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
                return Deserialize(reader);
        }

        public INetworkPacket Deserialize(BinaryReader reader)
        {
            return new TestPacket();
        }

        public T Deserialize<T>(byte[] data, int index, int count) where T : INetworkPacket
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(Stream stream) where T : INetworkPacket
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(BinaryReader reader) where T : INetworkPacket
        {
            throw new NotImplementedException();
        }

        public INetworkPacket Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(byte[] data) where T : INetworkPacket
        {
            throw new NotImplementedException();
        }

        public int Serialize(INetworkPacket packet, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
                return Serialize(packet, writer);
        }

        public int Serialize(INetworkPacket packet, BinaryWriter writer)
        {
            writer.Write(0x01);

            return 1;
        }
    }
}
