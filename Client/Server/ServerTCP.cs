using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Bindings;

namespace Server
{
    class ServerTCP
    {
        public static TempPlayer[] tempPlayer = new TempPlayer[Constants.MAX_PLAYERS];
        public static Client[] Clients = new Client[Constants.MAX_PLAYERS];
        public TcpListener ServerSocket;

        public void InitializeNetwork()
        {
            Console.WriteLine("Initializing Server Network...");
            ServerSocket = new TcpListener(IPAddress.Any, 8000);
            ServerSocket.Start();
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            TcpClient client = ServerSocket.EndAcceptTcpClient(ar);
            client.NoDelay = false;
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);

            for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
            {
                if (Clients[i].Socket == null)
                {
                    Clients[i].Socket = client;
                    Clients[i].Index = i;
                    Clients[i].IP = client.Client.RemoteEndPoint.ToString();
                    Clients[i].Start();
                    Console.WriteLine("Connection received from " + Clients[i].IP);
                    return;
                }
            }
        }
    }
}
