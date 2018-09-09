using Bindings;
using Microsoft.Xna.Framework;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    internal class Globals
    {
        public static bool Fullscreen = false;
        public static bool exitgame = false;
        public static bool windowOpen;
        public static Rectangle mapSize = new Rectangle(0, 0, Constants.MAP_SIZE_WIDTH, Constants.MAP_SIZE_HEIGHT);
        public static Rectangle playArea = new Rectangle(0, 0, Constants.PLAY_AREA_WIDTH, Constants.PLAY_AREA_HEIGHT);
        public static int PreferredBackBufferWidth = 1024;
        public static int PreferredBackBufferHeight = 768;

        public static float PlanetaryRotation = 0;
        public static long serverTime = 0;

        // GUI Stuff
        public static Panel chatPanel;
        public static bool pauseChat = false;
        public static bool cursorOverride = true;
        public static bool scanner = true;
        public static bool details = true;

        public static Color Luminosity = Color.White;
        public static bool Control = false;
        public static bool Shift = false;
        public static bool Alt = false;


        public static string loginUsername = "";
        public static string loginPassword = "";
        public static string registerUsername = "";
        public static string registerPassword = "";
        public static string registerValidate = "";

        // Game direction vars
        public static bool DirUp;
        public static bool DirDn;
        public static bool DirLt;
        public static bool DirRt;

        public static bool Details1;
        public static bool Details2;

        // Fonts
        public static SpriteFont Font14;
        public static SpriteFont Font12;
        public static SpriteFont Font10;
        public static SpriteFont Font8;
    }
}
