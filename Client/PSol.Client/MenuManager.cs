using GeonBit.UI;

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
            Message
        }

        public static void ChangeMenu(Menu menu)
        {
            Clear();
            InterfaceGUI.Windows[(int)menu].Visible = true;
                Globals.windowOpen = true;
            if ((int) menu == 1 || (int) menu == 2)
            {
                Globals.cursorOverride = true;
            }
        }

        public static void Clear(int which = -1)
        {
            Globals.cursorOverride = false;
            if (which == -1)
            {
                foreach (var window in InterfaceGUI.Windows)
                {
                    if (window != InterfaceGUI.Windows[0]) // Don't close chats.  At some point I'll make that an option
                    {
                        window.Visible = false;
                    }

                }

                UserInterface.Active.SetCursor(CursorType.Default);
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
