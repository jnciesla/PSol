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
        public static bool TransferItem(string item, int index, string recipient)
        {

            User newOwner = null;
            var temp = Types.Player[index].Inventory.ToList().FirstOrDefault(i => i.Id == item);
            if (recipient != "X")
            {
                newOwner = Types.Player.FirstOrDefault(p => p.Id == recipient);
            }

            if (recipient != "X" && newOwner != null)
            {
                var stacked = FindAvailableStack(newOwner.Id, temp);
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
                    UserId = newOwner.Id,
                    X = 0,
                    Y = 0
                };
                newOwner.Inventory.Add(newInv);
                Types.Player[index].Inventory.Remove(temp);
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
                    X = Types.Player[index].X,
                    Y = Types.Player[index].Y
                };
                Globals.Inventory.Add(newInv);
                Types.Player[index].Inventory.Remove(temp);
                return true;
            }
        }

        public static bool ReceiveFromGlobal(string ID, int index)
        {
            var temp = Globals.Inventory.FirstOrDefault(i => i.Id == ID);
            if (temp != null)
            {
                Globals.Inventory.Remove(temp);
                var stacked = FindAvailableStack(Types.Player[index].Id, temp);
                if (stacked == 0) return true;
                if (stacked == -1) return false;
                temp.Quantity = stacked;
                var newSlot = FindOpenSlot(Types.Player[index].Id);
                if (newSlot != -1)
                {
                    var newInv = new Inventory()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Dropped = DateTime.UtcNow,
                        ItemId = temp.ItemId,
                        Quantity = temp.Quantity,
                        Slot = newSlot,
                        UserId = Types.Player[index].Id,
                        X = 0,
                        Y = 0
                    };
                    Types.Player[index].Inventory.Add(newInv);
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

        public static int FindOpenSlot(string ID)
        {
            for (var n = 0; n < 60; n++)
            {
                if (Types.Player.FirstOrDefault(p => p.Id == ID)?.Inventory.FirstOrDefault(inv => inv.Slot == n + 101)?.ItemId == null)
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

        public static int EquipItem(string id, int index, int destSlot)
        {
            string OUT = null;
            string IN = null;
            var inv = Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id);
            if (inv == null) return 2;
            var itm = Globals.Items.FirstOrDefault(i => i.Id == inv?.ItemId);
            if (itm == null) return 2;
            switch (destSlot)
            {
                case 0: // Equip
                    var installed = Types.Player[index].Inventory.FirstOrDefault(s => s.Slot == itm.Slot); // Check if something is already installed
                    if (installed != null)
                    {
                        installed.Slot = inv.Slot;
                        OUT = installed.ItemId;
                    }
                    Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id).Slot = itm.Slot;
                    IN = inv.ItemId;
                    break;
                case -1: // Unequip
                    // TODO: Make it so unequipping an item tries to stack it first
                    var newSlot = FindOpenSlot(Types.Player[index].Id);
                    if (newSlot == -1)
                    {
                        return 3;
                    }
                    OUT = inv.ItemId;
                    Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id).Slot = newSlot;
                    break;
                default: // Move or install specific slot
                    if (destSlot > 100)
                    {
                        Types.Player[index].Inventory.FirstOrDefault(i => i.Slot == inv.Slot).Slot = destSlot;
                    }
                    else
                    {
                        var _installed = Types.Player[index].Inventory.FirstOrDefault(s => s.Slot == destSlot); // Check if something is already installed
                        if (_installed != null)
                        {
                            _installed.Slot = inv.Slot;
                            OUT = _installed.ItemId;
                        }
                        Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id).Slot = destSlot;
                        IN = inv.ItemId;
                    }
                    break;
            }
            ModifyAttributes(OUT, IN, index);
            return 1;
        }

        public static int BuyItem(string id, int qty, int index)
        {
            var item = Globals.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return 2; // Invalid item

            if (item.Cost * qty <= Types.Player[index].Credits)
            {
                Types.Player[index].Credits -= item.Cost * qty;
                var newInv = new Inventory()
                {
                    Id = Guid.NewGuid().ToString(),
                    Dropped = DateTime.UtcNow,
                    ItemId = item.Id,
                    Quantity = qty,
                    Slot = 61,
                    UserId = Types.Player[index].Id,
                    X = 0,
                    Y = 0
                };
                var stacked = FindAvailableStack(Types.Player[index].Id, newInv);
                if (stacked == 0) return -1;
                if (stacked == -1) return 2;
                newInv.Quantity = stacked;
                var newSlot = FindOpenSlot(Types.Player[index].Id);
                if (newSlot == -1) return 3; // Not enough room
                newInv.Slot = newSlot;
                Types.Player[index].Inventory.Add(newInv);
            }
            else
            {
                return 1; // Too expensive
            }
            return -1;
        }

        public static int SellItem(string id, int qty, int index)
        {
            var inv = Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id);
            if (inv == null) return 2; // Invalid item
            var item = Globals.Items.FirstOrDefault(n => n.Id == inv.ItemId);
            if (item == null) return 2; // Invalid item again
            if (inv.Quantity > qty)
            {
                Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id).Quantity -= qty;
            }
            else
            {
                Types.Player[index].Inventory.Remove(inv);
            }
            Types.Player[index].Credits += item.Cost * qty;
            return -1;
        }

        public static int StackItems(string from, string to, int index)
        {
            var invF = Types.Player[index].Inventory.FirstOrDefault(i => i.Id == from);
            var invT = Types.Player[index].Inventory.FirstOrDefault(i => i.Id == to);
            if (invF == null || invT == null) return -1;
            var stackable = Globals.Items.FirstOrDefault(i => i.Id == invT.ItemId).Stack;
            if (invT.ItemId != invF.ItemId || !stackable || invT.Quantity >= 999) return -1;
            {
                var allowable = 999 - invT.Quantity;
                if (invF.Quantity - allowable > 0)
                {
                    Types.Player[index].Inventory.FirstOrDefault(i => i.Id == to).Quantity += allowable;
                    Types.Player[index].Inventory.FirstOrDefault(i => i.Id == @from).Quantity -= allowable;
                }
                else
                {
                    Types.Player[index].Inventory.FirstOrDefault(i => i.Id == to).Quantity += invF.Quantity;
                    Types.Player[index].Inventory.Remove(invF);
                }
            }

            return 1;
        }

        public static void ModifyAttributes(string OUT, string IN, int index)
        {
            if (OUT == null && IN == null) return;
            if (IN != null)
            {
                var _IN = Globals.Items.FirstOrDefault(n => n.Id == IN);
                Types.Player[index].Armor += _IN?.Armor ?? 0;
                Types.Player[index].Defense += _IN?.Defense ?? 0;
                Types.Player[index].Health += _IN?.Hull ?? 0;
                Types.Player[index].MaxHealth += _IN?.Hull ?? 0;
                Types.Player[index].Offense += _IN?.Offense ?? 0;
                Types.Player[index].Power += _IN?.Power ?? 0;
                Types.Player[index].Repair += _IN?.Repair ?? 0;
                Types.Player[index].Shield += _IN?.Shield ?? 0;
                Types.Player[index].MaxShield += _IN?.Shield ?? 0;
                Types.Player[index].Thrust += _IN?.Thrust ?? 0;
                switch (_IN?.Slot)
                {
                    case 7:
                        Types.Player[index].Weap1Charge = 0;
                        Types.Player[index].Weap1ChargeRate = _IN.Recharge;
                        break;
                    case 8:
                        Types.Player[index].Weap2Charge = 0;
                        Types.Player[index].Weap2ChargeRate = _IN.Recharge;
                        break;
                    case 9:
                        Types.Player[index].Weap3Charge = 0;
                        Types.Player[index].Weap3ChargeRate = _IN.Recharge;
                        break;
                    case 10:
                        Types.Player[index].Weap4Charge = 0;
                        Types.Player[index].Weap4ChargeRate = _IN.Recharge;
                        break;
                    case 11:
                        Types.Player[index].Weap5Charge = 0;
                        Types.Player[index].Weap5ChargeRate = _IN.Recharge;
                        break;
                }
            }
            if (OUT == null) return;
            var _OUT = Globals.Items.FirstOrDefault(n => n.Id == OUT);
            Types.Player[index].Armor -= _OUT?.Armor ?? 0;
            Types.Player[index].Defense -= _OUT?.Defense ?? 0;
            Types.Player[index].Health -= _OUT?.Hull ?? 0;
            Types.Player[index].MaxHealth -= _OUT?.Hull ?? 0;
            Types.Player[index].Offense -= _OUT?.Offense ?? 0;
            Types.Player[index].Power -= _OUT?.Power ?? 0;
            Types.Player[index].Repair -= _OUT?.Repair ?? 0;
            Types.Player[index].Shield -= _OUT?.Shield ?? 0;
            Types.Player[index].MaxShield -= _OUT?.Shield ?? 0;
            Types.Player[index].Thrust -= _OUT?.Thrust ?? 0;
            if (Types.Player[index].Shield < 0) Types.Player[index].Shield = 0;
            if (Types.Player[index].Health <= 1) Types.Player[index].Shield = 1;
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

        public static void CreateLoot(int index, Vector2 location)
        {
            var loot = new Loot()
            {
                Owner = Types.Player[index].Id,
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

        public static bool CollectLoot(string lootId, int lootIndex, int index)
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
                UserId = Types.Player[index].Id,
                X = 0,
                Y = 0
            };
            var stacked = FindAvailableStack(Types.Player[index].Id, newInv);
            if (stacked == 0)
            {
                Globals.Loot.FirstOrDefault(l => l.Id == lootId).Items[lootIndex] = null;
                Globals.Loot.FirstOrDefault(l => l.Id == lootId).Quantities[lootIndex] = 0;
            };
            if (stacked > 0)
            {
                newInv.Quantity = stacked;
                var newSlot = FindOpenSlot(Types.Player[index].Id);
                if (newSlot != -1)
                {
                    newInv.Slot = newSlot;
                    Types.Player[index].Inventory.Add(newInv);
                    Globals.Loot.FirstOrDefault(l => l.Id == lootId).Items[lootIndex] = null;
                    Globals.Loot.FirstOrDefault(l => l.Id == lootId).Quantities[lootIndex] = 0;
                }
                else
                {
                    Program.shd.SendMessage(index, "You have no room for that in your cargo hold.",
                        MessageColors.Notification);
                }
            }
            if (temp.Items.All(i => i == null) == true) // All item strings are null.  Destroy it
            {
                Globals.Loot.Remove(Globals.Loot.FirstOrDefault(l => l.Id == lootId));
            }
            return true;
        }

        public static int GiveXP(int index, int XP)
        {
            var newLevels = 0;
            Types.Player[index].Exp += XP;
            while (Types.Player[index].Exp >= CheckLevel(Types.Player[index].Level + 1))
            {

                if (Types.Player[index].Level >= Constants.MAX_LEVEL)
                {
                    newLevels = -1;
                }
                else
                {
                    Types.Player[index].Level++;
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
