using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Entities;

namespace Client
{
    class MenuManager
    {
        public static Menu menu;

        public enum Menu
        {
            Login,
            Register
        }

        public static void ChangeMenu(Menu menu)
        {
            Clear();
            InterfaceGUI.Windows[(int)menu].Visible = true;
            Globals.windowOpen = true;
        }

        public static void Clear()
        {
            foreach (Panel window in InterfaceGUI.Windows)
            {
                window.Visible = false;
            }
            UserInterface.Active.SetCursor(CursorType.Default);
            Globals.windowOpen = false;
        }
    }
}
