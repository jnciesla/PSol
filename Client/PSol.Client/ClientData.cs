#pragma warning disable CS0436 // Type conflicts with imported type
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Bindings;
using Microsoft.Xna.Framework;
using PSol.Data.Models;
using static Bindings.ServerPackets;

namespace PSol.Client
{
    internal class ClientData
    {
        private ClientTCP ctcp;
        private delegate void Packet_(byte[] data);
        private static Dictionary<int, Packet_> packets;

        public void InitializeMessages()
        {
            ctcp = new ClientTCP();
            packets = new Dictionary<int, Packet_>
            {
                {(int) SMessage, HandleMessage},
                {(int) SPlayerData, DownloadData},
                {(int) SAckRegister, GoodRegister},
                {(int) SPulse, HandleServerPulse},
                {(int) SFullData, GetStaticPulse},
                {(int) SGalaxy, HandleGalaxy },
                {(int) SItems, HandleItems },
                {(int) SInventory, HandleInventory },
                {(int)SPlayerUpdate, UpdateData },
                {(int)SLevelUp, HandleExperience },
                {(int)SNebulae, HandleNebulae }
            };
        }

        public void HandleNetworkMessages(byte[] data)
        {
            var buffer = new PacketBuffer();

            buffer.AddBytes(data);
            var packetNum = buffer.GetInteger();
            buffer.Dispose();

            if (packets.TryGetValue(packetNum, out var Packet))
                Packet.Invoke(data);
        }

        private static void HandleMessage(byte[] data)
        {
            Color color;
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var colorCode = buffer.GetInteger();
            switch (colorCode)
            {
                case (int)MessageColors.Warning:
                    color = Color.DarkRed;
                    break;
                case (int)MessageColors.Notification:
                    color = Color.DarkGoldenrod;
                    break;
                case (int)MessageColors.Minor:
                    color = Color.DarkOliveGreen;
                    break;
                case (int)MessageColors.Announcement:
                    color = Color.SkyBlue;
                    break;
                default:
                    color = Color.DarkGray;
                    break;
            }

            InterfaceGUI.AddChats(buffer.GetString(), color);
        }

        private void GoodRegister(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.PlayerIndex = buffer.GetInteger(); // Index on server side
            buffer.Dispose();
            ctcp.SendLogin();
            MenuManager.Clear(2);
            InterfaceGUI.AddChats(@"Registration successful.", Color.DarkOliveGreen);
        }

        private static void DownloadData(byte[] data)
        {
            var buffer = new PacketBuffer();
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
                MaxShield = buffer.GetInteger(),
                Rank = buffer.GetString(),
                Credits = buffer.GetInteger(),
                Exp = buffer.GetInteger(),
                Level = buffer.GetInteger(),
                Weap1Charge = buffer.GetInteger(),
                Weap2Charge = buffer.GetInteger(),
                Weap3Charge = buffer.GetInteger(),
                Weap4Charge = buffer.GetInteger(),
                Weap5Charge = buffer.GetInteger(),
                Inventory = buffer.GetList<Inventory>()
            };
            buffer.Dispose();
            MenuManager.Clear(1);
            Globals.graphicsChange = Globals.Fullscreen;
            InterfaceGUI.AddChats(@"User data downloaded.", Color.DarkOliveGreen);
        }

