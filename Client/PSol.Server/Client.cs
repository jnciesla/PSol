﻿#pragma warning disable CS0436 // Type conflicts with imported type
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

        public Client(ServerData shd, IResolutionRoot kernel)
        {
            _shd = shd;
            _gameService = kernel.Get<IGameService>();
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
                catch
                {
                    Console.WriteLine(@"Remote connection forcibly closed...");
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
            _shd.SendMessage(-1, Types.Player[index].Name + @" has disconnected.", Notification);
            _gameService.SaveGame(new List<User> { Types.Player[index] });
            ServerTCP.tempPlayer[index].inGame = false;
            ServerTCP.tempPlayer[index].receiving = false;
            Socket.Close();
            Socket = null;
            Types.Player[index] = Types.Default;
        }

    }
}
