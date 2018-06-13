using System;
using Bindings;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PSol.Data.Models;

namespace PSol.Client
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
                {(int) ServerPackets.SPlayerData, DownloadData},
                {(int) ServerPackets.SAckRegister, GoodRegister},
                {(int) ServerPackets.SPulse, HandleServerPulse},
                {(int) ServerPackets.SFullData, GetStaticPulse}
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
            Color color = Color.DarkGray;
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            int colorCode = buffer.GetInteger();
            if (colorCode == 2) { color = Color.DarkRed; }
            if (colorCode == 3) { color = Color.DarkGoldenrod; }
            if (colorCode == 4) { color = Color.DarkOliveGreen; }
            InterfaceGUI.AddChats(buffer.GetString(), color);
        }

        private void GoodRegister(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.PlayerIndex = buffer.GetInteger(); // Index on server side
            buffer.Dispose();
            ctcp.SendLogin();
            MenuManager.Clear();
            InterfaceGUI.AddChats("Registration successful.", Color.DarkOliveGreen);
        }

        private void DownloadData(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.PlayerIndex = buffer.GetInteger(); // Index on server side
            var i = GameLogic.PlayerIndex;
            Types.Player[i] = new User
            {
                Name = buffer.GetString(),
                X = buffer.GetFloat(),
                Y = buffer.GetFloat(),
                Rotation = buffer.GetFloat(),
                Health = buffer.GetInteger(),
                MaxHealth = buffer.GetInteger(),
                Shield = buffer.GetInteger(),
                MaxShield = buffer.GetInteger()
            };

            buffer.Dispose();
            MenuManager.Clear();
            InterfaceGUI.AddChats("User data downloaded.", Color.DarkOliveGreen);
            
        }

        private void GetStaticPulse(byte[] data)
        {
            InterfaceGUI.AddChats("Static data downloaded.", Color.DarkOliveGreen);
            // Someone new connected so this is all the data we don't need updating every 100ms
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                Types.Player[i].Name = buffer.GetString();
            }
            buffer.Dispose();
        }

        private void HandleServerPulse(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger(); // Packet Type
            var timestamp = BitConverter.ToInt64(buffer.GetBytes(8), 0);
            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                var X = buffer.GetFloat();
                var Y = buffer.GetFloat();
                var Rotation = buffer.GetFloat();
                var Health = buffer.GetInteger();
                var MaxHealth = buffer.GetInteger();
                var Shield = buffer.GetInteger();
                var MaxShield = buffer.GetInteger();
                var inGame = BitConverter.ToBoolean(buffer.GetBytes(1), 0);
                // If the buffer is not in game or its ourselves, skip the update
                if (!inGame || i == GameLogic.PlayerIndex) continue;
                Types.Player[i].X = X;
                Types.Player[i].Y = Y;
                Types.Player[i].Rotation = Rotation;
                Types.Player[i].Health = Health;
                Types.Player[i].MaxHealth = MaxHealth;
                Types.Player[i].Shield = Shield;
                Types.Player[i].MaxShield = MaxShield;
            }
            buffer.Dispose();
        }
    }
}