        private static void UpdateData(byte[] data)
        {
            if (GameLogic.PlayerIndex < 0) return;
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Health = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].MaxHealth = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Shield = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].MaxShield = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Exp = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Weap1Charge = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Weap2Charge = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Weap3Charge = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Weap4Charge = buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Weap5Charge = buffer.GetInteger();
            buffer.Dispose();
        }

        private static void GetStaticPulse(byte[] data)
        {
            InterfaceGUI.AddChats(@"Existing connections downloaded.", Color.DarkOliveGreen);
            // Someone new connected so this is all the data we don't need updating every 100ms
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var numPlayers = buffer.GetInteger();
            for (var i = 0; i != numPlayers; i++)
            {
                var index = buffer.GetInteger();
                Types.Player[index].Name = buffer.GetString();
            }
            buffer.Dispose();
        }

        private static void HandleServerPulse(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger(); // Packet Type
            var numPlayers = buffer.GetInteger();
            Globals.serverTime = BitConverter.ToInt64(buffer.GetBytes(8), 0);
            for (var i = 0; i != numPlayers; i++)
            {
                var index = buffer.GetInteger();
                var Id = buffer.GetString();
                var X = buffer.GetFloat();
                var Y = buffer.GetFloat();
                var Rotation = buffer.GetFloat();
                var Health = buffer.GetInteger();
                var MaxHealth = buffer.GetInteger();
                var Shield = buffer.GetInteger();
                var MaxShield = buffer.GetInteger();
                var inGame = BitConverter.ToBoolean(buffer.GetBytes(1), 0);
                // If the buffer is not ourselves, skip the update - need to do not in game characters to remove logged out users
                if (index == GameLogic.PlayerIndex) continue;
                Types.Player[index].Id = Id;
                Types.Player[index].X = X;
                Types.Player[index].Y = Y;
                Types.Player[index].Rotation = Rotation;
                Types.Player[index].Health = Health;
                Types.Player[index].MaxHealth = MaxHealth;
                Types.Player[index].Shield = Shield;
                Types.Player[index].MaxShield = MaxShield;
            }
            GameLogic.LocalMobs = buffer.GetList<Mob>();
            GameLogic.LocalCombat = buffer.GetList<Combat>();
            GameLogic.LocalLoot = buffer.GetList<Inventory>();
            GameLogic.RealLoot = buffer.GetList<Loot>();
            GameLogic.WatchCombat();
            buffer.Dispose();
        }

        private static void HandleInventory(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            Types.Player[GameLogic.PlayerIndex].Inventory = buffer.GetList<Inventory>();
            Types.Player[GameLogic.PlayerIndex].Credits = buffer.GetInteger();
            Globals.newInventory = true;
            buffer.Dispose();
        }

        private static void HandleGalaxy(byte[] data)
        {
            InterfaceGUI.AddChats(@"Galaxy downloaded.", Color.DarkOliveGreen);
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.Galaxy = buffer.GetList<Star>();
            GameLogic.CollectPlanets();
            InterfaceGUI.PopulateMap();
            buffer.Dispose();
        }

        private static void HandleNebulae(byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.Nebulae = buffer.GetList<Nebula>();
            buffer.Dispose();
        }

        private static void HandleItems(byte[] data)
        {
            InterfaceGUI.AddChats(@"Item dictionary downloaded.", Color.DarkOliveGreen);
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            GameLogic.Items = buffer.GetList<Item>();
            buffer.Dispose();
        }

        private static void HandleExperience(byte[] data)
        {
            var random = new Random();
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var tempLevel = buffer.GetInteger();
            var tempXP = buffer.GetInteger();
            var increase = tempXP - Types.Player[GameLogic.PlayerIndex].Exp;
            if (tempLevel > Types.Player[GameLogic.PlayerIndex].Level)
            {
                Game1.LevelUp.Create(new Vector2(Types.Player[GameLogic.PlayerIndex].X - 20 + random.Next(40), Types.Player[GameLogic.PlayerIndex].Y - 20 + random.Next(40)));
            }
            Types.Player[GameLogic.PlayerIndex].Level = tempLevel;
            Types.Player[GameLogic.PlayerIndex].Exp = tempXP;
            Globals.minXP = buffer.GetInteger();     // XP requirement for current level
            Globals.maxXP = buffer.GetInteger();     // XP requirement for next level
            var tempDamage = new DamageText(increase.ToString(), new Vector2(Types.Player[GameLogic.PlayerIndex].X - 20 + random.Next(40), Types.Player[GameLogic.PlayerIndex].Y - 20 + random.Next(40)), 2);
            Game1.DamageTexts.Add(tempDamage);
        }

        //
        // XML DATA
        //
        private static XmlDocument XML = new XmlDocument();
        private const string Root = "PSOL";
        private const string Filename = "settings.xml";

        public static void NewXMLDoc()
        {
            var xmlTextWriter = new XmlTextWriter(Filename, Encoding.UTF8);
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
                XML.DocumentElement?.AppendChild(XML.CreateElement(selection));
            }

            var xmlNode = XML.SelectSingleNode(Root + "/" + selection + "/Element[@Name='" + name + "']");
            if (xmlNode == null)
            {
                var element = XML.CreateElement("Element");
                element.SetAttribute("Value", value);
                element.SetAttribute("Name", name);
                XML.DocumentElement?[selection]?.AppendChild(element);
            }
            else
            {
                //Update node
                if (xmlNode.Attributes == null) return;
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
            if (xmlNode?.Attributes != null)
                return xmlNode.Attributes["Value"].Value;
            WriteToXml(selection, name, defaultValue);
            return defaultValue;
        }

        public static void CloseXml(bool save)
        {
            if (save)
                XML.Save(Filename);
            XML = null;
        }
    }
}
