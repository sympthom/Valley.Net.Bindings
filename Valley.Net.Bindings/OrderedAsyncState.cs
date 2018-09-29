using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Valley.Net.Bindings
{
    public sealed class OrderedAsyncState
    {
        private byte[] _buffer = new byte[0];

        public readonly SocketAsyncEventArgs ReadEventArgs = new SocketAsyncEventArgs();

        public byte[] Buffer { get { return _buffer; } }

        public int BytesReceived { get; private set; } = 0;

        public Socket Client { get; internal set; }

        internal Func<SocketAsyncEventArgs, bool> ReceiveAsync { get; set; }

        internal OrderedAsyncState()
        {

        }

        internal void Write(byte[] buffer, int bytesReceived)
        {
            var offset = BytesReceived;

            BytesReceived += bytesReceived;

            Array.Resize(ref _buffer, BytesReceived);
            Array.Copy(buffer, 0, _buffer, offset, bytesReceived);
        }

        public void Clear()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            Array.Resize(ref _buffer, 0);

            BytesReceived = 0;
        }
    }
}
