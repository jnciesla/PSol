using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bindings;
using System.IO;
using MonoGame.Extended;

namespace Client
{
    internal class Graphics
    {
        public static Texture2D[] Characters = new Texture2D[2];
        public static Texture2D pixel;
        private static Model model;

        public static void InitializeGraphics(ContentManager manager)
        {
            LoadCharacters(manager);
            model = manager.Load<Model>("planet");
            pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        public static void RenderGraphics(ContentManager manager)
        {
            DrawPlayers(manager);
        }

        private static void DrawSprite(int sprite, Vector2 position, float rotation, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Characters[sprite], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        private static void DrawPlayers(ContentManager manager)
        {
            var HealthFont = manager.Load<SpriteFont>("GeonBit.UI/themes/editor/fonts/Size10");
            var ShieldFont = manager.Load<SpriteFont>("GeonBit.UI/themes/editor/fonts/Size8");
            if (GameLogic.PlayerIndex <= -1) return;

            int SpriteNum = 1;
            Types.Player[1].MaxHealth = 500;
            Types.Player[1].Health = 100;
            Types.Player[1].MaxShield = 100;
            Types.Player[1].Shield = 89;

            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                if (Types.Player[i].X.CompareTo(0.0f) == 0) continue;
                var origin = new Vector2(Characters[SpriteNum].Width / 2f, Characters[SpriteNum].Height / 2f);
                DrawSprite(SpriteNum, new Vector2(Types.Player[i].X, Types.Player[i].Y), Types.Player[i].Rotation, origin);

                // Draw status stuff
                if (Globals.Details)
                {
                    // Health
                    Color HealthColor1 = Color.Green;
                    Color HealthColor2 = Color.DarkGreen;
                    float percentHealth = (Types.Player[i].MaxHealth - Types.Player[i].Health) / (float)Types.Player[i].MaxHealth;
                    // Percent Health is percentage missing, so colors are inverted:
                    if (percentHealth > 0.75)
                    {
                        HealthColor1 = Color.Red;
                        HealthColor2 = Color.DarkRed;
                    }
                    else if (percentHealth < 0.75 && percentHealth > 0.25)
                    {
                        HealthColor1 = Color.Orange;
                        HealthColor2 = Color.DarkOrange;
                    }
                    else if (percentHealth < 0.25)
                    {
                        HealthColor1 = Color.Green;
                        HealthColor2 = Color.DarkGreen;
                    }
                    RectangleF healthRect = new RectangleF(Types.Player[i].X - Characters[SpriteNum].Width / 2,
                        Types.Player[i].Y + Characters[SpriteNum].Height / 2, Characters[SpriteNum].Width, 6);
                    Game1.spriteBatch.DrawLine(healthRect.Left + 2, healthRect.Bottom - 2, healthRect.Right - (Characters[SpriteNum].Width * percentHealth), healthRect.Bottom - 2, HealthColor1, 4F);
                    Game1.spriteBatch.DrawRectangle(healthRect, HealthColor2, 2F);
                    var HealthDisplay = Types.Player[i].Health + "/" + Types.Player[i].MaxHealth;
                    Game1.spriteBatch.DrawString(HealthFont, HealthDisplay, new Vector2(Types.Player[i].X - HealthFont.MeasureString(HealthDisplay).X / 2, healthRect.Bottom + 8), HealthColor2);

                    // Shield
                    RectangleF shieldRect = new RectangleF(Types.Player[i].X - Characters[SpriteNum].Width / 2,
                        (Types.Player[i].Y + Characters[SpriteNum].Height / 2) + 7, Characters[SpriteNum].Width, 6);
                    if (Types.Player[i].Shield > 0 && Types.Player[i].MaxShield > 0)
                    {
                        float percentShield = (Types.Player[i].MaxShield - Types.Player[i].Shield) /
                                              (float)Types.Player[i].MaxShield;
                        Game1.spriteBatch.DrawLine(shieldRect.Left + 2, shieldRect.Bottom - 2,
                            shieldRect.Right - (Characters[SpriteNum].Width * percentShield), shieldRect.Bottom - 2,
                            Color.Goldenrod, 4F);
                        Game1.spriteBatch.DrawRectangle(shieldRect, Color.DarkGoldenrod, 2F);
                        var ShieldDisplay = Types.Player[i].Shield + "/" + Types.Player[i].MaxShield;
                        Game1.spriteBatch.DrawString(ShieldFont, ShieldDisplay,
                            new Vector2(Types.Player[i].X - ShieldFont.MeasureString(ShieldDisplay).X / 2,
                                shieldRect.Bottom + 13), Color.Goldenrod);
                    } else if (Types.Player[i].Shield <= 0 && Types.Player[i].MaxShield > 0)
                    {

                    }
                }
            }
        }

        private static void LoadCharacters(ContentManager manager)
        {
            for (int i = 1; i < Characters.Length; i++)
            {
                Characters[i] = manager.Load<Texture2D>("Characters/" + i);
            }
        }

        public static void DrawHud(ContentManager manager)
        {
            if (GameLogic.PlayerIndex <= -1) return;
            var infoFont = manager.Load<SpriteFont>("GeonBit.UI/themes/hd/fonts/Regular");
            Game1.spriteBatch.Begin();
            Game1.spriteBatch.DrawString(infoFont, (int)Types.Player[GameLogic.PlayerIndex].X / 100 + ":" + (int)Types.Player[GameLogic.PlayerIndex].Y / 100,
                new Vector2(512, 10), Color.DarkBlue);
            Game1.spriteBatch.DrawString(infoFont, GameLogic.PlayerIndex.ToString(), new Vector2(512, 30), Color.Green);
            Game1.spriteBatch.End();
        }

        public static void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            Game1.spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            Game1.spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            Game1.spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                rectangleToDraw.Y,
                thicknessOfBorder,
                rectangleToDraw.Height), borderColor);
            // Draw bottom line
            Game1.spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                rectangleToDraw.Width,
                thicknessOfBorder), borderColor);
        }
    }
}
