using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bindings;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using GeonBit.UI;

namespace PSol.Client
{
    internal class Graphics
    {
        public static Texture2D[] Characters = new Texture2D[3];
        public static Texture2D[] Planets = new Texture2D[5];
        public static Texture2D scanner;
        public static Texture2D Shield;
        public static Texture2D pixel;
        public static Texture2D triangle;

        public static Texture2D laserMid;

        public static Texture2D[] Cursors = new Texture2D[3];

        public static void InitializeGraphics(ContentManager manager)
        {
            LoadCharacters(manager);
            LoadPlanets(manager);
            LoadCursors(manager);
            laserMid = manager.Load<Texture2D>("Particles/laserMid");
            scanner = manager.Load<Texture2D>("Panels/scanner");
            triangle = manager.Load<Texture2D>("Panels/triangle");
            pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        public static void RenderPlayers()
        {
            DrawPlayers();
            if (GameLogic.LocalMobs != null && GameLogic.LocalMobs.Count > 0)
            {
                DrawMobs();
            }
        }

        private static void DrawSprite(int sprite, Vector2 position, float rotation, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Characters[sprite], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        private static void DrawPlanet(int sprite, Vector2 position, Vector2 origin, float scale)
        {
            Game1.spriteBatch.Draw(Planets[sprite], position, null, Color.White, Globals.PlanetaryRotation, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawSystems()
        {
            int SpriteNum = 4;
            float scale = .3F;
            for (var i = 0; i != GameLogic.Galaxy.Count; i++)
            {
                var origin = new Vector2(Planets[SpriteNum].Width / 2f, Planets[SpriteNum].Height / 2f);
                DrawPlanet(SpriteNum, new Vector2(GameLogic.Galaxy[i].X, GameLogic.Galaxy[i].Y), origin, scale);
                // OnClick
                var Bound = new Rectangle((int)GameLogic.Galaxy[i].X - (int)(Planets[SpriteNum].Width * scale) / 2,
                    (int)GameLogic.Galaxy[i].Y - (int)(Planets[SpriteNum].Height * scale) / 2, (int)(Planets[SpriteNum].Width * scale),
                    (int)(Planets[SpriteNum].Height * scale));
                var ms = Mouse.GetState();
                float x = ms.X + -Camera.transform.M41;
                float y = ms.Y + -Camera.transform.M42;
                Vector2 position = new Vector2(x, y);
                if (Bound.Contains(position))
                {
                    UserInterface.Active.SetCursor(Cursors[1]);
                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.selectedPlanet != i)
                    {
                        GameLogic.selectedPlanet = i;
                        GameLogic.Selected = "";
                        GameLogic.SelectedType = "";
                    }
                }

                if (GameLogic.selectedPlanet == i)
                {
                    DrawBorder(Bound, 1, Color.DarkGray * .25F);
                    Game1.spriteBatch.DrawString(Globals.Font10, GameLogic.Galaxy[i].Name,
                        new Vector2(GameLogic.Galaxy[i].X - Globals.Font10.MeasureString(GameLogic.Galaxy[i].Name).X / 2,
                            GameLogic.Galaxy[i].Y - (int)(Planets[SpriteNum].Height * scale) / 2.0F - 20), Color.AntiqueWhite);
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
                if (Globals.Details1 || Globals.Details2 || GameLogic.Selected == Types.Player[i].Id)
                {
                    // Draw shield orb overlay when viewing details but not selected only on ourselves and when it's not depleted
                    if (Types.Player[i].Shield > 0 && GameLogic.Selected != Types.Player[i].Id && GameLogic.PlayerIndex == i)
                    {
                        Game1.spriteBatch.Draw(Shield, new Vector2(Types.Player[i].X, Types.Player[i].Y), null,
                            Color.LightBlue * 0.25f, Types.Player[i].Rotation * -1,
                            new Vector2(Shield.Width / 2f, Shield.Height / 2f), 1, SpriteEffects.None, 0);
                    }

                    Game1.spriteBatch.DrawString(Globals.Font10, Types.Player[i].Name, new Vector2(Types.Player[i].X - Globals.Font10.MeasureString(Types.Player[i].Name).X / 2, Types.Player[i].Y - (Characters[SpriteNum].Height / 2) - 10), Color.AntiqueWhite);

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
                    RectangleF healthRect = new RectangleF(Types.Player[i].X - Characters[SpriteNum].Width / 2.0F,
                        Types.Player[i].Y + Characters[SpriteNum].Height / 2.0F, Characters[SpriteNum].Width, 6);
                    Game1.spriteBatch.DrawLine(healthRect.Left + 2, healthRect.Bottom - 2, healthRect.Right - (Characters[SpriteNum].Width * percentHealth), healthRect.Bottom - 2, HealthColor1, 4F);
                    Game1.spriteBatch.DrawRectangle(healthRect, HealthColor2, 2F);
                    var HealthDisplay = "";
                    if (Globals.Details1 || GameLogic.Selected == Types.Player[i].Id)
                    {
                        HealthDisplay = Types.Player[i].Health + "/" + Types.Player[i].MaxHealth;
                    }
                    else if (Globals.Details2)
                    {
                        HealthDisplay = (int)((float)Types.Player[i].Health / Types.Player[i].MaxHealth * 100) + "%";
                    }

                    Game1.spriteBatch.DrawString(Globals.Font10, HealthDisplay, new Vector2(Types.Player[i].X - Globals.Font10.MeasureString(HealthDisplay).X / 2, healthRect.Bottom + 8), HealthColor2);

                    // Shield
                    RectangleF shieldRect = new RectangleF(Types.Player[i].X - Characters[SpriteNum].Width / 2.0F,
                        (Types.Player[i].Y + Characters[SpriteNum].Height / 2.0F) + 7, Characters[SpriteNum].Width, 6);
                    if (Types.Player[i].Shield > 0 && Types.Player[i].MaxShield > 0)
                    {
                        float percentShield = (Types.Player[i].MaxShield - Types.Player[i].Shield) /
                                              (float)Types.Player[i].MaxShield;
                        Game1.spriteBatch.DrawLine(shieldRect.Left + 2, shieldRect.Bottom - 2,
                            shieldRect.Right - (Characters[SpriteNum].Width * percentShield), shieldRect.Bottom - 2,
                            Color.Goldenrod, 4F);
                        Game1.spriteBatch.DrawRectangle(shieldRect, Color.DarkGoldenrod, 2F);
                        var ShieldDisplay = "";
                        if (Globals.Details1 || GameLogic.Selected == Types.Player[i].Id)
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
                float x = ms.X + -Camera.transform.M41;
                float y = ms.Y + -Camera.transform.M42;
                Vector2 position = new Vector2(x, y);
                if (Bound.Contains(position))
                {
                    UserInterface.Active.SetCursor(CursorType.Pointer);
                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != Types.Player[i].Id)
                    {
                        GameLogic.Selected = Types.Player[i].Id;
                        GameLogic.SelectedType = "PLAYER";
                        GameLogic.selectedPlanet = -1;
                    }
                }

                if (GameLogic.Selected == Types.Player[i].Id)
                {
                    DrawBorder(Bound, 1, Color.DarkGray * .25F);
                }
            }
        }

        private static void DrawMobs()
        {
            if (GameLogic.PlayerIndex <= -1) return;

            foreach (var mob in GameLogic.LocalMobs)
            {
                var spriteNum = mob.MobType.Sprite;
                var origin = new Vector2(Characters[spriteNum].Width / 2f, Characters[spriteNum].Height / 2f);
                DrawSprite(spriteNum, new Vector2(mob.X, mob.Y), mob.Rotation, origin);
                // Draw shield orb on and fade off when being attacked.  Rotation can potentially be extrapolated from angle of attack to make the 'light' side face target
                // Game1.spriteBatch.Draw(Shield, new Vector2(Types.Player[i].X, Types.Player[i].Y), null, Color.LightBlue * 0.25f, Types.Player[i].Rotation * -1, new Vector2(Shield.Width / 2f, Shield.Height / 2f), 1, SpriteEffects.None, 0);

                // Draw status stuff
                if (Globals.Details1 || Globals.Details2 || GameLogic.Selected == mob.Id)
                {
                    // Draw shield orb overlay when viewing details but not selected and when it's not depleted
                    if (mob.Shield > 0 && GameLogic.Selected != mob.Id)
                    {
                        Game1.spriteBatch.Draw(Shield, new Vector2(mob.X, mob.Y), null,
                            Color.LightBlue * 0.25f, mob.Rotation * -1,
                            new Vector2(Shield.Width / 2f, Shield.Height / 2f), 1, SpriteEffects.None, 0);
                    }

                    var height = Characters[spriteNum].Height / 2;
                    Game1.spriteBatch.DrawString(Globals.Font10, mob.MobType.Name, new Vector2(mob.X - Globals.Font10.MeasureString(mob.MobType.Name).X / 2,
                        mob.Y - height - 10), Color.AntiqueWhite);

                    // Health
                    var healthColor1 = Color.Green;
                    var healthColor2 = Color.DarkGreen;
                    var percentHealth = (mob.MobType.MaxHealth - mob.Health) / mob.MobType.MaxHealth;
                    // Percent Health is percentage missing, so colors are inverted:
                    if (percentHealth > 0.75)
                    {
                        healthColor1 = Color.Red;
                        healthColor2 = Color.DarkRed;
                    }
                    else if (percentHealth < 0.75 && percentHealth > 0.25)
                    {
                        healthColor1 = Color.Orange;
                        healthColor2 = Color.DarkOrange;
                    }
                    else if (percentHealth < 0.25)
                    {
                        healthColor1 = Color.Green;
                        healthColor2 = Color.DarkGreen;
                    }
                    var healthRect = new RectangleF(mob.X - Characters[spriteNum].Width / 2.0F,
                        mob.Y + Characters[spriteNum].Height / 2.0F, Characters[spriteNum].Width, 6);
                    Game1.spriteBatch.DrawLine(healthRect.Left + 2, healthRect.Bottom - 2, healthRect.Right - (Characters[spriteNum].Width * percentHealth),
                        healthRect.Bottom - 2, healthColor1, 4F);
                    Game1.spriteBatch.DrawRectangle(healthRect, healthColor2, 2F);
                    var healthDisplay = "";
                    if (Globals.Details1 || GameLogic.Selected == mob.Id)
                    {
                        healthDisplay = mob.Health + "/" + mob.MobType.MaxHealth;
                    }
                    else if (Globals.Details2)
                    {
                        healthDisplay = mob.Health / mob.MobType.MaxHealth * 100 + "%";
                    }

                    Game1.spriteBatch.DrawString(Globals.Font10, healthDisplay, new Vector2(mob.X - Globals.Font10.MeasureString(healthDisplay).X / 2,
                        healthRect.Bottom + 8), healthColor2);

                    // Shield
                    var shieldRect = new RectangleF(mob.X - Characters[spriteNum].Width / 2.0F,
                        mob.Y + Characters[spriteNum].Height / 2.0F + 7, Characters[spriteNum].Width, 6);
                    if (mob.Shield > 0 && mob.MobType.MaxShield > 0)
                    {
                        var percentShield = (mob.MobType.MaxShield - mob.Shield) / mob.MobType.MaxShield;
                        Game1.spriteBatch.DrawLine(shieldRect.Left + 2, shieldRect.Bottom - 2,
                            shieldRect.Right - (Characters[spriteNum].Width * percentShield), shieldRect.Bottom - 2,
                            Color.Goldenrod, 4F);
                        Game1.spriteBatch.DrawRectangle(shieldRect, Color.DarkGoldenrod, 2F);
                        var shieldDisplay = "";
                        if (Globals.Details1 || GameLogic.Selected == mob.Id)
                        {
                            shieldDisplay = mob.Shield + "/" + mob.MobType.MaxShield;
                        }
                        else if (Globals.Details2)
                        {
                            shieldDisplay = (int)(mob.Shield / mob.MobType.MaxShield * 100) + "%";
                        }

                        Game1.spriteBatch.DrawString(Globals.Font8, shieldDisplay,
                            new Vector2(mob.X - Globals.Font8.MeasureString(shieldDisplay).X / 2,
                                shieldRect.Bottom + 13), Color.Goldenrod);
                    }
                    else if (mob.Shield <= 0 && mob.MobType.MaxShield > 0)
                    {
                        Game1.spriteBatch.DrawString(Globals.Font8, "Shield depleted", new Vector2(mob.X - Globals.Font8.MeasureString("Shield Depleted!").X / 2,
                            shieldRect.Bottom + 13), Color.DarkGoldenrod);
                    }
                }

                // OnClick
                var bound = new Rectangle((int)mob.X - Characters[spriteNum].Width / 2, (int)mob.Y - Characters[spriteNum].Height / 2,
                    Characters[spriteNum].Width, Characters[spriteNum].Height);
                var ms = Mouse.GetState();
                var x = ms.X + -Camera.transform.M41;
                var y = ms.Y + -Camera.transform.M42;
                var position = new Vector2(x, y);
                if (bound.Contains(position))
                {
                    UserInterface.Active.SetCursor(CursorType.Pointer);
                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != mob.Id)
                    {
                        GameLogic.Selected = mob.Id;
                        GameLogic.SelectedType = "MOB";
                        GameLogic.selectedPlanet = -1;
                    }
                }

                if (GameLogic.Selected == mob.Id)
                {
                    DrawBorder(bound, 1, Color.DarkGray * .25F);
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

        private static void LoadCursors(ContentManager manager)
        {
            for (int i = 1; i < Cursors.Length; i++)
            {
                Cursors[i] = manager.Load<Texture2D>("Cursors/" + i);
            }
        }

        public static void DrawHud(ContentManager manager)
        {
            if (GameLogic.PlayerIndex <= -1) return;
            Game1.spriteBatch.Begin();
            // Draw scanner
            if (Globals.scanner)
            {
                Game1.spriteBatch.Draw(scanner, new Vector2(0, Globals.PreferredBackBufferHeight - 200), null,
                    Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                Game1.spriteBatch.Draw(triangle, new Vector2(100, Globals.PreferredBackBufferHeight - 100), null,
                    Color.White, Types.Player[GameLogic.PlayerIndex].Rotation, new Vector2(8, 8), 1, SpriteEffects.None, 0);

                // Click to close
                Rectangle Bound = new Rectangle(186, Globals.PreferredBackBufferHeight - 196, 12, 12);
                MouseState ms = Mouse.GetState();
                Vector2 position = new Vector2(ms.X, ms.Y);
                if (Bound.Contains(position))
                {
                    UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        Globals.scanner = false;

                    };
                }

            }

            Game1.spriteBatch.DrawString(Globals.Font10, (int)Types.Player[GameLogic.PlayerIndex].X / 100 + ":" + (int)Types.Player[GameLogic.PlayerIndex].Y / 100,
                new Vector2(Globals.PreferredBackBufferWidth / 2.0f, 10), Color.DimGray);
            Game1.spriteBatch.DrawString(Globals.Font10, GameLogic.PlayerIndex.ToString(), new Vector2(512, 30), Color.Green);
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
