using Microsoft.Xna.Framework;
using GeonBit.UI.Entities;

namespace PSol.Client
{
    internal class Globals
    {
        public static bool exitgame = false;
        public static bool windowOpen;
        public static Rectangle mapSize = new Rectangle(0, 0, 102500, 102500);
        public static Rectangle playArea = new Rectangle(0, 0, 100010, 100010);
        public static int PreferredBackBufferWidth = 1024;
        public static int PreferredBackBufferHeight = 768;

        public static Panel chatPanel;
        public static bool pauseChat = false;

        public static Color Luminosity = Color.White;
        public static bool Control = false;

        public static string loginUsername = "";
        public static string loginPassword = "";
        public static string registerUsername = "";
        public static string registerPassword = "";
        public static string registerValidate = "";
        public static int Selected = -1;

        // Game direction vars
        public static bool DirUp;
        public static bool DirDn;
        public static bool DirLt;
        public static bool DirRt;

        public static bool ZoomIn;
        public static bool ZoomOut;
        public static bool ZoomDefault;

        public static bool Details1;
        public static bool Details2;
    }
}
