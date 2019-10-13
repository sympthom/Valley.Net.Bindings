using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Valley.Net.Bindings.Serial
{
    public sealed class SerialBinding : IEndPointBinding
    {
        private readonly IPacketSerializer _serializer;
        private readonly SerialPort _serialPort;
        public readonly Func<SerialPort, IPacketSerializer, INetworkPacket> _readPort;
        private Thread _readThread;
        private volatile bool _continue = false;

        public event EventHandler<PacketEventArgs> PacketReceived;

        public SerialBinding(SerialPort serialPort, Func<SerialPort, IPacketSerializer, INetworkPacket> readPort, IPacketSerializer serializer)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
            _readPort = readPort ?? throw new ArgumentNullException(nameof(readPort));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task ConnectAsync()
        {
            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            _serialPort.Open();
            _continue = _serialPort.IsOpen;

            _readThread = new Thread(Read);
            _readThread.Start();

            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            _serialPort.Close();
            _continue = _serialPort.IsOpen;

            _readThread.Join();

            return Task.CompletedTask;
        }

        public Task SendAsync(INetworkPacket package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (_serializer == null)
                throw new NullReferenceException(nameof(_serializer));

            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            if (!_serialPort.IsOpen)
                throw new Exception("Serial port is not open.");

            using (var stream = new MemoryStream())
            {
                var length = _serializer.Serialize(package, stream);

                _serialPort.Write(stream.ToArray(), 0, length);

                return Task.CompletedTask;
            }
        }

        public bool ListenAsync()
        {
            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            _serialPort.Open();
            _continue = _serialPort.IsOpen;

            _readThread = new Thread(Read);
            _readThread.Start();

            return _serialPort.IsOpen;
        }

        private void Read()
        {
            if (_readPort == null)
                throw new NullReferenceException(nameof(_readPort));

            if (_serializer == null)
                throw new NullReferenceException(nameof(_serializer));

            while (_continue)
            {
                var packet = _readPort(_serialPort, _serializer);

                if (packet == null)
                    continue;

                OnPacketReceived(new PacketEventArgs(packet, this));
            }
        }

        private void OnPacketReceived(PacketEventArgs e)
        {
            PacketReceived?.Invoke(this, e);
        }
    }
}
