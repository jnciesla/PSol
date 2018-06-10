using System;
using Bindings;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Client
{
    internal class HandleData
    {
        private ClientTCP ctcp;
        private delegate void Packet_(byte[] data);
        private static Dictionary<int, Packet_> packets;


        public void InitializeMesssages()
        {
            ctcp = new ClientTCP();
            packets = new Dictionary<int, Packet_>
            {
                {(int) ServerPackets.SMessage, HandleMessage},
                {(int) ServerPackets.SAckLogin, HandleLogin},
                {(int) ServerPackets.SPlayerData, DownloadData},
                {(int) ServerPackets.SAckRegister, GoodRegister},
                {(int) ServerPackets.SPulse, HandleServerPulse}
            };
        }

        public void HandleNetworkMessages(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();

            buffer.AddBytes(data);
            var packetNum = buffer.GetInteger();
            buffer.Dispose();

            if (packets.TryGetValue(packetNum, out Packet_ Packet))
                Packet.Invoke(data);
        }

        private void HandleMessage(byte[] data)
        {
            Color color = Color.White;
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            int colorCode = buffer.GetInteger();
            if (colorCode == 2) { color = Color.DarkRed; }
            if (colorCode == 3) { color = Color.DarkGoldenrod; }
            if (colorCode == 4) { color = Color.DarkGray; }
            InterfaceGUI.AddChats(buffer.GetString(), color);
        }

        private void HandleLogin(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.PlayerIndex = buffer.GetInteger(); // Index on server side
            MenuManager.Clear();
        }

        private void GoodRegister(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.PlayerIndex = buffer.GetInteger(); // Index on server side

            ctcp.SendLogin();
            MenuManager.Clear();
        }

        private void DownloadData(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            float posX = buffer.GetFloat();
            float posY = buffer.GetFloat();
            float rot = buffer.GetFloat();
            Types.Player[GameLogic.PlayerIndex].Rotation = rot;
            Types.Player[GameLogic.PlayerIndex].X = posX;
            Types.Player[GameLogic.PlayerIndex].Y = posY;
            MenuManager.Clear();
        }

        private void HandleServerPulse(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger(); // Packet Type
            var timestamp = BitConverter.ToInt64(buffer.GetBytes(8), 0);
            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                var posX = buffer.GetFloat();
                var posY = buffer.GetFloat();
                var rot = buffer.GetFloat();
                var inGame = BitConverter.ToBoolean(buffer.GetBytes(1), 0);
                if (inGame == false)
                {
                    Types.Player[i] = Types.Default;
                }
                // If the buffer is not in game or its ourselves, skip the update
                if (!inGame || i == GameLogic.PlayerIndex) continue;
                Types.Player[i].X = posX;
                Types.Player[i].Y = posY;
                Types.Player[i].Rotation = rot;
            }
        }
    }
}
