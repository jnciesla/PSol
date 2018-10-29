#pragma warning disable CS0436 // Type conflicts with imported type
using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;
using Microsoft.Xna.Framework;
using PSol.Data.Models;
using Inventory = PSol.Data.Models.Inventory;

namespace PSol.Server
{
    internal class Transactions
    {
        public static bool TransferItem(string item, User player, User recipient)
        {
            var temp = player.Inventory.ToList().FirstOrDefault(i => i.Id == item);

            if (recipient != null)
            {
                var stacked = FindAvailableStack(recipient.Id, temp);
                switch (stacked)
                {
                    case 0:
                        return true;
                    case -1:
                        return false;
                }

                temp.Quantity = stacked;
                var newSlot = FindOpenSlot(recipient);
                var newInv = new Inventory()
                {
                    Id = Guid.NewGuid().ToString(),
                    Dropped = DateTime.UtcNow,
                    ItemId = temp.ItemId,
                    Quantity = temp.Quantity,
                    Slot = newSlot,
                    UserId = recipient.Id,
                    X = 0,
                    Y = 0
                };
                recipient.Inventory.Add(newInv);
                player.Inventory.Remove(temp);
                return true;
            }
            else
            {
                var newInv = new Inventory()
                {
                    Id = Guid.NewGuid().ToString(),
                    Dropped = DateTime.UtcNow,
                    ItemId = temp.ItemId,
                    Quantity = temp.Quantity,
                    Slot = 0,
                    UserId = "X",
                    X = player.X,
                    Y = player.Y
                };
                Globals.Inventory.Add(newInv);
                player.Inventory.Remove(temp);
                return true;
            }
        }

