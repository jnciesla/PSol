#pragma warning disable CS0436 // Type conflicts with imported type
using System;
using System.Linq;
using Bindings;
using PSol.Data.Models;

namespace PSol.Server
{
    internal class Transactions
    {
        public static bool TransferItem(string item, int index, string recipient)
        {

            User newOwner = null;
            var newSlot = -1;
            var temp = Types.Player[index].Inventory.ToList().FirstOrDefault(i => i.Id == item);
            if (recipient != "X")
            {
                newOwner = Types.Player.FirstOrDefault(p => p.Id == recipient);
                newSlot = FindOpenSlot(recipient);
            }

            if (recipient != "X" && newOwner != null && newSlot != -1)
            {
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
                var newSlot = FindOpenSlot(Types.Player[index].Id);
                if (newSlot != -1)
                {
                    var newInv = new Inventory()
                    {
                        Id = null,
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
                if (Types.Player.FirstOrDefault(p => p.Id == ID)?.Inventory.FirstOrDefault(inv => inv.Slot == n)?.ItemId == null)
                {
                    return n + 101;
                };
            }
            return -1;
        }

        public static int EquipItem(string id, int index)
        {
            var inv = Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id);
            var itm = Globals.Items.FirstOrDefault(I => I.Id == inv?.ItemId);
            if (inv?.Slot > 100)
            {
                var existing = Types.Player[index].Inventory.FirstOrDefault(i => i.Id == id);
                if (existing == null)
                {
                    inv.Slot = itm.Slot;
                }
                else
                {
                    var tmp = itm.Slot;
                    Types.Player[index].Inventory.FirstOrDefault(i => i.Slot == inv.Slot).Slot = inv.Slot;
                    inv.Slot = tmp;
                }
            }
            else
            {
                var newSlot = FindOpenSlot(Types.Player[index].Id);
                if (newSlot == -1)
                {
                    return 3;
                }

                inv.Slot = newSlot;
            }
            return 1;
        }

        public static void ClearDebris()
        {
            foreach (Inventory itm in Globals.Inventory.ToList())
            {
                if ((DateTime.UtcNow - itm.Dropped).TotalMinutes > 1.0)
                {
                    Globals.Inventory.Remove(itm);
                }
            }
        }
    }
}
