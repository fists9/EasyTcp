using System;
using System.Net.Sockets;
using System.Text;

namespace EasyTcp3.ClientUtils
{
    public static class SendUtil
    {
        /// <summary>
        /// Send data (byte[]) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid client</exception>
        public static void Send(this EasyTcpClient client, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not send data: Data array is empty");
            if (client == null || !client.IsConnected())
                throw new Exception("Could not send data: Client not connected or null");

            var message = new byte[2 + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) data.Length),
                0, message, 0, 2); //Write length of data to message.
            Buffer.BlockCopy(data, 0, message, 2, data.Length); //Write data to message.

            using var e = new SocketAsyncEventArgs();
            e.SetBuffer(message);
            client.BaseSocket.SendAsync(e);
        }

        /// <summary>
        /// Send data (ushort) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, ushort data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (short) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, short data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (uint) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, uint data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (int) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, int data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (ulong) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, ulong data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (long) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, long data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (double) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, double data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (bool) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        public static void Send(this EasyTcpClient client, bool data) => client.Send(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (string) to the remote host
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        public static void Send(this EasyTcpClient client, string data, Encoding encoding = null)
            => client.Send((encoding ?? Encoding.UTF8).GetBytes(data));
    }
}