using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Valley.Net.Bindings
{
    public static class SocketExtensions
    {
        /// <summary>
        /// Asynchronously waits to accept an incoming connection.
        /// </summary>
        /// <returns>Task&lt;Socket&gt;</returns>
        public static Task<Socket> AcceptAsync(this Socket value)
        {
            return Task.Factory.FromAsync<Socket>(value.BeginAccept(null, null), value.EndAccept);
        }

        /// <summary>
        /// Asynchronously waits to accept an incoming connection.
        /// </summary>
        /// <param name="receiveSize">The number of bytes to accept from the sender</param>
        /// <returns>Task&lt;Socket&gt;</returns>
        public static Task<Socket> AcceptAsync(this Socket s, int receiveSize)
        {
            return Task.Factory.FromAsync<Socket>(s.BeginAccept(receiveSize, null, null), s.EndAccept);
        }

        /// <summary>
        /// Asynchronously waits to accept an incoming connection.
        /// </summary>
        /// <param name="acceptSocket">The accepted System.Net.Sockets.Socket object, nullable.</param>
        /// <param name="receiveSize">The number of bytes to accept from the sender</param>
        /// <returns>Task&lt;Socket&gt;</returns>
        public static Task<Socket> AcceptAsync(this Socket value, Socket acceptSocket, int receiveSize)
        {
            return Task.Factory.FromAsync<Socket>(value.BeginAccept(acceptSocket, receiveSize, null, null), value.EndAccept);
        }

        /// <summary>
        /// Makes an asynchronous request for a remote host connection
        /// </summary>
        /// <param name="remoteEp">
        ///     A System.Net.EndPoint that represents the remote host
        /// </param>
        /// <returns>Task</returns>
        public static Task ConnectAsync(this Socket value, EndPoint remoteEp)
        {
            return Task.Factory.FromAsync(value.BeginConnect(remoteEp, null, null), value.EndConnect);
        }

        /// <summary>
        /// Makes an asynchronous request for a remote host connection. 
        /// The host is specified by an IPAddress and port number.
        /// </summary>
        /// <param name="address">The IPAddress of the remote host</param>
        /// <param name="port">The port of the remote host</param><
        /// <returns>Task</returns>
        public static Task ConnectAsync(this Socket value, IPAddress address, int port)
        {
            return Task.Factory.FromAsync(value.BeginConnect(address, port, null, null), value.EndConnect);
        }

        /// <summary>
        /// Makes an asynchronous request for a remote host connection.
        /// The host is specified by an IPAddress array and a port number.
        /// </summary>
        /// <param name="address">Array if IpAddresses of the remote host</param>
        /// <param name="port">The port of the remote host</param>
        /// <returns>Task</returns>
        public static Task ConnectAsync(this Socket value, IPAddress[] addresses, int port)
        {
            return Task.Factory.FromAsync(value.BeginConnect(addresses, port, null, null), value.EndConnect);
        }

        /// <summary>
        /// Makes an asynchronous request for a remote host connection.
        /// The host is specified by a hostname and port number
        /// </summary>
        /// <param name="host">The name of the remote host</param>
        /// <param name="port">The port of the remote host</param>
        /// <returns>Task</returns>
        public static Task ConnectAsync(this Socket value, string host, int port)
        {
            return Task.Factory.FromAsync(value.BeginConnect(host, port, null, null), value.EndConnect);
        }

        /// <summary>
        /// Asynchronously requests to disconnect from a remote endpoint.
        /// </summary>
        /// <param name="resuseSocket">Re-use the socket after connection is closed</param>
        /// <returns>Task</returns>
        public static Task DisconnectAsync(this Socket value, bool resuseSocket)
        {
            return Task.Factory.FromAsync(value.BeginDisconnect(resuseSocket, null, null), value.EndDisconnect);
        }

        /// <summary>
        /// Asynchronously receives data from a connected System.Net.Sockets.Socket
        /// </summary>
        /// <param name="buffers">
        /// An array of type System.Byte that is the storage location for the received data
        /// </param>
        /// <param name="flags">
        /// A bitwise combination of the System.Net.Sockets.SocketFlags values
        /// </param>
        /// <returns>Task&lt;int&gt;</returns>
        public static Task<int> ReceiveAsync(this Socket value, IList<ArraySegment<byte>> buffers, SocketFlags flags)
        {
            return Task.Factory.FromAsync<int>(value.BeginReceive(buffers, flags, null, null), value.EndReceive);
        }

        /// <summary>
        /// Asynchronously receives data from a connected System.Net.Sockets.Socket
        /// </summary>
        /// <param name="buffer">
        /// An array of type System.Byte that is the storage location for the received data
        /// </param>
        /// <param name="offset">
        /// The zero-based position in the buffer parameter at which to store the received data
        /// </param>
        /// <param name="size">The number of bytes to receive</param>
        /// <param name="flags">A bitwise combination of the System.Net.Sockets.SocketFlags values</param>
        /// <returns>Task&lt;int&gt;</returns>
        public static Task<int> ReceiveAsync(this Socket value, byte[] buffer, int offset, int size, SocketFlags flags)
        {
            return Task.Factory.FromAsync<int>(value.BeginReceive(buffer, offset, size, flags, null, null), value.EndReceive);
        }

        /// <summary>
        /// Sends data asynchronously to a connected System.Net.Sockets.Socket
        /// </summary>
        /// <param name="buffers">
        /// An array of type System.Byte that contains the data to send
        /// </param>
        /// <param name="flags">A bitwise combination of the System.Net.Sockets.SocketFlags values</param>
        /// <returns>Task&lt;int&gt;</returns>
        public static Task<int> SendAsync(this Socket value, IList<ArraySegment<byte>> buffers, SocketFlags flags)
        {
            return Task.Factory.FromAsync<int>(value.BeginSend(buffers, flags, null, null), value.EndSend);
        }

        /// <summary>
        /// Sends data asynchronously to a connected System.Net.Sockets.Socket
        /// </summary>
        /// <param name="buffer">
        /// An array of type System.Byte that contains the data to send
        /// </param>
        /// <param name="offset">
        /// The zero-based position in the buffer parameter at which to begin sending data
        /// </param>
        /// <param name="size">The number of bytes to send</param>
        /// <param name="flags">A bitwise combination of the System.Net.Sockets.SocketFlags values</param>
        /// <returns>Task&lt;int&gt;</returns>
        public static Task<int> SendAsync(this Socket value, byte[] buffer, int offset, int size, SocketFlags flags)
        {
            return Task.Factory.FromAsync<int>(value.BeginSend(buffer, offset, size, flags, null, null), value.EndSend);
        }

        /// <summary>
        /// Sends the file fileName to a connected Socket object using the UseDefaultWorkerThread flag.
        /// </summary>
        /// <param name="fileName">
        /// A string that contains the path and name of the file to send. This parameter can be null.
        /// </param>
        /// <returns>Task</returns>
        public static Task SendFileAsync(this Socket value, string fileName)
        {
            return Task.Factory.FromAsync(value.BeginSendFile(fileName, null, null), value.EndSendFile);
        }

        /// <summary>
        /// Sends the file fileName to a connected Socket object using the UseDefaultWorkerThread flag.
        /// </summary>
        /// <param name="fileName">
        /// A string that contains the path and name of the file to send. This parameter can be null.
        /// </param>
        /// <param name="preBuffer">
        /// A Byte array that contains data to be sent before the file is sent. This parameter can be null.
        /// </param>
        /// <param name="postBuffer">
        /// A Byte array that contains data to be sent after the file is sent. This parameter can be null.
        /// </param>
        /// <param name="flags">A bitwise combination of TransmitFileOptions values.</param>
        /// <returns>Task</returns>
        public static Task SendFileAsync(this Socket value, string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
        {
            return Task.Factory.FromAsync(value.BeginSendFile(fileName, preBuffer, postBuffer, flags, null, null), value.EndSendFile);
        }
    }
}
