#pragma warning disable CS0436 // Type conflicts with imported type
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Bindings;
using Microsoft.Xna.Framework;
using static Bindings.ClientPackets;

namespace PSol.Client
{
    internal class ClientTCP
    {
        public TcpClient PlayerSocket;
        public static NetworkStream myStream;
        private HandleData chd;
        private bool connected;
        public bool isOnline;

        public void ConnectToServer()
        {
            // Already connected.  Destroy connection and re-connect
            if (PlayerSocket != null)
            {
                if (PlayerSocket.Connected || connected)
                {
                    PlayerSocket.Close();
                    PlayerSocket = null;
                }
            }

            PlayerSocket = new TcpClient();
            chd = new HandleData();
            PlayerSocket.NoDelay = false;
            PlayerSocket.BeginConnect("127.0.0.1", 8000, ConnectCallback, PlayerSocket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                PlayerSocket.EndConnect(ar);
                isOnline = true;
            }
            catch
            {
                isOnline = false;
                ConnectToServer();
            }
            if (PlayerSocket.Connected == false)
            {
                connected = false;
            }
            else
            {
                PlayerSocket.NoDelay = true;
                connected = true;
                myStream = PlayerSocket.GetStream();
                NetworkListen();
            }
        }

        private void NetworkListen()
        {
            while (PlayerSocket.Connected)
            {
                do
                {
                    Thread.Sleep(20);
                } while (PlayerSocket.Connected && !myStream.DataAvailable);

                if (!PlayerSocket.Connected) break;
                var bytesData = new byte[4];
                myStream.Read(bytesData, 0, 4);
                var bytesInMessage = BitConverter.ToInt32(bytesData, 0);

                var data = new byte[bytesInMessage];
                myStream.Read(data, 0, bytesInMessage);
                OnReceive(data);
            }
        }

        private void OnReceive(byte[] data)
        {
            if (data.Length == 0)
            {
                // Destroy game; empty packet received
                return;
            }

            // Handle network packets
            chd.HandleNetworkMessages(Decompress(data));
        }

        public void SendData(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger(data.Length);
            buffer.AddBytes(data);
            myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
            buffer.Dispose();
        }

        public void SendLogin()
        {
            InterfaceGUI.AddChats("Logging in...", Color.DarkOliveGreen);
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)CLogin);
            buffer.AddString(Globals.loginUsername);
            buffer.AddString(Globals.loginPassword);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void SendRegister()
        {
            InterfaceGUI.AddChats("Registering new user...", Color.DarkOliveGreen);
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)CRegister);
            buffer.AddString(Globals.registerUsername);
            buffer.AddString(Globals.registerPassword);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void XFerPlayer()
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)CPlayerData);
            buffer.AddFloat(Types.Player[GameLogic.PlayerIndex].X);
            buffer.AddFloat(Types.Player[GameLogic.PlayerIndex].Y);
            buffer.AddFloat(Types.Player[GameLogic.PlayerIndex].Rotation);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void UpdateInventory()
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int) CPlayerItem);
            buffer.AddArray(Types.Player[GameLogic.PlayerIndex].Inventory.ToArray());
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void SendChat(string message)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)CChat);
            buffer.AddString(message);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void SendCombat(string targetId, string weaponId)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)CCombat);
            buffer.AddString(targetId);
            buffer.AddString(weaponId);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public byte[] Compress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return mso.ToArray();
            }
        }

        public byte[] Decompress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return mso.ToArray();
            }
        }

    }
}
