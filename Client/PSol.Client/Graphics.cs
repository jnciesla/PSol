using System;
using System.Linq;
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
        public static Texture2D[] Objects = new Texture2D[2];
        public static Texture2D detailsGfx;
        public static Texture2D details;
        public static Texture2D scanner;
        public static Texture2D Shield;
        public static Texture2D pixel;
        public static Texture2D triangle;
        public static Texture2D circle;
        public static Texture2D diamond;
        public static Texture2D star;

        public static Texture2D laserMid;

        public static Texture2D[] Cursors = new Texture2D[3];
        private static Laser _beam;
        private static Laser _bolt;

        public static void InitializeGraphics(ContentManager manager)
        {
            LoadCharacters(manager);
            LoadPlanets(manager);
            LoadCursors(manager);
            LoadObjects(manager);
            laserMid = manager.Load<Texture2D>("Particles/laserMid");
            detailsGfx = manager.Load<Texture2D>("Panels/Equipment2");
            scanner = manager.Load<Texture2D>("Panels/scanner");
            triangle = manager.Load<Texture2D>("Panels/triangle");
            circle = manager.Load<Texture2D>("Panels/circleIco");
            diamond = manager.Load<Texture2D>("Panels/diamondIco");
            star = manager.Load<Texture2D>("Panels/starIco");
            details = manager.Load<Texture2D>("Panels/info");
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

        public static void DrawWeapons(SpriteBatch spriteBatch)
        {
            _bolt?.Update();
            _beam?.Update();
            _bolt?.Draw(spriteBatch);
            _beam?.Draw(spriteBatch);
        }

        public static void DrawSystems()
        {
            var ms = Mouse.GetState();
            float x = ms.X + -Camera.transform.M41;
            float y = ms.Y + -Camera.transform.M42;
            Vector2 position = new Vector2(x, y);

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
                if (Bound.Contains(position) && !Globals.windowOpen)
                {
                    UserInterface.Active.SetCursor(Cursors[1]);
                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.selectedPlanet != GameLogic.Galaxy[i].Id)
                    {
                        GameLogic.selectedPlanet = GameLogic.Galaxy[i].Id;
                        GameLogic.Selected = "";
                        GameLogic.SelectedType = "";
                    }
                }
                if (GameLogic.selectedPlanet == GameLogic.Galaxy[i].Id)
                {
                    DrawBorder(Bound, 1, Color.DarkGray * .25F);
                    Game1.spriteBatch.DrawString(Globals.Font10, GameLogic.Galaxy[i].Name,
                        new Vector2(GameLogic.Galaxy[i].X - Globals.Font10.MeasureString(GameLogic.Galaxy[i].Name).X / 2,
                            GameLogic.Galaxy[i].Y - (int)(Planets[SpriteNum].Height * scale) / 2.0F - 20), Color.AntiqueWhite);
                }
                // Get orbiting planets
                foreach (var planet in GameLogic.Galaxy[i].Planets)
                {
                    var _origin = new Vector2(Planets[planet.Sprite].Width / 2f, Planets[planet.Sprite].Height / 2f);
                    var orbitalX = planet.Orbit * Math.Cos(MathHelper.TwoPi * DateTime.UtcNow.TimeOfDay.TotalMilliseconds / Math.Pow(10, 7)) + GameLogic.Galaxy[i].X;
                    var orbitalY = planet.Orbit * Math.Sin(MathHelper.TwoPi * DateTime.UtcNow.TimeOfDay.TotalMilliseconds / Math.Pow(10, 7)) + GameLogic.Galaxy[i].Y;
                    planet.X = (float)orbitalX;
                    planet.Y = (float)orbitalY;
                    DrawPlanet(planet.Sprite, new Vector2((float)orbitalX, (float)orbitalY), _origin, scale);
                    // OnClick
                    var _Bound = new Rectangle((int)planet.X - (int)(Planets[planet.Sprite].Width * scale) / 2,
                        (int)planet.Y - (int)(Planets[planet.Sprite].Height * scale) / 2, (int)(Planets[planet.Sprite].Width * scale),
                        (int)(Planets[planet.Sprite].Height * scale));
                    if (_Bound.Contains(position) && !Globals.windowOpen)
                    {
                        UserInterface.Active.SetCursor(Cursors[1]);
                        if (ms.LeftButton == ButtonState.Pressed && GameLogic.selectedPlanet != planet.Id)
                        {
                            GameLogic.selectedPlanet = planet.Id;
                            GameLogic.Selected = "";
                            GameLogic.SelectedType = "";
                        }
                    }
                    if (GameLogic.selectedPlanet == planet.Id)
                    {
                        DrawBorder(_Bound, 1, Color.DarkGray * .25F);
                        Game1.spriteBatch.DrawString(Globals.Font10, planet.Name,
                            new Vector2(planet.X - Globals.Font10.MeasureString(planet.Name).X / 2,
                                planet.Y - (int)(Planets[planet.Sprite].Height * scale) / 2.0F - 20), Color.AntiqueWhite);
                    }
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

                    int h = Characters[SpriteNum].Height / 2;
                    Game1.spriteBatch.DrawString(Globals.Font10, Types.Player[i].Name, new Vector2(Types.Player[i].X - Globals.Font10.MeasureString(Types.Player[i].Name).X / 2, Types.Player[i].Y - h - 10), Color.AntiqueWhite);

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
                if (Bound.Contains(position) && !Globals.windowOpen)
                {
                    UserInterface.Active.SetCursor(CursorType.Pointer);
                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != Types.Player[i].Id)
                    {
                        GameLogic.Selected = Types.Player[i].Id;
                        GameLogic.SelectedType = "PLAYER";
                        GameLogic.selectedPlanet = "";
                    }
                }

                if (GameLogic.Selected == Types.Player[i].Id)
                {
                    DrawBorder(Bound, 1, Color.DarkGray * .25F);
                }
            }
        }

        public static void DrawLaser(Vector2 source, Vector2 target)
        {
            _beam = new Laser(source, target, Color.HotPink, 2, false);
            _bolt = new Laser(source, target, Color.Red);
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
                        GameLogic.selectedPlanet = "";
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
            for (int i = 0; i < Planets.Length; i++)
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

        private static void LoadObjects(ContentManager manager)
        {
            for (int i = 1; i < Objects.Length; i++)
            {
                Objects[i] = manager.Load<Texture2D>("Objects/" + i);
            }
        }

        public static void DrawInfo(ContentManager manager)
        {
            if (GameLogic.PlayerIndex <= -1) return;
            int offsetX = 0;
            Game1.spriteBatch.Begin();
            if (Globals.scanner) { offsetX = 200; }

            // Draw details panel
            if (Globals.details)
            {
                MouseState ms = Mouse.GetState();
                Vector2 position = new Vector2(ms.X, ms.Y);
                Game1.spriteBatch.Draw(details, new Vector2(0 + offsetX, Globals.PreferredBackBufferHeight - 200), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                // Planetary info
                if (GameLogic.selectedPlanet != "")
                {
                    var STAR = GameLogic.Galaxy.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);
                    var PLANET = GameLogic.Planets.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);

                    if (STAR != null)
                    {
                        float scale = (float)150 / Planets[4].Width;
                        Game1.spriteBatch.Draw(Planets[4], new Vector2(20 + offsetX, Globals.PreferredBackBufferHeight - 178), null, Color.White * .5F, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        DrawString(Globals.Font12, "Name: " + STAR.Name, 150 + offsetX, Globals.PreferredBackBufferHeight - 175, false, Color.DarkGray);
                        DrawString(Globals.Font10, "Classification: " + "MK - G", 160 + offsetX, Globals.PreferredBackBufferHeight - 155, false, Color.DarkGray);
                        DrawString(Globals.Font10, " Coordinates: " + STAR.X / 100 + ":" + STAR.Y / 100, 160 + offsetX, Globals.PreferredBackBufferHeight - 140, false, Color.DarkGray);
                        DrawString(Globals.Font10, "  Belligerence: " + "Moderate", 160 + offsetX, Globals.PreferredBackBufferHeight - 125, false, Color.DarkGray);
                        DrawString(Globals.Font10, "  Planets: " + STAR.Planets.Count, 160 + offsetX, Globals.PreferredBackBufferHeight - 110, false, Color.DarkGray);
                    }

                    if (PLANET != null)
                    {
                        float scale = (float)150 / Planets[PLANET.Sprite].Width;
                        Game1.spriteBatch.Draw(Planets[PLANET.Sprite], new Vector2(20 + offsetX, Globals.PreferredBackBufferHeight - 178), null, Color.White * .5F, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        DrawString(Globals.Font12, "Name: " + PLANET.Name, 150 + offsetX, Globals.PreferredBackBufferHeight - 175, false, Color.DarkGray);
                        DrawString(Globals.Font10, "Classification: " + "Terrestrial", 160 + offsetX, Globals.PreferredBackBufferHeight - 155, false, Color.DarkGray);
                        DrawString(Globals.Font10, " Coordinates: " + Math.Round(PLANET.X / 100, 2) + ":" + Math.Round(PLANET.Y / 100, 2), 160 + offsetX, Globals.PreferredBackBufferHeight - 140, false, Color.DarkGray);
                        DrawString(Globals.Font10, "  Belligerence: " + "Moderate", 160 + offsetX, Globals.PreferredBackBufferHeight - 125, false, Color.DarkGray);
                    }
                }

                // Mob info
                if (GameLogic.SelectedType == "MOB")
                {
                    var MOB = GameLogic.LocalMobs.FirstOrDefault(m => m.Id == GameLogic.Selected);
                    if (MOB != null)
                    {
                        Game1.spriteBatch.Draw(detailsGfx, new Vector2(82 + offsetX, Globals.PreferredBackBufferHeight - 140), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        Game1.spriteBatch.Draw(Characters[MOB.MobType.Sprite], new Vector2(90 + offsetX, Globals.PreferredBackBufferHeight - 100 - Characters[MOB.MobType.Sprite].Height / 2), null, Color.White * .5F, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        DrawString(Globals.Font10, "Gocharreg DeadlyMetal the death-dealer", 200 + offsetX, Globals.PreferredBackBufferHeight - 175, true, Color.DarkGray);
                        DrawString(Globals.Font8, "Type: " + MOB.MobType.Name, 173 + offsetX, Globals.PreferredBackBufferHeight - 138, false, Color.DarkGray);
                        DrawString(Globals.Font8, "Hull strength: " + MOB.Health / MOB.MobType.MaxHealth * 100 + "%", 173 + offsetX, Globals.PreferredBackBufferHeight - 122, false, Color.DarkGray);
                        if (MOB.MobType.MaxHealth <= 0)
                        {
                            DrawString(Globals.Font8, "Shield strength: " + MOB.Shield / MOB.MobType.MaxShield * 100 + "%", 173 + offsetX, Globals.PreferredBackBufferHeight - 106, false, Color.DarkGray);
                        }
                        else
                        {
                            DrawString(Globals.Font8, "Shield strength: " + "N/A", 173 + offsetX, Globals.PreferredBackBufferHeight - 106, false, Color.DarkGray);
                        }

                        DrawString(Globals.Font8, "Experience level: " + MOB.MobType.Level, 173 + offsetX, Globals.PreferredBackBufferHeight - 90, false, Color.DarkGray);
                        DrawString(Globals.Font8, "Something else: ", 173 + offsetX, Globals.PreferredBackBufferHeight - 74, false, Color.DarkGray);
                    }
                }

                // Click to close
                Rectangle Bound = new Rectangle(386 + offsetX, Globals.PreferredBackBufferHeight - 196, 12, 12);
                if (Bound.Contains(position))
                {
                    UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        Globals.details = false;
                    }
                }
            }
            Game1.spriteBatch.End();
        }

        public static void DrawHud(ContentManager manager)
        {
            if (GameLogic.PlayerIndex <= -1) return;
            const float scale = (float)200 / 2250;
            Vector2 offset = new Vector2(100, Globals.PreferredBackBufferHeight - 100);
            Game1.spriteBatch.Begin();
            // Draw scanner
            if (Globals.scanner)
            {
                MouseState ms = Mouse.GetState();
                Vector2 position = new Vector2(ms.X, ms.Y);
                var _player = Types.Player[GameLogic.PlayerIndex];

                Game1.spriteBatch.Draw(scanner, new Vector2(0, Globals.PreferredBackBufferHeight - 200), null,
                    Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                Game1.spriteBatch.Draw(triangle, offset, null, Color.White, Types.Player[GameLogic.PlayerIndex].Rotation, new Vector2(8, 8), 1, SpriteEffects.None, 0);

                // Look for mobs and display if within range
                if (GameLogic.LocalMobs != null && GameLogic.LocalMobs.Count > 0)
                {
                    foreach (var m in GameLogic.LocalMobs)
                    {
                        Vector2 mobPosition = offset + new Vector2(((m.X + -Camera.transform.M41) - (_player.X + -Camera.transform.M41)) * scale, ((m.Y + -Camera.transform.M42) - (_player.Y - Camera.transform.M42)) * scale);
                        if (mobPosition.X > 17 && mobPosition.X < 185 && mobPosition.Y > Globals.PreferredBackBufferHeight - 183 && mobPosition.Y < Globals.PreferredBackBufferHeight - 16)
                        {
                            Game1.spriteBatch.Draw(diamond, mobPosition, null, new Color(88, 170, 76), m.Rotation, new Vector2(6, 6), 1, SpriteEffects.None, 0);
                            Rectangle bound = new Rectangle(mobPosition.ToPoint() - new Point(6), new Point(12));
                            if (bound.Contains(position))
                            {
                                UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                                if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != m.Id)
                                {
                                    GameLogic.Selected = m.Id;
                                    GameLogic.SelectedType = "MOB";
                                    GameLogic.selectedPlanet = "";
                                }
                            }
                            if (GameLogic.Selected == m.Id)
                            {
                                DrawBorder(bound, 1, Color.DarkGreen * .25F);
                            }
                        }
                    }
                }

                // Look for planets and stars and display if within range
                if (GameLogic.Galaxy != null)
                {
                    foreach (var obj in GameLogic.Galaxy.Where(p =>
                        p.X + -Camera.transform.M41 >= _player.X + -Camera.transform.M41 - 2000 &&
                        p.X + -Camera.transform.M41 <= _player.X + -Camera.transform.M41 + 2000 &&
                        p.Y + -Camera.transform.M42 >= _player.Y + -Camera.transform.M42 - 2000 &&
                        p.Y + -Camera.transform.M42 <= _player.Y + -Camera.transform.M42 + 2000))
                    {
                        Vector2 starPosition =
                            offset + new Vector2(
                                ((obj.X + -Camera.transform.M41) - (_player.X + -Camera.transform.M41)) * scale,
                                ((obj.Y + -Camera.transform.M42) - (_player.Y - Camera.transform.M42)) * scale);
                        if (starPosition.X > 17 && starPosition.X < 185 &&
                            starPosition.Y > Globals.PreferredBackBufferHeight - 183 &&
                            starPosition.Y < Globals.PreferredBackBufferHeight - 16)
                        {
                            Game1.spriteBatch.Draw(star, starPosition, null, Color.White, 0, new Vector2(6, 6), 1, SpriteEffects.None, 0);
                            Rectangle bound = new Rectangle(starPosition.ToPoint() - new Point(6), new Point(12));
                            if (bound.Contains(position))
                            {
                                UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                                if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != obj.Id)
                                {
                                    GameLogic.selectedPlanet = obj.Id;
                                    GameLogic.Selected = "";
                                    GameLogic.SelectedType = "";
                                }
                            }
                            if (GameLogic.selectedPlanet == obj.Id)
                            {
                                DrawBorder(bound, 1, Color.DarkGreen * .25F);
                            }
                        }

                        foreach (var sat in obj.Planets)
                        {
                            Vector2 satPosition =
                                offset + new Vector2(
                                    ((sat.X + -Camera.transform.M41) - (_player.X + -Camera.transform.M41)) * scale,
                                    ((sat.Y + -Camera.transform.M42) - (_player.Y - Camera.transform.M42)) * scale);
                            if (satPosition.X > 17 && satPosition.X < 185 &&
                                satPosition.Y > Globals.PreferredBackBufferHeight - 183 &&
                                satPosition.Y < Globals.PreferredBackBufferHeight - 16)
                            {
                                Game1.spriteBatch.Draw(circle, satPosition, null, Color.White, 0, new Vector2(6, 6), 1, SpriteEffects.None, 0);
                                Rectangle bound = new Rectangle(satPosition.ToPoint() - new Point(6), new Point(12));
                                if (bound.Contains(position))
                                {
                                    UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != sat.Id)
                                    {
                                        GameLogic.selectedPlanet = sat.Id;
                                        GameLogic.Selected = "";
                                        GameLogic.SelectedType = "";
                                    }
                                }
                                if (GameLogic.selectedPlanet == sat.Id)
                                {
                                    DrawBorder(bound, 1, Color.DarkGreen * .25F);
                                }
                            }
                        }
                    }
                }

                // Click to close
                Rectangle Bound = new Rectangle(186, Globals.PreferredBackBufferHeight - 196, 12, 12);
                if (Bound.Contains(position))
                {
                    UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        Globals.scanner = false;
                    }
                }
            }
            DrawString(Globals.Font10, (int)Types.Player[GameLogic.PlayerIndex].X / 100 + ":" + (int)Types.Player[GameLogic.PlayerIndex].Y / 100, -1, 10, true, Color.DimGray);
            if (GameLogic.Navigating)
            {
                DrawString(Globals.Font8, "Navigating to: " + (int)GameLogic.Destination.X / 100 + ":" + (int)GameLogic.Destination.Y / 100, -1, 30, true, Color.DimGray);
                DrawString(Globals.Font8, "Distance remaining: " + ((int)GameLogic.distance / 100) + "AU", -1, 50, true, Color.DimGray);
            }
            else
            {
                DrawString(Globals.Font8, GameLogic.PlayerIndex.ToString(), -1, 30, true, Color.Green);
            }

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

        public static void DrawString(SpriteFont font, string text, float X, float Y, bool align, Color color)
        {
            if ((int)X == -1) { X = Globals.PreferredBackBufferWidth / 2.0F; }
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new Vector2(X, Y);
            if (align)
            {
                pos.X -= size.X / 2;
            }
            Game1.spriteBatch.DrawString(font, text, pos, color);
        }
    }
}
