#pragma warning disable CS0436 // Type conflicts with imported type
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Bindings;
using Ninject;
using Ninject.Syntax;
using PSol.Data.Models;
using PSol.Data.Services.Interfaces;
using static Bindings.MessageColors;

namespace PSol.Server
{
    internal class Client
    {
        public int Index;
        public string IP;
        public TcpClient Socket;
        public NetworkStream Stream;
        private readonly ServerData _shd;
        private readonly IGameService _gameService;
        private readonly IUserService _userService;

        public Client(ServerData shd, IResolutionRoot kernel)
        {
            _shd = shd;
            _gameService = kernel.Get<IGameService>();
            _userService = kernel.Get<IUserService>();
        }

        public void Start()
        {
            Stream = Socket.GetStream();
            NetworkListen();
        }

        private void NetworkListen()
        {
            while (Socket != null && Socket.Connected && Stream.CanRead)
            {
                try
                {
                    var bytesData = new byte[4];
                    Stream.Read(bytesData, 0, 4);
                    var bytesInMessage = BitConverter.ToInt32(bytesData, 0);

                    var data = new byte[bytesInMessage];
                    Stream.Read(data, 0, bytesInMessage);
                    OnReceiveData(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"Remote connection forcibly closed...");
                    Console.WriteLine(e.Message);
                    CloseSocket(Index);
                }
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
            Console.WriteLine(@"Connection from " + IP + @" has been terminated.");
            var player = _userService.ActiveUsers.Find(p => p.Id == Types.PlayerIds[index]);
            _shd.SendMessage(-1, player.Name + @" has disconnected.", Notification);
            _gameService.SaveGame(new List<User> { player });
            ServerTCP.tempPlayer[index].inGame = false;
            ServerTCP.tempPlayer[index].receiving = false;
            Socket.Close();
            Socket = null;
            Types.PlayerIds[index] = null;
            _userService.ActiveUsers.Remove(player);
        }

    }
}
