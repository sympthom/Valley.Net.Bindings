using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Valley.Net.Bindings
{
    public abstract class SocketBinding : IDisposable
    {
        protected const int MAX_BUFFER_SIZE = 2048;
        protected readonly Socket _socket;
        protected readonly IPacketSerializer _serializer;
        protected EndPoint _endpoint;

        public event EventHandler<PacketEventArgs> PacketReceived;
        public event EventHandler<SocketAsyncEventArgs> IoCompleted;
        public event EventHandler<ErrorAsyncEventArgs> Error;

        public SocketBinding(Socket socket, IPacketSerializer serializer)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        protected void OnPacketReceived(PacketEventArgs e)
        {
            PacketReceived?.Invoke(this, e);
        }

        protected void OnIoCompleted(SocketAsyncEventArgs e)
        {
            IoCompleted?.Invoke(this, e);
        }

        protected void OnError(ErrorAsyncEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public override string ToString()
        {
            switch (_endpoint)
            {
                case IPEndPoint ipEndpoint: return $"{GetType().Name} {ipEndpoint.Address.ToString()}:{ipEndpoint.Port}";
                default: return base.ToString();
            }
        }
    }
}
