using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace HenkTcp
{
    public class HenkTcpServer
    {
        private ServerListener _ServerListener;

        public event EventHandler<TcpClient> ClientConnected;
        public event EventHandler<TcpClient> ClientDisconnected;
        public event EventHandler<Message> DataReceived;
        public event EventHandler<Exception> OnError;

        private SymmetricAlgorithm _Algorithm;
        private byte[] _EncryptionKey;

        public void Start(int Port, int MaxConnections = 10000) { Start(IPAddress.Any, Port, MaxConnections); }
        public void Start(string Ip, int Port, int MaxConnections, int BufferSize = 1024) { Start(IPAddress.Parse(Ip), Port, MaxConnections, null, null, BufferSize); }
        public void Start(IPAddress Ip, int Port, int MaxConnections, int BufferSize = 1024) { Start(Ip, Port, MaxConnections, null, null, BufferSize); }
        public void Start(string Ip, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024) { Start(IPAddress.Parse(Ip), Port, MaxConnections, Algorithm, EncryptionKey, BufferSize); }
        public void Start(IPAddress Ip, int Port, int MaxConnections, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, int BufferSize = 1024)
        {
            if (_ServerListener != null) return;

            _ServerListener = new ServerListener(new TcpListener(Ip, Port), this, MaxConnections, BufferSize);
            _Algorithm = Algorithm;
            _EncryptionKey = EncryptionKey;
        }

        public List<TcpClient> ConnectedClients { get { if (_ServerListener == null) return null; return _ServerListener.ConnectedClients; } }

        public int ConnectedClientsCount { get { if (_ServerListener == null) return 0; return _ServerListener.ConnectedClients.Count; } }

        public TcpListener Listener { get { return _ServerListener.Listener; } }
        public bool IsRunning { get { return _ServerListener != null; } }
        public void Stop() { _ServerListener.Stop(); _ServerListener = null; }

        public void Broadcast(string Data) { Broadcast(Encoding.UTF8.GetBytes(Data)); }
        public void Broadcast(byte[] Data)
        {
            if (_ServerListener == null) return;
            Parallel.ForEach(_ServerListener.ConnectedClients, Client =>
            {
                try
                {
                    Client.GetStream().WriteAsync(Data, 0, Data.Length);
                }
                catch (Exception ex) { NotifyOnError(ex); }
            });
        }

        public void BroadcastEncrypted(string Data) { BroadcastEncrypted(Encoding.UTF8.GetBytes(Data)); }
        public void BroadcastEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return; }
            Broadcast(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        public void Write(TcpClient Client, string Data) { Write(Client, Encoding.UTF8.GetBytes(Data)); }
        public void Write(TcpClient Client, byte[] Data)
        {
            Client.GetStream().Write(Data, 0, Data.Length);
        }

        public void WriteEncrypted(TcpClient Client, string Data) { WriteEncrypted(Client, Encoding.UTF8.GetBytes(Data)); }
        public void WriteEncrypted(TcpClient Client, byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) { NotifyOnError(new Exception("Could not send message: Alghoritm/Key not set")); return; }
            Write(Client, Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }

        internal void NotifyClientConnected(TcpClient Client) { ClientConnected?.Invoke(this, Client); }
        internal void NotifyClientDisconnected(TcpClient Client) { ClientDisconnected?.Invoke(this, Client); }
        internal void NotifyDataReceived(byte[] Data, TcpClient Client) { DataReceived?.Invoke(this, new Message(Data, Client, _Algorithm, _EncryptionKey)); }
        internal void NotifyOnError(Exception ex) { if (OnError != null) OnError(this, ex); else throw ex; }
    }
}
