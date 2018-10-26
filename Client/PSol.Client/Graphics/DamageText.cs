using System;
using Microsoft.Xna.Framework;

namespace PSol.Client
{
    public class DamageText
    {
        public string Str { get; set; }
        public Vector2 Pos { get; set; }

        private float Opacity { get; set; }
        private float Angle { get;}

        public DamageText(string str, Vector2 pos)
        {
            var random = new Random();
            Str = str;
            Pos = pos;
            Opacity = 3.5f;
            Angle = (float)random.NextDouble() * (random.Next(0, 1) * 2 - 1);
        }

        public void Update()
        {
            Pos = new Vector2(Pos.X - Angle, Pos.Y - 2.5F);
            Opacity -= .075F;
            if (Opacity <= 0)
            {
                Game1.DamageTexts.RemoveAt(Game1.DamageTexts.IndexOf(this));
            }
        }

        public void Draw()
        {
            Graphics.DrawString(Globals.Font10, Str,Pos.X,Pos.Y,false, Color.DarkRed * Opacity);
            Graphics.DrawString(Globals.Font10, Str, Pos.X - 1, Pos.Y, false, Color.Red * Opacity);
        }
    }
}
