using System;
using System.Net;
using System.Net.Sockets;

namespace EasyTcp3.ClientUtils
{
    /// <summary>
    /// Class with Connect functions
    /// </summary>
    public static class ConnectUtil
    {
        /// <summary>
        /// Default timeout when timeout parameter is not specified 
        /// </summary>
        private const int DefaultTimeout = 5_000;

        /// <summary>
        /// Establishes connection with remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endPoint">endPoint of remote host</param>
        /// <param name="timeout">maximum time for connecting with remote host</param>
        /// <param name="socket">socket for EasyTcpClient, new one is create when null</param>
        /// <returns>determines whether the client connected successfully</returns> 
        public static bool Connect(this EasyTcpClient client, EndPoint endPoint, TimeSpan? timeout = null,
            Socket socket = null)
        {
            if (client == null) throw new ArgumentException("Could not connect: client is null");
            if (endPoint == null) throw new ArgumentException("Could not connect: endpoint is null");
            if (client.BaseSocket != null) throw new ArgumentException("Could not connect: client is still connected");

            try
            {
                client.BaseSocket = socket ?? client.Protocol.GetSocket(endPoint.AddressFamily);
                client.BaseSocket.ConnectAsync(endPoint).Wait(DefaultTimeout);

                if (client.BaseSocket.Connected && client.Protocol.OnConnect(client))
                {
                    client.FireOnConnect();
                    return true;
                }
            }
            catch
            {
                //Ignore exception, dispose (&disconnect) client and return false
            }

            client.Dispose();
            return false;
        }

        /// <summary>
        /// Establishes connection with remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host</param>
        /// <param name="port">port of remote host</param>
        /// <param name="timeout">maximum time for connecting with remote host</param>
        /// <param name="socket">socket for EasyTcpClient, new one is create when null</param>
        /// <returns>determines whether the client connected successfully</returns> 
        public static bool Connect(this EasyTcpClient client, IPAddress ipAddress, ushort port,
            TimeSpan? timeout = null, Socket socket = null)
            => client.Connect(new IPEndPoint(ipAddress, port), timeout, socket);

        /// <summary>
        /// Establishes connection with remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddress">ipAddress of remote host as string</param>
        /// <param name="port">port of remote host</param>
        /// <param name="timeout">maximum time for connecting with remote host</param>
        /// <param name="socket">socket for EasyTcpClient, new one is create when null</param>
        /// <returns>determines whether the client connected successfully</returns>  
        public static bool Connect(this EasyTcpClient client, string ipAddress, ushort port, TimeSpan? timeout = null,
            Socket socket = null)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress address))
                throw new ArgumentException("Could not connect: ipAddress is not a valid IPv4/IPv6 address");
            return client.Connect(address, port, timeout, socket);
        }
    }
}