using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Bindings;
using Ninject;
using PSol.Data.Models;
using PSol.Data.Services.Interfaces;

namespace PSol.Server
{
    internal class Client
    {
        public int Index;
        public string IP;
        public TcpClient Socket;
        public NetworkStream Stream;
        private readonly HandleData _shd;
        private readonly IGameService _gameService;

        public Client(HandleData shd, IKernel kernel)
        {
            _shd = shd;
            _gameService = kernel.Get<IGameService>();
        }

        public void Start()
        {
            Socket.SendBufferSize = 4096;
            Socket.ReceiveBufferSize = 4096;
            Stream = Socket.GetStream();
            NetworkListen();
        }

        private void NetworkListen()
        {
            while (Socket.Connected)
            {
                do
                {
                    Thread.Sleep(20);
                } while (!Stream.DataAvailable);

                var bytesData = new byte[4];
                Stream.Read(bytesData, 0, 4);
                var bytesInMessage = BitConverter.ToInt32(bytesData, 0);

                var data = new byte[bytesInMessage];
                Stream.Read(data, 0, bytesInMessage);
                OnReceiveData(data);
            }
        }

        private void OnReceiveData(byte[] data)
        {
            if (data.Length <= 0)
            {
                CloseSocket(Index); // Disconnect client when stream is <= 0 bytes
                return;
            }
            // Handle Data
            _shd.HandleNetworkMessages(Index, data);
        }

        private void CloseSocket(int index)
        {
            Console.WriteLine("Connection from " + IP + " has been terminated.");
            _shd.SendMessage(-1, Types.Player[index].Name + " has disconnected.", MessageColors.Notification);
            _gameService.SaveGame(new List<User> { Types.Player[index] });
            ServerTCP.tempPlayer[index].inGame = false;
            ServerTCP.tempPlayer[index].receiving = false;
            Stream.Close();
            Stream = null;
            Socket.Close();
            Socket = null;
            Types.Player[index] = Types.Default;
        }

    }
}
