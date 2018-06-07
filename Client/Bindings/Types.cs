using System;
using System.Collections.Generic;
using System.Text;
using Bindings;
using Microsoft.Xna.Framework;

namespace Bindings
{
    internal class Types
    {
        public static PlayerStruct[] Player = new PlayerStruct[Constants.MAX_PLAYERS];

        [Serializable]
        public struct PlayerStruct
        {
            // Account
            public string Login;
            public string Password;

            // General
            public string Name;
            public int MaxHealth;
            public int Health;
            public int MaxShield;
            public int Shield;
            public int Sprite;
            public int Level;
            public int Exp;

            //Position
            public int Map;
            public float X;
            public float Y;
            public int Dir;
            public float Rotation;

            // Client use
            public int XOffset;
            public int YOffset;
            public int Moving;
            public byte Steps;
        }

        public struct RECT
        {
            public int Top;
            public int Right;
            public int Bottom;
            public int Left;
        }
    }
}
