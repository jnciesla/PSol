using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bindings;
using System.IO;
using MonoGame.Extended;

namespace Client
{
    class Graphics
    {
        public static Texture2D[] Characters = new Texture2D[2];
        public static string statusMessage = "";

        public static void InitializeGraphics(ContentManager manager)
        {
            LoadCharacters(manager);
        }

        public static void RenderGraphics()
        {
            DrawPlayer();
        }

        private static void DrawSprite(int sprite, Vector2 position, float rotation, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Characters[sprite], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        private static void DrawPlayer()
        {
            double X, Y;
            int SpriteNum;

            SpriteNum = 1;

            var origin = new Vector2(Characters[SpriteNum].Width / 2f, Characters[SpriteNum].Height / 2f); 
            X = Types.Player[0].X * 32 + Types.Player[0].XOffset - ((Characters[SpriteNum].Width / 4 - 32) / 2);
            Y = Types.Player[0].Y * 32 + Types.Player[0].YOffset;
            DrawSprite(SpriteNum, new Vector2(Types.Player[0].X, Types.Player[0].Y), Types.Player[0].Rotation, origin);
        }

        private static void LoadCharacters(ContentManager manager)
        {
            for(int i = 1; i < Characters.Length; i++)
            {
                Characters[i] = manager.Load<Texture2D>("Characters/" + i.ToString());
            }
        }

        public static void DrawHud(ContentManager manager)
        {
            SpriteFont infoFont = manager.Load<SpriteFont>("GeonBit.UI/themes/hd/fonts/Regular");
            Game1.spriteBatch.Begin();
            Game1.spriteBatch.DrawString(infoFont, (int)Types.Player[0].X/100 + ":" + (int)Types.Player[0].Y/100, new Vector2(512, 10), Color.DarkBlue);
            Game1.spriteBatch.DrawString(infoFont, statusMessage, new Vector2(512, 30), Color.DarkBlue);
            Game1.spriteBatch.End();
        }
    }
}
