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

        public event EventHandler<PacketEventArgs> PacketReceived;

        public SerialBinding(IPacketSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _serialPort = new SerialPort();
        }

        public Task ConnectAsync(IPEndPoint endpoint)
        {
            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            // Allow the user to set the appropriate properties.
            //_serialPort.PortName = SetPortName(_serialPort.PortName);
            //_serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            //_serialPort.Parity = SetPortParity(_serialPort.Parity);
            //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            //_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            //_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            _serialPort.Open();

            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            _serialPort.Close();

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

            using (var stream = new MemoryStream())
            {
                var length = _serializer.Serialize(package, stream);

                _serialPort.Write(stream.ToArray(), 0, length);

                return Task.CompletedTask;
            }
        }

        public bool ListenAsync(IPEndPoint endpoint)
        {
            if (_serialPort == null)
                throw new NullReferenceException(nameof(_serialPort));

            return _serialPort.IsOpen;
        }
    }
}
