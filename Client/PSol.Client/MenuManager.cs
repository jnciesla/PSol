using System;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace PSol.Client
{
    internal class MenuManager
    {
        public static Menu menu;

        public enum Menu
        {
            Chats,
            Login,
            Register,
            Message,
            Map
        }

        public static void ChangeMenu(Menu menu)
        {
            Clear();
            InterfaceGUI.Windows[(int)menu].Visible = true;
            Globals.windowOpen = true;
            if (menu == Menu.Login || menu == Menu.Register || menu == Menu.Map)
            {
                Globals.cursorOverride = true;
            }

            if ((int)menu == 3)
            {
                if (Globals.scanner)
                {
                    InterfaceGUI.Windows[(int)menu].SetPosition(Anchor.BottomLeft, new Vector2(-50, 240));
                }
                else
                {
                    InterfaceGUI.Windows[(int)menu].SetPosition(Anchor.BottomLeft, new Vector2(-50, 40));
                }
            }
        }

        public static void Clear(int which = -1)
        {
            Globals.cursorOverride = false;
            if (which == -1)
            {
                foreach (var window in InterfaceGUI.Windows)
                {
                    if (window == InterfaceGUI.Windows[0] || window == InterfaceGUI.Windows[1]) // Don't close chats or login
                        continue;
                    UserInterface.Active.SetCursor(CursorType.Default);
                    window.Visible = false;
                }

                if (!InterfaceGUI.Windows[1].Visible && !InterfaceGUI.Windows[2].Visible)
                    Globals.windowOpen = false;
            }
            else
            {
                InterfaceGUI.Windows[which].Visible = false;
                UserInterface.Active.SetCursor(CursorType.Default);
                Globals.windowOpen = false;
            }
        }
    }
}
