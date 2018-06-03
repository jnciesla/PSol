using System;
using Microsoft.Xna.Framework;
using Bindings;

namespace Client
{
    class Globals
    {
        public static bool windowOpen;

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

        public static bool ZoomIn;
        public static bool ZoomOut;
        public static bool ZoomDefault;

        public static bool Tab;
        public static bool Enter;

    }
}
