using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bindings;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Client
{
    internal class Graphics
    {
        public static Texture2D[] Characters = new Texture2D[2];
        public static Texture2D[] Planets = new Texture2D[2];
        public static Texture2D Shield;
        public static Texture2D pixel;

        public static void InitializeGraphics(ContentManager manager)
        {
            LoadCharacters(manager);
            LoadPlanets(manager);
            pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        public static void RenderGraphics()
        {
            DrawSystems();
            DrawPlayers();
        }

        private static void DrawSprite(int sprite, Vector2 position, float rotation, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Characters[sprite], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        private static void DrawPlanet(int sprite, Vector2 position, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Planets[sprite], position, null, Color.White, Globals.PlanetaryRotation, origin, 0.2F, SpriteEffects.None, 0);
        }

        private static void DrawSystems()
        {
            int SpriteNum = 1;
            var origin = new Vector2(Planets[SpriteNum].Width / 2f, Planets[SpriteNum].Height / 2f);
            DrawPlanet(SpriteNum, new Vector2(1000, 1000), origin);
            // OnClick
            var Bound = new Rectangle(1000 - Characters[SpriteNum].Width / 2, 1000 - Characters[SpriteNum].Height / 2, Characters[SpriteNum].Width, Characters[SpriteNum].Height);
            var ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                float x = ms.X + (-Camera.transform.M41);
                float y = ms.Y + (-Camera.transform.M42);
                Vector2 position = new Vector2(x, y);
                if (Bound.Contains(position))
                {
                    InterfaceGUI.AddChats("test",Color.MediumVioletRed);
                }
            }
        }

        private static void DrawPlayers()
        {
            if (GameLogic.PlayerIndex <= -1) return;

            int SpriteNum = 1;

            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                if (Types.Player[i].X.CompareTo(0.0f) == 0) continue;
                var origin = new Vector2(Characters[SpriteNum].Width / 2f, Characters[SpriteNum].Height / 2f);
                DrawSprite(SpriteNum, new Vector2(Types.Player[i].X, Types.Player[i].Y), Types.Player[i].Rotation, origin);
                // Draw shield orb on and fade off when being attacked.  Rotation can potentially be extrapolated from angle of attack to make the 'light' side face target
                // Game1.spriteBatch.Draw(Shield, new Vector2(Types.Player[i].X, Types.Player[i].Y), null, Color.LightBlue * 0.25f, Types.Player[i].Rotation * -1, new Vector2(Shield.Width / 2f, Shield.Height / 2f), 1, SpriteEffects.None, 0);

                // Draw status stuff
                if (Globals.Details1 || Globals.Details2 || Globals.Selected == i)
                {
                    // Draw shield orb overlay when viewing details but not selected only on ourselves and when it's not depleted
                    if (Types.Player[i].Shield > 0 && Globals.Selected != i && GameLogic.PlayerIndex == i)
                    {
                        Game1.spriteBatch.Draw(Shield, new Vector2(Types.Player[i].X, Types.Player[i].Y), null,
                            Color.LightBlue * 0.25f, Types.Player[i].Rotation * -1,
                            new Vector2(Shield.Width / 2f, Shield.Height / 2f), 1, SpriteEffects.None, 0);
                    }

                    Game1.spriteBatch.DrawString(Globals.Font10, Types.Player[i].Name, new Vector2(Types.Player[i].X - Globals.Font10.MeasureString(Types.Player[i].Name).X / 2, Types.Player[i].Y - (Characters[SpriteNum].Height /2) - 10), Color.AntiqueWhite);

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
                    var HealthDisplay = "";
                    if (Globals.Details1 || Globals.Selected == i)
                    {
                        HealthDisplay = Types.Player[i].Health + "/" + Types.Player[i].MaxHealth;
                    }
                    else if (Globals.Details2)
                    {
                        HealthDisplay = (int)((float)Types.Player[i].Health / Types.Player[i].MaxHealth * 100) + "%";
                    }

                    Game1.spriteBatch.DrawString(Globals.Font10, HealthDisplay, new Vector2(Types.Player[i].X - Globals.Font10.MeasureString(HealthDisplay).X / 2, healthRect.Bottom + 8), HealthColor2);

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
                        var ShieldDisplay = "";
                        if (Globals.Details1 || Globals.Selected == i)
                        {
                            ShieldDisplay = Types.Player[i].Shield + "/" + Types.Player[i].MaxShield;
                        }
                        else if (Globals.Details2)
                        {
                            ShieldDisplay = (int)((float)Types.Player[i].Shield / Types.Player[i].MaxShield * 100) + "%";
                        }

                        Game1.spriteBatch.DrawString(Globals.Font8, ShieldDisplay,
                            new Vector2(Types.Player[i].X - Globals.Font8.MeasureString(ShieldDisplay).X / 2,
                                shieldRect.Bottom + 13), Color.Goldenrod);
                    }
                    else if (Types.Player[i].Shield <= 0 && Types.Player[i].MaxShield > 0)
                    {
                        Game1.spriteBatch.DrawString(Globals.Font8, "Shield depleted", new Vector2(Types.Player[i].X - Globals.Font8.MeasureString("Shield Depleted!").X / 2,
                            shieldRect.Bottom + 13), Color.DarkGoldenrod);
                    }
                }
                // OnClick
                Rectangle Bound = new Rectangle((int)Types.Player[i].X - Characters[SpriteNum].Width / 2, (int)Types.Player[i].Y - Characters[SpriteNum].Height / 2, Characters[SpriteNum].Width, Characters[SpriteNum].Height);
                MouseState ms = Mouse.GetState();
                if (ms.LeftButton == ButtonState.Pressed && Globals.Selected != i)
                {
                    float x = ms.X + (-Camera.transform.M41);
                    float y = ms.Y + (-Camera.transform.M42);
                    Vector2 position = new Vector2(x, y);
                    if (Bound.Contains(position))
                    {
                        Globals.Selected = i;
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

            Shield = manager.Load<Texture2D>("Shield");
        }

        private static void LoadPlanets(ContentManager manager)
        {
            for (int i = 1; i < Planets.Length; i++)
            {
                Planets[i] = manager.Load<Texture2D>("Planets/" + i);
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
