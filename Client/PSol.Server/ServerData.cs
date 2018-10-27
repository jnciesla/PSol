#pragma warning disable CS0436 // Type conflicts with imported type
using System;
using Bindings;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework;
using Ninject;
using Ninject.Syntax;
using PSol.Data.Models;
using PSol.Data.Services.Interfaces;
using static Bindings.ClientPackets;
using static Bindings.MessageColors;
using static Bindings.ServerPackets;

namespace PSol.Server
{
    internal class ServerData
    {
        private delegate void Packet_(int index, byte[] data);
        private static Dictionary<int, Packet_> packets;
        private readonly IUserService _userService;
        private readonly IMobService _mobService;
        private readonly ICombatService _combatService;

        public ServerData(IResolutionRoot kernel)
        {
            _userService = kernel.Get<IUserService>();
            _mobService = kernel.Get<IMobService>();
            _combatService = kernel.Get<ICombatService>();
        }

        public void InitializeMessages()
        {
            Console.WriteLine(@"Initializing Network Packets");
            packets = new Dictionary<int, Packet_>
            {
                {(int) CLogin, HandleLogin},
                {(int) CRegister, HandleRegister},
                {(int) CPlayerData, RecvPlayer},
                {(int) CChat, ParseChat},
                {(int) CCombat, HandleCombat },
                {(int) CItemTransaction, HandleItemTransaction },
                {(int) CEquipItem, HandleEquip },
                {(int) CItemSale, HandleSale },
                {(int) CItemStack, HandleStack },
                {(int)CLootTransaction, HandleLoot }
            };
        }

        public void HandleNetworkMessages(int index, byte[] data)
        {
            var buffer = new PacketBuffer();

            buffer.AddBytes(data);
            var packetNum = buffer.GetInteger();
            buffer.Dispose();

            if (packets.TryGetValue(packetNum, out var Packet))
                Packet.Invoke(index, data);

        }

        private void HandleLogin(int index, byte[] data)
        {
            Console.WriteLine(@"Received login packet");
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var username = buffer.GetString();
            var password = buffer.GetString();

            if (!_userService.AccountExists(username))
            {
                SendMessage(index, "Username does not exist!", Warning);
                return;
            }

            if (!_userService.PasswordOK(username, password))
            {
                SendMessage(index, "Password incorrect!", Warning);
                return;
            }

            Types.Player[index] = _userService.LoadPlayer(username);
            ServerTCP.tempPlayer[index].inGame = true;
            XFerLoad(index);
            SendGalaxy(index);
            SendItems(index);
            SendNebulae(index);
            SendMessage(-1, Types.Player[index].Name + " has connected.", Notification);
            Globals.FullData = true;
            Console.WriteLine(username + @" logged in successfully.");
            ServerTCP.tempPlayer[index].receiving = true;
        }

        private void HandleRegister(int index, byte[] data)
        {
            Console.WriteLine(@"Received register packet");
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var username = buffer.GetString();
            var password = buffer.GetString();
            var exists = _userService.AccountExists(username);
            if (!exists)
            {
                Types.Player[index] = new User();
                Types.Player[index] = _userService.RegisterUser(username, password);
                AcknowledgeRegister(index);
            }
            else
            {
                SendMessage(index, "That username already exists!", Warning);
            }
        }

        private static void RecvPlayer(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var posX = buffer.GetFloat();
            var posY = buffer.GetFloat();
            var rot = buffer.GetFloat();
            Types.Player[index].Rotation = rot;
            Types.Player[index].X = posX;
            Types.Player[index].Y = posY;
        }

