using System;
using Bindings;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Client
{
	class HandleData
	{
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
			packets.Add((int)ServerPackets.SPulse, HandleServerPulse);
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
				// If the buffer is not in game or its ourselves, skip the update
				if (!inGame || i == GameLogic.PlayerIndex) continue;
				Types.Player[i].X = posX;
				Types.Player[i].Y = posY;
				Types.Player[i].Rotation = rot;
			}
		}
	}
}
