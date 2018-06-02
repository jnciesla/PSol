using System;
using Bindings;
using System.Net.Sockets;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Client
{
    class HandleData
    {
        public PacketBuffer buffer = new PacketBuffer();
        private ClientTCP ctcp;
        private delegate void Packet_(byte[] data);
        private static Dictionary<int, Packet_> packets;


        public void InitializeMesssages()
        {
            ctcp = new ClientTCP();
            packets = new Dictionary<int, Packet_>();
            packets.Add((int)ServerPackets.SMessage, HandleMessage);
            packets.Add((int)ServerPackets.SAckLogin, HandleLogin);
            packets.Add((int)ServerPackets.SPlayerData, DownloadData);
            packets.Add((int)ServerPackets.SAckRegister, GoodRegister);
        }

        public void HandleNetworkMessages(byte[] data)
        {
            int packetNum;
            PacketBuffer buffer = new PacketBuffer();

            buffer.AddBytes(data);
            packetNum = buffer.GetInteger();
            buffer.Dispose();

            if (packets.TryGetValue(packetNum, out Packet_ Packet))
                Packet.Invoke(data);
        }

        private void HandleMessage(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            Graphics.statusMessage = buffer.GetString();
        }

        private void HandleLogin(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            buffer.GetInteger(); // Index on server side
            MenuManager.Clear();
        }

        private void GoodRegister(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            buffer.GetInteger(); // Index on server side

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
            Types.Player[0].Rotation = rot;
            Types.Player[0].X = posX;
            Types.Player[0].Y = posY;
            MenuManager.Clear();
        }
    }
}