        public void SendData(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            var compressed = Compress(data);
            buffer.AddInteger(compressed.Length);
            buffer.AddBytes(compressed);
            try
            {
                ServerTCP.Clients[index].Stream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
            }
            catch
            {
                // Console.WriteLine(@"Unable to send packet- client disconnected");
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
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SMessage);
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

        public void AcknowledgeRegister(int index)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SAckRegister);
            buffer.AddInteger(index);
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void SendXP(int index)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SLevelUp);
            buffer.AddInteger(Types.Player[index].Level);
            buffer.AddInteger(Types.Player[index].Exp);
            buffer.AddInteger((int)Transactions.CheckLevel(Types.Player[index].Level));     // XP requirement for current level
            buffer.AddInteger((int)Transactions.CheckLevel(Types.Player[index].Level + 1)); // XP requirement for next level
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void XFerLoad(int index)
        {
            var buffer = new PacketBuffer();
            var player = Types.Player[index];
            buffer.AddInteger((int)SPlayerData);
            buffer.AddInteger(index);
            buffer.AddString(player.Id);
            buffer.AddString(player.Name);
            buffer.AddFloat(player.X);
            buffer.AddFloat(player.Y);
            buffer.AddFloat(player.Rotation);
            buffer.AddInteger(player.Health);
            buffer.AddInteger(player.MaxHealth);
            buffer.AddInteger(player.Shield);
            buffer.AddInteger(player.MaxShield);
            buffer.AddString(player.Rank);
            buffer.AddInteger(player.Credits);
            buffer.AddInteger(player.Exp);
            buffer.AddInteger(player.Level);
            buffer.AddInteger(player.Weap1Charge);
            buffer.AddInteger(player.Weap2Charge);
            buffer.AddInteger(player.Weap3Charge);
            buffer.AddInteger(player.Weap4Charge);
            buffer.AddInteger(player.Weap5Charge);
            buffer.AddArray(player.Inventory.ToArray());
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void UpdatePlayer(int index)
        {
            var buffer = new PacketBuffer();
            var player = Types.Player[index];
            buffer.AddInteger((int)SPlayerUpdate);
            buffer.AddInteger(player.Health);
            buffer.AddInteger(player.MaxHealth);
            buffer.AddInteger(player.Shield);
            buffer.AddInteger(player.MaxShield);
            buffer.AddInteger(player.Exp);
            buffer.AddInteger(player.Weap1Charge);
            buffer.AddInteger(player.Weap2Charge);
            buffer.AddInteger(player.Weap3Charge);
            buffer.AddInteger(player.Weap4Charge);
            buffer.AddInteger(player.Weap5Charge);
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void PreparePulseBroadcast()
        {
            var mobRange = 2000;
            _combatService.CycleArrays();
            for (var i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (ServerTCP.Clients[i].Socket != null && ServerTCP.tempPlayer[i].inGame && ServerTCP.tempPlayer[i].receiving)
                {
                    var buffer = new PacketBuffer();
                    buffer.AddInteger((int)SPulse);
                    buffer.AddBytes(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
                    for (var j = 1; j < Constants.MAX_PLAYERS; j++)
                    {
                        buffer.AddString(Types.Player[j].Id);
                        buffer.AddFloat(Types.Player[j].X);
                        buffer.AddFloat(Types.Player[j].Y);
                        buffer.AddFloat(Types.Player[j].Rotation);
                        buffer.AddInteger(Types.Player[j].Health);
                        buffer.AddInteger(Types.Player[j].MaxHealth);
                        buffer.AddInteger(Types.Player[j].Shield);
                        buffer.AddInteger(Types.Player[j].MaxShield);
                        buffer.AddBytes(BitConverter.GetBytes(ServerTCP.tempPlayer[j].inGame));
                    }
                    var minX = (int)Types.Player[i].X - mobRange;
                    var minY = (int)Types.Player[i].Y - mobRange;
                    var maxX = (int)Types.Player[i].X + mobRange;
                    var maxY = (int)Types.Player[i].Y + mobRange;
                    buffer.AddArray(_mobService.GetMobs(minX, maxX, minY, maxY).ToArray());
                    buffer.AddArray(_combatService.GetCombats((int)Types.Player[i].X, (int)Types.Player[i].Y).ToArray());
                    buffer.AddArray(Globals.Inventory.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY).ToArray());
                    buffer.AddArray(Globals.Loot.Where(L => L.X >= minX && L.X <= maxX && L.Y >= minY && L.Y <= maxY && L.Owner == Types.Player[i].Id).ToArray());
                    SendData(i, buffer.ToArray());
                    buffer.Dispose();
                }
            }
        }

        public void PrepareStaticBroadcast()
        {
            Globals.FullData = false;
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SFullData);
            for (var i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                buffer.AddString(Types.Player[i].Name ?? "");
            }
            BroadcastData(buffer.ToArray());
            buffer.Dispose();
        }

        public void SendInventory(int index, bool newLoot = false)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SInventory);
            buffer.AddArray(Types.Player[index].Inventory.ToArray());
            buffer.AddInteger(Types.Player[index].Credits);
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void ParseChat(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var str = buffer.GetString();
            if (str.ToLower().StartsWith("/c"))
            {
                RelayChat(index, str.Substring(3));
            }
        }

        public void RelayChat(int index, string str)
        {
            var buffer = new PacketBuffer();
            var newString = Types.Player[index].Name + ": " + str;
            buffer.AddInteger((int)SMessage);
            buffer.AddInteger((int)Chat);
            buffer.AddString(newString);
            BroadcastData(buffer.ToArray());
            buffer.Dispose();
        }

