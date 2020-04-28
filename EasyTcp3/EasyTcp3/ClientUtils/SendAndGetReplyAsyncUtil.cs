using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTcp3.ClientUtils
{
    public static class SendAndGetReplyAsyncUtil
    {
        /// <summary>
        /// Send data (byte[]) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <returns>received data</returns>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, byte[] data, TimeSpan timeout)
        { 
            //TODO: Finish function
            if (timeout.Ticks.Equals(0)) throw new ArgumentException("Invalid Timeout.");

            Message reply = null;
            using var signal = new SemaphoreSlim(0,1);

            void Event(object sender, Message e)
            {
                reply = e;
                client.OnDataReceive -= Event;
                signal.Release();
            }

            client.OnDataReceive += Event;
            client.Send(data);

            await signal.WaitAsync(timeout);
            if (reply == null) client.OnDataReceive -= Event;
            return reply;
        }

        /// <summary>
        /// Send data (ushort) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ushort data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (short) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, short data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (uint) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, uint data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (int) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, int data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (ulong) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, ulong data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (long) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, long data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (double) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, double data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (bool) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        public static async Task<Message>
            SendAndGetReplyAsync(this EasyTcpClient client, bool data, TimeSpan timeout) =>
            await client.SendAndGetReplyAsync(BitConverter.GetBytes(data), timeout);

        /// <summary>
        /// Send data (string) to the remote host. Then wait for a reply from the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data">Data to send to server</param>
        /// <param name="timeout">Time to wait for a reply, if time expired: return null</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        public static async Task<Message> SendAndGetReplyAsync(this EasyTcpClient client, string data, TimeSpan timeout,
            Encoding encoding = null)
            => await client.SendAndGetReplyAsync((encoding ?? Encoding.UTF8).GetBytes(data), timeout);
    }
}