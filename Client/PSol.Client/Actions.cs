using System;
using System.Linq;
using Bindings;
using Microsoft.Xna.Framework;

namespace PSol.Client
{
    public class Actions
    {
        public static void Trade()
        {
            if (GameLogic.selectedPlanet == "") return;
            var PLANET = GameLogic.Planets.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);
            var P = Types.Player[GameLogic.PlayerIndex];
            if (P.X < PLANET.X + 500 && P.X > PLANET.X - 500 && P.Y < PLANET.Y + 500 && P.Y > PLANET.Y - 500)
            {
                Globals.inventoryMode = 2;
                Game1.IGUI.pInv = false;
                Game1.IGUI.PopulateInventory();
                MenuManager.ChangeMenu(MenuManager.Menu.Inventory);
            }
            else
            {
                InterfaceGUI.AddChats("You are too far away to interact with " + PLANET.Name + ".", Color.DarkGoldenrod);
            }
        }

        public static void Mine()
        {
            if (GameLogic.selectedPlanet == "") return;
            var PLANET = GameLogic.Planets.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);
            var P = Types.Player[GameLogic.PlayerIndex];
            if (P.X < PLANET.X + 500 && P.X > PLANET.X - 500 && P.Y < PLANET.Y + 500 && P.Y > PLANET.Y - 500)
            {
                Console.WriteLine("Mining");
            }
            else
            {
                InterfaceGUI.AddChats("You are too far away to interact with " + PLANET.Name + ".", Color.DarkGoldenrod);
            }
        }

        public static void Aerology()
        {
            if (GameLogic.selectedPlanet == "") return;
            var PLANET = GameLogic.Planets.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);
            var P = Types.Player[GameLogic.PlayerIndex];
            if (P.X < PLANET.X + 500 && P.X > PLANET.X - 500 && P.Y < PLANET.Y + 500 && P.Y > PLANET.Y - 500)
            {
                Console.WriteLine("Aerology");
            }
            else
            {
                InterfaceGUI.AddChats("You are too far away to interact with " + PLANET.Name + ".", Color.DarkGoldenrod);
            }
        }

        public static void Attack(int weap, string mob)
        {
            switch (weap)
            {
                case 1:
                    if (Types.Player[GameLogic.PlayerIndex].Weap1Charge >= 100)
                    {
                        var weapId = Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(i => i.Slot == 7)?.ItemId;
                        if (weapId == null) break;
                        Types.Player[GameLogic.PlayerIndex].Weap1Charge = 0;
                        Game1.ctcp.SendCombat(mob, 7);
                    }
                    break;
                case 2:
                    if (Types.Player[GameLogic.PlayerIndex].Weap2Charge >= 100)
                    {
                        Types.Player[GameLogic.PlayerIndex].Weap2Charge = 0;
                        var weapId = Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(i => i.Slot == 8)?.ItemId;
                        if (weapId == null) break;
                        Game1.ctcp.SendCombat(mob, 8);
                    }
                    break;
                case 3:
                    if (Types.Player[GameLogic.PlayerIndex].Weap3Charge >= 100)
                    {
                        Types.Player[GameLogic.PlayerIndex].Weap3Charge = 0;
                        var weapId = Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(i => i.Slot == 9)?.ItemId;
                        if (weapId == null) break;
                        Game1.ctcp.SendCombat(mob, 9);
                    }
                    break;
                case 4:
                    if (Types.Player[GameLogic.PlayerIndex].Weap4Charge >= 100)
                    {
                        Types.Player[GameLogic.PlayerIndex].Weap4Charge = 0;
                        var weapId = Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(i => i.Slot == 10)?.ItemId;
                        if (weapId == null) break;
                        Game1.ctcp.SendCombat(mob, 10);
                    }
                    break;
                case 5:
                    if (Types.Player[GameLogic.PlayerIndex].Weap5Charge >= 100)
                    {
                        Types.Player[GameLogic.PlayerIndex].Weap5Charge = 0;
                        var weapId = Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(i => i.Slot == 11)?.ItemId;
                        if (weapId == null) break;
                        Game1.ctcp.SendCombat(mob, 11);
                    }
                    break;
            }
        }
    }
}
