using System;
using System.Net;
using System.Net.Sockets;
using EasyTcp3.ClientUtils.Internal;

namespace EasyTcp3.ClientUtils
{
    public static class ConnectUtil
    {
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of host</param>
        /// <param name="port">port of host</param>
        /// <param name="timeout">max timeout of the connection</param>
        /// <returns>true = client connected successful, false = connecting failed</returns>
        public static bool Connect(this EasyTcpClient client, IPAddress ipAddress, ushort port, TimeSpan timeout)
        {
            if (client == null) throw new ArgumentException("Could not connect: client is null");
            if (ipAddress == null) throw new ArgumentException("Could not connect: ipAddress is null");
            if (port == 0) throw new ArgumentException("Could not connect: Invalid port");
            if (client.BaseSocket != null) throw new ArgumentException("Could not connect: client is still connected");

            try
            {
                client.BaseSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.BaseSocket.BeginConnect(ipAddress, port, null, null).AsyncWaitHandle.WaitOne(timeout);
                if (client.BaseSocket.Connected)
                {
                    client.FireOnConnect();
                    OnReceiveUtil.StartListening(client);
                    return true;
                }
            }
            catch { }

            client.Dispose();
            return false;
        }

        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of host</param>
        /// <param name="port">port of host</param>
        /// <returns>true = client connected successful, false = connecting failed</returns>
        public static bool Connect(this EasyTcpClient client, IPAddress ipAddress, ushort port)
            => Connect(client, ipAddress, port, TimeSpan.FromMilliseconds(DefaultTimeout));

        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of host as string</param>
        /// <param name="port">port of host</param>
        /// <param name="timeout">max timeout of the connection</param>
        /// <returns>true = client connected successful, false = connecting failed</returns>
        /// <exception cref="ArgumentException">ipAddress is not a valid IPv4/IPv6 address</exception>
        public static bool Connect(this EasyTcpClient client, string ipAddress, ushort port, TimeSpan timeout)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not connect: ipAddress is not a valid IPv4/IPv6 address");
            return client.Connect(address, port, timeout);
        }

        /// <summary>
        /// Establishes a connection to a remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of host as string</param>
        /// <param name="port">port of host</param>
        /// <returns>true = client connected successful, false = connecting failed</returns>
        public static bool Connect(this EasyTcpClient client, string ipAddress, ushort port)
            => Connect(client, ipAddress, port, TimeSpan.FromMilliseconds(DefaultTimeout));
    }
}