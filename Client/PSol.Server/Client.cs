using System;
using System.Net.Sockets;
using Bindings;

namespace PSol.Server
{
    internal class Client
    {
        public int Index;
        public string IP;
        public TcpClient Socket;
        public NetworkStream Stream;
        private readonly HandleData _shd;
        public byte[] readBuff;

        public Client(HandleData shd)
        {
            _shd = shd;
        }

        public void Start()
        {
            Socket.SendBufferSize = 4096;
            Socket.ReceiveBufferSize = 4096;
            Stream = Socket.GetStream();
            Array.Resize(ref readBuff, Socket.ReceiveBufferSize);
            Stream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            try
            {
                int readBytes = Stream.EndRead(ar);
                if (readBytes <= 0)
                {
                    CloseSocket(Index); // Disconnect client when stream is <= 0 bytes
                    return;
                }

                byte[] newBytes = null;
                Array.Resize(ref newBytes, readBytes);
                Buffer.BlockCopy(readBuff, 0, newBytes, 0, readBytes);
                // Handle Data
                _shd.HandleNetworkMessages(Index, newBytes);
                Stream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch
            {
                CloseSocket(Index);
            }
        }

        private void CloseSocket(int index)
        {
            Console.WriteLine("Connection from " + IP + " has been terminated.");
            _shd.SendMessage(-1, Types.Player[index].Name + " has disconnected.", MessageColors.Notification);
            var db = new SQL();
            db.SaveGame(index);
            Socket.Close();
            Socket = null;
            Types.Player[index] = Types.Default;
        }

    }
}
