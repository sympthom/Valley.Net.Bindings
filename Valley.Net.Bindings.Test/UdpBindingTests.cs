using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;
using System.Net;
using Valley.Net.Bindings.Udp;

namespace Valley.Net.Bindings.Test
{
    [TestClass]
    public sealed class UdpBindingTests
    {
        [TestMethod]
        public async Task Client_Should_Receive_Package_When_Server_Responds()
        {
            var serverReceivedEvent = new AutoResetEvent(false);
            var clientReceivedEvent = new AutoResetEvent(false);
            
            var server = new UdpBinding(new IPEndPoint(IPAddress.Any, 1700), new TestSerializer());
            server.IoCompleted += (sender, e) => Debug.WriteLine(e.LastOperation);
            server.PacketReceived += async (sender, e) =>
            {
                serverReceivedEvent.Set();

                await e.Binding.SendAsync(e.Packet);
            };

            var result = server.ListenAsync();

            Assert.IsTrue(result);

            var client = new UdpBinding(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1700), new TestSerializer());
            client.IoCompleted += (sender, e) => Debug.WriteLine(e.LastOperation);
            client.PacketReceived += (sender, e) => clientReceivedEvent.Set();

            await client.ConnectAsync();

            await client.SendAsync(new TestPacket());

            Assert.IsTrue(serverReceivedEvent.WaitOne(TimeSpan.FromSeconds(3)));

            Assert.IsTrue(clientReceivedEvent.WaitOne(TimeSpan.FromSeconds(3)));
        }

        [TestMethod]
        public async Task Error_Callback_Should_Be_Executed_When_Serializer_Throws_An_Exception()
        {
            var serverErrorEvent = new AutoResetEvent(false);

            var server = new UdpBinding(new IPEndPoint(IPAddress.Any, 1701), new ExceptionSerializer());
            server.IoCompleted += (sender, e) => Debug.WriteLine(e.LastOperation);
            server.Error += (sender, e) =>
            {
                serverErrorEvent.Set();
            };

            var result = server.ListenAsync();

            Assert.IsTrue(result);

            var client = new UdpBinding(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1701), new TestSerializer());
            client.IoCompleted += (sender, e) => Debug.WriteLine(e.LastOperation);

            await client.ConnectAsync();

            await client.SendAsync(new TestPacket());

            Assert.IsTrue(serverErrorEvent.WaitOne(TimeSpan.FromSeconds(3)));
        }
    }
}
