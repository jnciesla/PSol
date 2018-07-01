using System;
using Bindings;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using PSol.Data.Models;

namespace PSol.Client
{
    internal class HandleData
    {
        private ClientTCP ctcp;
        private delegate void Packet_(byte[] data);
        private static Dictionary<int, Packet_> packets;


        public void InitializeMessages()
        {
            ctcp = new ClientTCP();
            packets = new Dictionary<int, Packet_>
            {
                {(int) ServerPackets.SMessage, HandleMessage},
                {(int) ServerPackets.SPlayerData, DownloadData},
                {(int) ServerPackets.SAckRegister, GoodRegister},
                {(int) ServerPackets.SPulse, HandleServerPulse},
                {(int) ServerPackets.SFullData, GetStaticPulse},
                {(int) ServerPackets.SGalaxy, HandleGalaxy }
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
            MenuManager.Clear(2);
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
                Id = buffer.GetString(),
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
            MenuManager.Clear(1);
            InterfaceGUI.AddChats("User data downloaded.", Color.DarkOliveGreen);

        }

        private void GetStaticPulse(byte[] data)
        {
            InterfaceGUI.AddChats("Existing connections downloaded.", Color.DarkOliveGreen);
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
            Globals.serverTime = BitConverter.ToInt64(buffer.GetBytes(8), 0);
            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                var Id = buffer.GetString();
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
                Types.Player[i].Id = Id;
                Types.Player[i].X = X;
                Types.Player[i].Y = Y;
                Types.Player[i].Rotation = Rotation;
                Types.Player[i].Health = Health;
                Types.Player[i].MaxHealth = MaxHealth;
                Types.Player[i].Shield = Shield;
                Types.Player[i].MaxShield = MaxShield;
            }

            GameLogic.LocalMobs = buffer.GetList<Mob>();
            buffer.Dispose();
        }

        private void HandleGalaxy(byte[] data)
        {
            InterfaceGUI.AddChats("Galaxy downloaded.", Color.DarkOliveGreen);
            PacketBuffer buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.Galaxy = buffer.GetList<Star>();
            InterfaceGUI.PopulateMap();
            buffer.Dispose();
        }

        //
        // XML DATA
        //
        private static XmlDocument XML = new XmlDocument();
        private static string Root = "PSOL";
        private static string Filename = "settings.xml";

        public static void NewXMLDoc()
        {
            XmlTextWriter xmlTextWriter = new XmlTextWriter(Filename, Encoding.UTF8);
            //Write a blank XML doc

            var xml = xmlTextWriter;
            {
                xml.WriteStartDocument(true);
                xml.WriteStartElement(Root);
                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
        }

        public static void WriteToXml(string selection, string name, string value)
        {
            var check = XML.SelectSingleNode(Root + "/" + selection);
            if (check == null)
            {
                XML.DocumentElement.AppendChild(XML.CreateElement(selection));
            }

            XmlNode xmlNode = XML.SelectSingleNode(Root + "/" + selection + "/Element[@Name='" + name + "']");
            if (xmlNode == null)
            {
                XmlElement element = XML.CreateElement("Element");
                element.SetAttribute("Value", value);
                element.SetAttribute("Name", name);
                XML.DocumentElement[selection].AppendChild(element);
            }
            else
            {
                //Update node
                xmlNode.Attributes["Value"].Value = value;
                xmlNode.Attributes["Name"].Value = name;
            }
        }

        public static void LoadXml()
        {
            if (!File.Exists(Filename))
            {
                NewXMLDoc();
            }
            XML.Load(Filename);
        }

        public static string ReadFromXml(string selection, string name, string defaultValue = "")
        {
            var xmlNode = XML.SelectSingleNode(Root + "/" + selection + "/Element[@Name='" + name + "']");
            if (xmlNode == null)
            {
                WriteToXml(selection, name, defaultValue);
                return defaultValue;
            }
            return xmlNode.Attributes["Value"].Value;
        }

        public static void CloseXml(bool save)
        {
            if (save)
            {
                XML.Save(Filename);
            }

            XML = null;
        }

    }
}
