using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valley.Net.Bindings.Test
{
    public sealed class ExceptionSerializer : IPacketSerializer
    {
        public INetworkPacket Deserialize(byte[] data, int index, int count)
        {
            throw new Exception("Test");
        }

        public INetworkPacket Deserialize(Stream stream)
        {
            throw new Exception("Test");
        }

        public INetworkPacket Deserialize(BinaryReader reader)
        {
            throw new NotImplementedException();
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

        public int Serialize(INetworkPacket package, Stream stream)
        {
            throw new Exception("Test");
        }

        public int Serialize(INetworkPacket package, BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
