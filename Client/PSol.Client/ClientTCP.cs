using System;
using System.Net.Sockets;
using Bindings;
using Microsoft.Xna.Framework;

namespace PSol.Client
{
    internal class ClientTCP
    {
        public TcpClient PlayerSocket;
        public static NetworkStream myStream;
        private HandleData chd;
        private byte[] asyncBuff;
        private bool connected;
        public bool isOnline;

        public void ConnectToServer()
        {
            // Already connected.  Destroy connection and re-connect
            if(PlayerSocket != null)
            {
                if(PlayerSocket.Connected || connected)
                {
                    PlayerSocket.Close();
                    PlayerSocket = null;
                }
            }

            PlayerSocket = new TcpClient();
            chd = new HandleData();
            PlayerSocket.ReceiveBufferSize = 4096;
            PlayerSocket.SendBufferSize = 4096;
            PlayerSocket.NoDelay = false;
            Array.Resize(ref asyncBuff, 8192);
            PlayerSocket.BeginConnect("127.0.0.1", 8000, ConnectCallback, PlayerSocket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                PlayerSocket.EndConnect(ar);
                isOnline = true;
            } catch
            {
                isOnline = false;
                ConnectToServer();
            }
            if(PlayerSocket.Connected == false)
            {
                connected = false;
            } else {
                PlayerSocket.NoDelay = true;
                myStream = PlayerSocket.GetStream();
                myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
                connected = true;
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            int byteAmt = myStream.EndRead(ar);
            byte[] myBytes = null;
            Array.Resize(ref myBytes, byteAmt);
            Buffer.BlockCopy(asyncBuff, 0, myBytes, 0, byteAmt);

            if(byteAmt == 0)
            {
                // Destroy game; empty packet received
                return;
            }

            // Handle network packets
            chd.HandleNetworkMessages(myBytes);
            myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
        }

        public void SendData(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
            buffer.Dispose();
        }

        public void SendLogin()
        {
            InterfaceGUI.AddChats("Logging in...", Color.DarkOliveGreen);
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)ClientPackets.CLogin);
            buffer.AddString(Globals.loginUsername);
            buffer.AddString(Globals.loginPassword);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void SendRegister()
        {
            InterfaceGUI.AddChats("Registering new user...", Color.DarkOliveGreen);
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)ClientPackets.CRegister);
            buffer.AddString(Globals.registerUsername);
            buffer.AddString(Globals.registerPassword);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public void XFerPlayer()
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)ClientPackets.CPlayerData);
            buffer.AddFloat(Types.Player[GameLogic.PlayerIndex].X);
            buffer.AddFloat(Types.Player[GameLogic.PlayerIndex].Y);
            buffer.AddFloat(Types.Player[GameLogic.PlayerIndex].Rotation);
            SendData(buffer.ToArray());
            buffer.Dispose();

        }

        public void SendChat(string message)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)ClientPackets.CChat);
            buffer.AddString(message);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

    }
}
