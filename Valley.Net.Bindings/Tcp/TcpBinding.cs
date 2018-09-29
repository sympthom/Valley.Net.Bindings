using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Valley.Net.Bindings.Tcp
{
    public sealed class TcpBinding : SocketBinding, IEndPointBinding
    {
        public TcpBinding(Socket socket, IPacketSerializer serializer) :
            base(socket, serializer)
        {

        }

        public TcpBinding(IPacketSerializer serializer) :
            this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), serializer)
        {

        }

        public Task ConnectAsync(IPEndPoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            if (_socket == null)
                throw new NullReferenceException(nameof(_socket));

            if (_socket.Connected)
                return Task.CompletedTask;

            return _socket.ConnectAsync(_endpoint);
        }

        public Task DisconnectAsync()
        {
            if (_socket == null)
                throw new NullReferenceException(nameof(_socket));

            return _socket.DisconnectAsync(true);
        }

        public Task SendAsync(INetworkPacket package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (_socket == null)
                throw new NullReferenceException(nameof(_socket));

            if (_serializer == null)
                throw new NullReferenceException(nameof(_serializer));

            using (var stream = new MemoryStream())
            {
                var length = _serializer.Serialize(package, stream);

                var state = new OrderedAsyncState();
                state.Client = _socket;
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

                var result = _socket.SendAsync(state.ReadEventArgs);

                return Task.CompletedTask;
            }
        }

        public bool ListenAsync(IPEndPoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            _socket.Bind(endpoint);
            _socket.Listen(1);

            var state = new OrderedAsyncState();
            state.ReadEventArgs.UserToken = state;
            state.ReadEventArgs.Completed += OnCompleted;
            state.ReceiveAsync = e =>
            {
                var receiveBuffer = new byte[MAX_BUFFER_SIZE];
                e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                return e.AcceptSocket.ReceiveAsync(e);
            };

            return _socket.AcceptAsync(state.ReadEventArgs);
        }

        private void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            var state = e.UserToken as OrderedAsyncState;

            state.ReceiveAsync(e);

            Debug.WriteLine($"state: {state.GetHashCode()}, socket: {e.AcceptSocket.GetHashCode()}");

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

                                        OnPacketReceived(new PacketEventArgs(packet, new TcpBinding(e.AcceptSocket, _serializer)));
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
            }
        }
    }
}