        public void HandleCombat(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var targetId = buffer.GetString();
            var weapon = buffer.GetInteger();
            var weaponId = Types.Player[index].Inventory.FirstOrDefault(i => i.Slot == weapon);
            if (weaponId == null) return;
            var WEAPON = Globals.Items.FirstOrDefault(w => w.Id == weaponId?.ItemId);
            if (WEAPON == null) return;
            switch (weaponId.Slot)
            {
                case 7:
                    Types.Player[index].Weap1Charge = 0;
                    break;
                case 8:
                    Types.Player[index].Weap2Charge = 0;
                    break;
                case 9:
                    Types.Player[index].Weap3Charge = 0;
                    break;
                case 10:
                    Types.Player[index].Weap4Charge = 0;
                    break;
                case 11:
                    Types.Player[index].Weap5Charge = 0;
                    break;

            }
            var combat = _combatService.DoAttack(targetId, Types.Player[index].Id, WEAPON, Types.Player.ToList());
            var targetPlayer = Types.Player.ToList().FirstOrDefault(p => p?.Id == combat.TargetId);

            if (combat.TargetId == "dead")
            {
                Transactions.CreateLoot(index, new Vector2(combat.TargetX, combat.TargetY));
                var newLevel = Transactions.GiveXP(index, 1000);

                if (newLevel == -1) { SendMessage(index, "You have already reached the maximum level!", Announcement); }
                if (newLevel > 0)
                {
                    SendMessage(index, "Congratulations, you have reached level " + (Types.Player[index].Level) + "!", Announcement);
                }
                SendXP(index);
            }

            if (targetPlayer == null) return;
            targetPlayer.Shield -= combat.WeaponDamage;
            if (targetPlayer.Shield >= 0) return;
            targetPlayer.Health += targetPlayer.Shield;
            targetPlayer.Shield = 0;
        }

        public void HandleItemTransaction(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var id = buffer.GetString();
            var recipient = buffer.GetString();
            buffer.Dispose();
            if (recipient == Types.Player[index].Id)
            {
                if (Transactions.ReceiveFromGlobal(id, index))
                    SendInventory(index);
                else
                    SendMessage(index, "The object no longer exists", Minor);
            }
            else
            {
                if (!Transactions.TransferItem(id, index, recipient)) return;
                SendInventory(index);
                if (recipient == "X") return;
                var recipientIndex = Array.FindIndex(Types.Player, row => row.Id == recipient);
                SendInventory(recipientIndex);
            }
        }

        public void HandleEquip(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var id = buffer.GetString();
            var destSlot = buffer.GetInteger();
            buffer.Dispose();
            var result = Transactions.EquipItem(id, index, destSlot);
            switch (result)
            {
                case 1:
                    UpdatePlayer(index);
                    SendInventory(index);
                    break;
                case 2:
                    SendMessage(index, "Invalid item.", Warning);
                    break;
                case 3:
                    SendMessage(index, "No room in the cargo hold to unequip that item", Minor);
                    break;
            }
        }

        public void HandleSale(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var mode = buffer.GetInteger();
            var id = buffer.GetString();
            var qty = buffer.GetInteger();
            buffer.Dispose();
            var result = mode == 1 ? Transactions.BuyItem(id, qty, index) : Transactions.SellItem(id, qty, index);

            switch (result)
            {
                case 1:
                    SendMessage(index, "Not enough credits to buy that item.", Minor);
                    break;
                case 2:
                    SendMessage(index, "Invalid item.", Warning);
                    break;
                case 3:
                    SendMessage(index, "No room in the cargo hold to buy that item.", Minor);
                    break;
                default:
                    SendInventory(index);
                    break;
            }
        }

        public void HandleStack(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var from = buffer.GetString();
            var to = buffer.GetString();
            buffer.Dispose();
            if (Transactions.StackItems(from, to, index) == -1)
            {
                SendMessage(index, "Invalid item, item cannot stack, or stack is full.", Warning);
            };
            SendInventory(index);
        }

        public void HandleLoot(int index, byte[] data)
        {
            var buffer = new PacketBuffer();
            buffer.AddBytes(data);
            buffer.GetInteger();
            var type = buffer.GetInteger();
            var lootId = buffer.GetString();
            var itemIndex = buffer.GetInteger();
            if (type == 1)
            {
                Globals.Loot.Remove(Globals.Loot.FirstOrDefault(l => l.Id == lootId));
            }
            else
            {
                if (Transactions.CollectLoot(lootId, itemIndex, index))
                    SendInventory(index, true);
            }
        }

        public void SendGalaxy(int index)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SGalaxy);
            buffer.AddArray(Globals.Galaxy.ToArray());
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public void SendNebulae(int index = -1)
        {
            if (index == -1)
            {
                for (var i = 0; i < Constants.MAX_PLAYERS; i++)
                {
                    if (ServerTCP.Clients[i].Socket != null && ServerTCP.tempPlayer[i].inGame &&
                        ServerTCP.tempPlayer[i].receiving)
                    {
                        var buffer = new PacketBuffer();
                        buffer.AddInteger((int)SNebulae);
                        buffer.AddArray(Globals.Nebulae.ToArray());
                        SendData(i, buffer.ToArray());
                        buffer.Dispose();
                    }
                }
            }
            else
            {
                var buffer = new PacketBuffer();
                buffer.AddInteger((int)SNebulae);
                buffer.AddArray(Globals.Nebulae.ToArray());
                SendData(index, buffer.ToArray());
                buffer.Dispose();
            }
        }

        public void SendItems(int index)
        {
            var buffer = new PacketBuffer();
            buffer.AddInteger((int)SItems);
            buffer.AddArray(Globals.Items.ToArray());
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }

        public byte[] Compress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Optimal))
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
