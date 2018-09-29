using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Valley.Net.Bindings.Udp
{
    public sealed class UdpBinding : SocketBinding, IEndPointBinding
    {
        public UdpBinding(Socket socket, IPacketSerializer serializer) :
            base(socket, serializer)
        {

        }

        public UdpBinding(IPacketSerializer serializer) : 
            this(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp), serializer)
        {
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        }

        public Task ConnectAsync(IPEndPoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            return Task.CompletedTask;
        }

        public Task SendAsync(INetworkPacket package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (_socket == null)
                throw new NullReferenceException(nameof(_socket));

            if (_serializer == null)
                throw new NullReferenceException(nameof(_serializer));

            if (_endpoint == null)
                throw new NullReferenceException(nameof(_endpoint));

            using (var stream = new MemoryStream())
            {
                var length = _serializer.Serialize(package, stream);

                var state = new OrderedAsyncState();
                state.Client = _socket;
                state.ReadEventArgs.RemoteEndPoint = _endpoint;
                state.ReadEventArgs.SetBuffer(stream.ToArray(), 0, length);
                state.ReadEventArgs.AcceptSocket = _socket;
                state.ReadEventArgs.UserToken = state;
                state.ReadEventArgs.Completed += OnCompleted;
                state.ReceiveAsync = e =>
                {
                    var receiveBuffer = new byte[MAX_BUFFER_SIZE];
                    e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                    return e.AcceptSocket.ReceiveAsync(e);
                };

                var result = _socket.SendToAsync(state.ReadEventArgs);

                return Task.CompletedTask;
            }
        }

        public bool ListenAsync(IPEndPoint endpoint)
        {
            var state = new OrderedAsyncState();
            state.Client = _socket;
            state.ReadEventArgs.RemoteEndPoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint)); // used to bind
            state.ReadEventArgs.SetBuffer(new byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);
            state.ReadEventArgs.AcceptSocket = _socket;
            state.ReadEventArgs.UserToken = state;
            state.ReadEventArgs.Completed += OnCompleted;
            state.ReceiveAsync = e =>
            {
                var receiveBuffer = new byte[MAX_BUFFER_SIZE];
                e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                return e.AcceptSocket.ReceiveFromAsync(e);
            };

            _socket.Bind(endpoint);

            return _socket.ReceiveFromAsync(state.ReadEventArgs);
        }

        private async void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            var state = e.UserToken as OrderedAsyncState;

            try
            {
                switch (e.SocketError)
                {
                    case SocketError.Success:
                        {
                            switch (e.LastOperation)
                            {
                                case SocketAsyncOperation.Receive:
                                case SocketAsyncOperation.ReceiveFrom:
                                    {
                                        state.Write(e.Buffer, e.BytesTransferred);

                                        var packet = _serializer.Deserialize(state.Buffer, 0, state.BytesReceived);

                                        if (packet == null)
                                            return;

                                        state.Clear();

                                        var binding = new UdpBinding(_serializer);
                                        await binding.ConnectAsync(e.RemoteEndPoint as IPEndPoint);

                                        OnPacketReceived(new PacketEventArgs(packet, binding));
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorAsyncEventArgs { Error = ex, State = state });
            }
            finally
            {
                OnIoCompleted(e);

                state.ReceiveAsync(e);
            }
        }       
    }
}
