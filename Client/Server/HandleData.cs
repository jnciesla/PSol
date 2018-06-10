using System;
using Bindings;
using System.Collections.Generic;

namespace Server
{
    internal class HandleData
    {
        private delegate void Packet_(int index, byte[] data);
        private static Dictionary<int, Packet_> packets;
        private SQL db = new SQL();

        public void InitializeMesssages()
        {
            Console.WriteLine("Initializing Network Packets...");
            packets = new Dictionary<int, Packet_>();
            packets.Add((int)ClientPackets.CLogin, HandleLogin);
            packets.Add((int)ClientPackets.CRegister, HandleRegister);
            packets.Add((int)ClientPackets.CPlayerData, RecvPlayer);
            packets.Add((int)ClientPackets.CChat, RelayChat);
        }

        public void HandleNetworkMessages(int index, byte[] data)
        {
            int packetNum;
            PacketBuffer buffer = new PacketBuffer();

            buffer.AddBytes(data);
            packetNum = buffer.GetInteger();
            buffer.Dispose();

            if (packets.TryGetValue(packetNum, out Packet_ Packet))
                Packet.Invoke(index, data);

        }

        private void HandleLogin(int index, byte[] data)
        {
            Console.WriteLine("Received login packet");
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            string username = buffer.GetString();
            string password = buffer.GetString();

            if (!db.AccountExists(username))
            {
                SendMessage(index, "Username does not exist!", MessageColors.Warning);
                return;
            }

            if (!db.PasswordOK(username, password))
            {
                SendMessage(index, "Password incorrect!", MessageColors.Warning);
                return;
            }

            db.LoadPlayer(index, username);
            SendMessage(index, "Querying database...", MessageColors.System);
            ServerTCP.tempPlayer[index].inGame = true;
            XFerLoad(index);
            Console.WriteLine(username + " logged in successfully.");
            AcknowledgeLogin(index);
        }

        private void HandleRegister(int index, byte[] data)
        {
            Console.WriteLine("Received register packet");
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            string username = buffer.GetString();
            string password = buffer.GetString();
            bool exists = db.AccountExists(username);
            if (!exists)
            {
                db.register(index, username, password);
                AcknowledgeRegister(index);
            }
            else
            {
                SendMessage(index, "That username already exists!", MessageColors.Warning);
            }
        }

        private static void RecvPlayer(int index, byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            float posX = buffer.GetFloat();
            float posY = buffer.GetFloat();
            float rot = buffer.GetFloat();
            Types.Player[index].Rotation = rot;
            Types.Player[index].X = posX;
            Types.Player[index].Y = posY;
        }

        public void SendData(int index, byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            try
            {
                ServerTCP.Clients[index].Stream
                    .BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send packet- client disconnected");
                Console.WriteLine(ex);
            }

            buffer.Dispose();
        }

        public void BroadcastData(byte[] data)
        {
            for (var i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (ServerTCP.Clients[i].Socket != null && ServerTCP.tempPlayer[i].inGame)
                {
                    SendData(i, data);
                }
            }
        }

        public void SendMessage(int index, string message, MessageColors color)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddInteger((int)ServerPackets.SMessage);
            buffer.AddInteger((int)color);
            buffer.AddString(message);
            // Use index -1 to broadcast from server to all players
            if (index != -1)
            {
                SendData(index, buffer.ToArray());
            }
            else
            {
                BroadcastData(buffer.ToArray());
            }

            buffer.Dispose();
        }

        public void AcknowledgeLogin(int index)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddInteger((int)ServerPackets.SAckLogin);
            buffer.AddInteger(index);
            SendData(index, buffer.ToArray());
            buffer.Dispose();
            string message = Types.Player[index].Name + " has connected.";
            SendMessage(-1, message, MessageColors.Notification);
        }

        public void AcknowledgeRegister(int index)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddInteger((int)ServerPackets.SAckRegister);
            buffer.AddInteger(index);
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void XFerLoad(int index)
        {
            SendMessage(index, "Downloading user data...", MessageColors.System);
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddInteger((int)ServerPackets.SPlayerData);
            buffer.AddFloat(Types.Player[index].X);
            buffer.AddFloat(Types.Player[index].Y);
            buffer.AddFloat(Types.Player[index].Rotation);
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void PreparePulseBroadcast()
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)ServerPackets.SPulse);
            buffer.AddBytes(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
            for (var i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                buffer.AddFloat(Types.Player[i].X);
                buffer.AddFloat(Types.Player[i].Y);
                buffer.AddFloat(Types.Player[i].Rotation);
                buffer.AddBytes(BitConverter.GetBytes(ServerTCP.tempPlayer[i].inGame));
            }
            BroadcastData(buffer.ToArray());
            buffer.Dispose();
        }

        public void RelayChat(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var oldString = buffer.GetString();
            var newString = Types.Player[index].Login + ": " + oldString;
            buffer.Dispose();
            buffer.AddInteger((int)ServerPackets.SMessage);
            buffer.AddInteger((int)MessageColors.Chat);
            buffer.AddString(newString);
            BroadcastData(buffer.ToArray());
            buffer.Dispose();
        }

    }
}
