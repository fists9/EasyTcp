using System;
using System.Linq;
using EasyEncrypt2;
using EasyTcp3;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp.Encryption.Protocols.Tcp
{
    /// <summary>
    /// Protocol that determines the length of a message based on a small header
    /// Header is an ushort as byte[] with length of incoming message
    ///
    /// All data is encrypted before sending to remote host
    /// All received data is decrypted before triggering OnDataReceive
    /// </summary>
    public class EncryptedPrefixLengthProtocol : PrefixLengthProtocol
    {
        /// <summary>
        /// Encrypter instance, used to encrypt and decrypt data 
        /// </summary>
        protected readonly EasyEncrypt Encrypter;

        /// <summary></summary>
        /// <param name="encrypter"></param>
        public EncryptedPrefixLengthProtocol(EasyEncrypt encrypter) : base()
            => Encrypter = encrypter;

        /// <summary>
        /// Create a new encrypted message from 1 or multiple byte arrays
        /// returned data will be send to remote host
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public override byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var dataLength = data.Sum(t => t?.Length ?? 0);
            if (dataLength == 0)
                throw new ArgumentException("Could not create message: Data array only contains empty arrays");
            byte[] mergedData = new byte[dataLength];

            // Add data to message
            int offset = 0;
            foreach (var d in data)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, mergedData, offset, d.Length);
                offset += d.Length;
            }

            // Encrypt and create message
            var encryptedData = Encrypter.Encrypt(mergedData);
            var message = new byte[2 + encryptedData.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) encryptedData.Length), 0, message, 0, 2);
            Buffer.BlockCopy(encryptedData, 0, message, 2, encryptedData.Length);
            return message;
        }

        /// <summary>
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new EncryptedPrefixLengthProtocol(Encrypter);
        
        /// <summary>
        /// Handle received data, trigger event and set new bufferSize determined by the header 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="receivedBytes">ignored</param>
        /// <param name="client"></param>
        public override void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            ushort dataLength = 2;

            if (!ReceivingLength)
            {
                try
                {
                    client.DataReceiveHandler(new Message(client.Buffer, client).Decrypt(Encrypter));
                }
                catch { OnDecryptionError(client); }
            }
            else  dataLength = BitConverter.ToUInt16(client.Buffer, 0);
            
            ReceivingLength = !ReceivingLength;
            if (dataLength == 0) client.Dispose();
            else BufferSize = dataLength;
        }

        /*
         * Internal
         */
        
        /// <summary>
        /// Handle decryption error 
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnDecryptionError(EasyTcpClient client) => client.Dispose();
    }
}