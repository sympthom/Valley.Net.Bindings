using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Valley.Net.Bindings.Udp
{
    public sealed class UdpBroadcastBinding
    {
        private readonly int _port;
        private readonly IPacketSerializer _serializer;

        public UdpBroadcastBinding(int port, IPacketSerializer serializer)
        {
            _port = port;
            _serializer = serializer;
        }

        public void Send(INetworkPacket package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (_serializer == null)
                throw new NullReferenceException(nameof(_serializer));

            var broadcastDomain = new IPEndPoint(IPAddress.Broadcast, _port);

            using (var stream = new MemoryStream())
            {
                var length = _serializer.Serialize(package, stream);

                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    switch (nic.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.Wireless80211:
                        case NetworkInterfaceType.Ethernet:
                            {
                                if (nic.Supports(NetworkInterfaceComponent.IPv4) == false)
                                    continue;

                                var adapterProperties = nic.GetIPProperties();

                                foreach (var ua in adapterProperties.UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
                                {
                                    try
                                    {
                                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                                        socket.ReceiveTimeout = 200;
                                        socket.Bind(new IPEndPoint(ua.Address, _port));

                                        var result = socket.SendTo(stream.ToArray(), broadcastDomain);

                                        int BUFFER_SIZE_ANSWER = 1024;
                                        byte[] bufferAnswer = new byte[BUFFER_SIZE_ANSWER];

                                        do
                                        {
                                            try
                                            {
                                                socket.Receive(bufferAnswer);

                                                var returnData = Encoding.ASCII.GetString(bufferAnswer, 0, bufferAnswer.Length);

                                                if (true)
                                                {

                                                }
                                                //DevicesList.Add(GetMyDevice(bufferAnswer)); //Corresponding functions to get the devices information. Depends on the application.
                                            }
                                            catch { break; }

                                        } while (socket.ReceiveTimeout != 0); //fixed receive timeout for each adapter that supports our broadcast

                                        socket.Close();
                                    }
                                    catch { }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