        public static bool ReceiveFromGlobal(string ID, User player)
        {
            var temp = Globals.Inventory.FirstOrDefault(i => i.Id == ID);
            if (temp != null)
            {
                Globals.Inventory.Remove(temp);
                var stacked = FindAvailableStack(player.Id, temp);
                if (stacked == 0) return true;
                if (stacked == -1) return false;
                temp.Quantity = stacked;
                var newSlot = FindOpenSlot(player);
                if (newSlot != -1)
                {
                    var newInv = new Inventory()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Dropped = DateTime.UtcNow,
                        ItemId = temp.ItemId,
                        Quantity = temp.Quantity,
                        Slot = newSlot,
                        UserId = player.Id,
                        X = 0,
                        Y = 0
                    };
                    player.Inventory.Add(newInv);
                }
                else
                {
                    var newInv = new Inventory()
                    {
                        Id = null,
                        Dropped = DateTime.UtcNow,
                        ItemId = temp.ItemId,
                        Quantity = temp.Quantity,
                        Slot = 0,
                        UserId = "X",
                        X = temp.Y,
                        Y = temp.Y
                    };
                    Globals.Inventory.Add(newInv);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public static int FindOpenSlot(User recipient)
        {
            for (var n = 0; n < 60; n++)
            {
                if (recipient?.Inventory.FirstOrDefault(inv => inv.Slot == n + 101)?.ItemId == null)
                {
                    return n + 101;
                };
            }
            return -1;
        }

        public static int FindAvailableStack(string player, Inventory itm)
        {
            var inv = Types.Player.FirstOrDefault(p => p.Id == player)?.Inventory;
            if (inv == null) return -1;
            foreach (var stack in inv.Where(i => i.ItemId == itm.ItemId && i.Slot > 100))
            {
                if (itm.Quantity <= 0) continue;
                var capacity = 999 - stack.Quantity;
                if (itm.Quantity <= capacity)
                {
                    stack.Quantity += itm.Quantity;
                    itm.Quantity = 0;
                    inv.Remove(itm);
                }
                else
                {
                    stack.Quantity += capacity;
                    itm.Quantity -= capacity;
                }
            }
            return itm.Quantity > 0 ? itm.Quantity : 0;
        }

        public static int EquipItem(string id, User player, int destSlot)
        {
            string OUT = null;
            string IN = null;
            var inv = player.Inventory.FirstOrDefault(i => i.Id == id);
            if (inv == null) return 2;
            var itm = Globals.Items.FirstOrDefault(i => i.Id == inv?.ItemId);
            if (itm == null) return 2;
            switch (destSlot)
            {
                case 0: // Equip
                    var installed = player.Inventory.FirstOrDefault(s => s.Slot == itm.Slot); // Check if something is already installed
                    if (installed != null)
                    {
                        installed.Slot = inv.Slot;
                        OUT = installed.ItemId;
                    }
                    player.Inventory.FirstOrDefault(i => i.Id == id).Slot = itm.Slot;
                    IN = inv.ItemId;
                    break;
                case -1: // Unequip
                    // TODO: Make it so unequipping an item tries to stack it first
                    var newSlot = FindOpenSlot(player);
                    if (newSlot == -1)
                    {
                        return 3;
                    }
                    OUT = inv.ItemId;
                    player.Inventory.FirstOrDefault(i => i.Id == id).Slot = newSlot;
                    break;
                default: // Move or install specific slot
                    if (destSlot > 100)
                    {
                        player.Inventory.FirstOrDefault(i => i.Slot == inv.Slot).Slot = destSlot;
                    }
                    else
                    {
                        var _installed = player.Inventory.FirstOrDefault(s => s.Slot == destSlot); // Check if something is already installed
                        if (_installed != null)
                        {
                            _installed.Slot = inv.Slot;
                            OUT = _installed.ItemId;
                        }
                        player.Inventory.FirstOrDefault(i => i.Id == id).Slot = destSlot;
                        IN = inv.ItemId;
                    }
                    break;
            }
            ModifyAttributes(OUT, IN, player);
            return 1;
        }

        public static int BuyItem(string id, int qty, User player)
        {
            var item = Globals.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return 2; // Invalid item

            if (item.Cost * qty <= player.Credits)
            {
                player.Credits -= item.Cost * qty;
                var newInv = new Inventory()
                {
                    Id = Guid.NewGuid().ToString(),
                    Dropped = DateTime.UtcNow,
                    ItemId = item.Id,
                    Quantity = qty,
                    Slot = 61,
                    UserId = player.Id,
                    X = 0,
                    Y = 0
                };
                var stacked = FindAvailableStack(player.Id, newInv);
                if (stacked == 0) return -1;
                if (stacked == -1) return 2;
                newInv.Quantity = stacked;
                var newSlot = FindOpenSlot(player);
                if (newSlot == -1) return 3; // Not enough room
                newInv.Slot = newSlot;
                player.Inventory.Add(newInv);
            }
            else
            {
                return 1; // Too expensive
            }
            return -1;
        }

        public static int SellItem(string id, int qty, User player)
        {
            var inv = player.Inventory.FirstOrDefault(i => i.Id == id);
            if (inv == null) return 2; // Invalid item
            var item = Globals.Items.FirstOrDefault(n => n.Id == inv.ItemId);
            if (item == null) return 2; // Invalid item again
            if (inv.Quantity > qty)
            {
                player.Inventory.FirstOrDefault(i => i.Id == id).Quantity -= qty;
            }
            else
            {
                player.Inventory.Remove(inv);
            }
            player.Credits += item.Cost * qty;
            return -1;
        }

        public static int StackItems(string from, string to, User player)
        {
            var invF = player.Inventory.FirstOrDefault(i => i.Id == from);
            var invT = player.Inventory.FirstOrDefault(i => i.Id == to);
            if (invF == null || invT == null) return -1;
            var stackable = Globals.Items.FirstOrDefault(i => i.Id == invT.ItemId).Stack;
            if (invT.ItemId != invF.ItemId || !stackable || invT.Quantity >= 999) return -1;
            {
                var allowable = 999 - invT.Quantity;
                if (invF.Quantity - allowable > 0)
                {
                    player.Inventory.FirstOrDefault(i => i.Id == to).Quantity += allowable;
                    player.Inventory.FirstOrDefault(i => i.Id == @from).Quantity -= allowable;
                }
                else
                {
                    player.Inventory.FirstOrDefault(i => i.Id == to).Quantity += invF.Quantity;
                    player.Inventory.Remove(invF);
                }
            }

            return 1;
        }

        public static void ModifyAttributes(string OUT, string IN, User player)
        {
            if (OUT == null && IN == null) return;
            if (IN != null)
            {
                var _IN = Globals.Items.FirstOrDefault(n => n.Id == IN);
                player.Armor += _IN?.Armor ?? 0;
                player.Defense += _IN?.Defense ?? 0;
                player.Health += _IN?.Hull ?? 0;
                player.MaxHealth += _IN?.Hull ?? 0;
                player.Offense += _IN?.Offense ?? 0;
                player.Power += _IN?.Power ?? 0;
                player.Repair += _IN?.Repair ?? 0;
                player.Shield += _IN?.Shield ?? 0;
                player.MaxShield += _IN?.Shield ?? 0;
                player.Thrust += _IN?.Thrust ?? 0;
                switch (_IN?.Slot)
                {
                    case 7:
                        player.Weap1Charge = 0;
                        player.Weap1ChargeRate = _IN.Recharge;
                        break;
                    case 8:
                        player.Weap2Charge = 0;
                        player.Weap2ChargeRate = _IN.Recharge;
                        break;
                    case 9:
                        player.Weap3Charge = 0;
                        player.Weap3ChargeRate = _IN.Recharge;
                        break;
                    case 10:
                        player.Weap4Charge = 0;
                        player.Weap4ChargeRate = _IN.Recharge;
                        break;
                    case 11:
                        player.Weap5Charge = 0;
                        player.Weap5ChargeRate = _IN.Recharge;
                        break;
                }
            }
            if (OUT == null) return;
            var _OUT = Globals.Items.FirstOrDefault(n => n.Id == OUT);
            player.Armor -= _OUT?.Armor ?? 0;
            player.Defense -= _OUT?.Defense ?? 0;
            player.Health -= _OUT?.Hull ?? 0;
            player.MaxHealth -= _OUT?.Hull ?? 0;
            player.Offense -= _OUT?.Offense ?? 0;
            player.Power -= _OUT?.Power ?? 0;
            player.Repair -= _OUT?.Repair ?? 0;
            player.Shield -= _OUT?.Shield ?? 0;
            player.MaxShield -= _OUT?.Shield ?? 0;
            player.Thrust -= _OUT?.Thrust ?? 0;
            if (player.Shield < 0) player.Shield = 0;
            if (player.Health <= 1) player.Shield = 1;
        }

        public static void ClearDebris()
        {
            foreach (var itm in Globals.Inventory.ToList())
            {
                if ((DateTime.UtcNow - itm.Dropped).TotalMinutes > 1.0)
                {
                    Globals.Inventory.Remove(itm);
                }
            }
            foreach (var loot in Globals.Loot.ToList())
            {
                if ((DateTime.UtcNow - loot.Dropped).TotalMinutes > 3.0)
                {
                    Globals.Loot.Remove(loot);
                }
            }
        }

        public static void Charge(List<User> users)
        {
            var up = false;
            users.Where(u => u?.Id != null).ToList().ForEach(user =>
            {
                if (user.Weap1Charge < 100 && user.Weap1ChargeRate > 0)
                {
                    up = true;
                    user.Weap1Charge += user.Weap1ChargeRate;
                    if (user.Weap1Charge > 100) user.Weap1Charge = 100;
                }
                if (user.Weap2Charge < 100 && user.Weap2ChargeRate > 0)
                {
                    up = true;
                    user.Weap2Charge += user.Weap2ChargeRate / 2;
                    if (user.Weap2Charge > 100) user.Weap2Charge = 0;
                }
                if (user.Weap3Charge < 100 && user.Weap3ChargeRate > 0)
                {
                    up = true;
                    user.Weap3Charge += user.Weap3ChargeRate / 2;
                    if (user.Weap3Charge > 100) user.Weap3Charge = 0;
                }
                if (user.Weap4Charge < 100 && user.Weap4ChargeRate > 0)
                {
                    up = true;
                    user.Weap4Charge += user.Weap4ChargeRate / 2;
                    if (user.Weap4Charge > 100) user.Weap4Charge = 0;
                }
                if (user.Weap5Charge < 100 && user.Weap5ChargeRate > 0)
                {
                    up = true;
                    user.Weap5Charge += user.Weap5ChargeRate / 2;
                    if (user.Weap5Charge > 100) user.Weap5Charge = 0;
                }

                if (up)
                {
                    Program.shd.UpdatePlayer(Array.FindIndex(Types.Player, u => u.Id == user.Id));
                }
            });
        }

        public static void CreateLoot(User player, Vector2 location)
        {
            var loot = new Loot()
            {
                Owner = player.Id,
                Dropped = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Items = new string[9],
                Quantities = new int[9],
                X = location.X,
                Y = location.Y
            };
            loot.Items[0] = "a53edcf0-1105-4edf-9c30-217279c3d286";
            loot.Items[1] = "50521cfe-7d63-4495-a1cf-b900fb8d225f";
            loot.Quantities[0] = 1;
            loot.Quantities[1] = 10;
            Globals.Loot.Add(loot);
        }

        public static bool CollectLoot(string lootId, int lootIndex, User player)
        {
            var temp = Globals.Loot.FirstOrDefault(l => l.Id == lootId);
            if (temp == null) return false;
            var newInv = new Inventory()
            {
                Id = Guid.NewGuid().ToString(),
                Dropped = DateTime.UtcNow,
                ItemId = temp.Items[lootIndex],
                Quantity = temp.Quantities[lootIndex],
                Slot = 0,
                UserId = player.Id,
                X = 0,
                Y = 0
            };
            var stacked = FindAvailableStack(player.Id, newInv);
            if (stacked == 0)
            {
                Globals.Loot.FirstOrDefault(l => l.Id == lootId).Items[lootIndex] = null;
                Globals.Loot.FirstOrDefault(l => l.Id == lootId).Quantities[lootIndex] = 0;
            };
            if (stacked > 0)
            {
                newInv.Quantity = stacked;
                var newSlot = FindOpenSlot(player);
                if (newSlot != -1)
                {
                    newInv.Slot = newSlot;
                    player.Inventory.Add(newInv);
                    Globals.Loot.FirstOrDefault(l => l.Id == lootId).Items[lootIndex] = null;
                    Globals.Loot.FirstOrDefault(l => l.Id == lootId).Quantities[lootIndex] = 0;
                }
                else
                {
                    Program.shd.SendMessage(Array.IndexOf(Types.PlayerIds, player.Id), "You have no room for that in your cargo hold.",
                        MessageColors.Notification);
                }
            }
            if (temp.Items.All(i => i == null) == true) // All item strings are null.  Destroy it
            {
                Globals.Loot.Remove(Globals.Loot.FirstOrDefault(l => l.Id == lootId));
            }
            return true;
        }

        public static int GiveXP(User player, int XP)
        {
            var newLevels = 0;
            player.Exp += XP;
            while (player.Exp >= CheckLevel(player.Level + 1))
            {

                if (player.Level >= Constants.MAX_LEVEL)
                {
                    newLevels = -1;
                }
                else
                {
                    player.Level++;
                    newLevels++;
                }
            }

            return newLevels;
        }

        public static double CheckLevel(int level)
        {
            return Math.Floor(Constants.LVL_BASE * Math.Pow(level, Constants.LVL_EXPONENT));
        }

        public static void GenerateNebulae()
        {
            var random = new Random();
            for (var i = 0; i < Constants.MAX_NEBULAE; i++)
            {
                var temp = new Nebula
                {
                    ID = Guid.NewGuid().ToString(),
                    Type = 0,
                    X = random.Next(500, Constants.PLAY_AREA_WIDTH - 1000),
                    Y = random.Next(500, Constants.PLAY_AREA_HEIGHT - 1000)
                };
                Globals.Nebulae.Add(temp);
            }
        }
    }
}
