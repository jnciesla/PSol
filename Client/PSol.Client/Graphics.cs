using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using PSol.Data.Models;

namespace PSol.Client
{
    internal class Graphics
    {
        private static readonly ClientTCP ctcp = new ClientTCP();
        private static readonly KeyControl KC = new KeyControl();
        private static DateTime doubleClick;

        public static Texture2D[] Characters = new Texture2D[3];
        public static Texture2D[] Planets = new Texture2D[5];
        public static Texture2D[] Objects = new Texture2D[3];
        public static List<Texture2D> smallExplosionTextures;
        public static List<Texture2D> largeExplosionTextures;
        public static List<Texture2D> levelUpTextures;
        public static Texture2D experienceBar;
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
        public static Texture2D splash;
        public static Texture2D spark;
        public static Texture2D shockwave;
        public static Texture2D fire;
        public static Texture2D lootContainer;
        public static Texture2D nebula;

        public static Texture2D[] Cursors = new Texture2D[3];
        private static Laser _beam;
        private static Laser _bolt;

        public static void InitializeGraphics(ContentManager manager)
        {
            LoadCharacters(manager);
            LoadPlanets(manager);
            LoadCursors(manager);
            LoadObjects(manager);
            spark = manager.Load<Texture2D>("Particles/circle");
            fire = manager.Load<Texture2D>("Particles/expl1");
            laserMid = manager.Load<Texture2D>("Particles/laserMid");
            detailsGfx = manager.Load<Texture2D>("Panels/Equipment2");
            scanner = manager.Load<Texture2D>("Panels/scanner");
            triangle = manager.Load<Texture2D>("Panels/triangle");
            circle = manager.Load<Texture2D>("Panels/circleIco");
            diamond = manager.Load<Texture2D>("Panels/diamondIco");
            star = manager.Load<Texture2D>("Panels/starIco");
            details = manager.Load<Texture2D>("Panels/info");
            splash = manager.Load<Texture2D>("Panels/Splash");
            lootContainer = manager.Load<Texture2D>("Objects/loot");
            shockwave = manager.Load<Texture2D>("Particles/shockwave");
            nebula = manager.Load<Texture2D>("Overlays/Nebula");
            experienceBar = manager.Load<Texture2D>("Panels/XPBar");
            Shield = manager.Load<Texture2D>("Shield");
            pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
            // Particle small explosion textures
            smallExplosionTextures = new List<Texture2D>
            {
                fire, spark, spark, spark, spark, spark, spark, spark, spark, spark, spark, spark
            };
            // Particle large explosion textures
            largeExplosionTextures = new List<Texture2D>
            {
                fire, fire, spark, spark, spark
            };
            // Particle level up textures
            levelUpTextures = new List<Texture2D>
            {
                star, spark, spark, spark
            };
        }

        public static void RenderPlayers()
        {
            Globals.HoveringMob = false;
            if (GameLogic.LocalMobs != null && GameLogic.LocalMobs.Count > 0)
            {
                DrawMobs();
            }
            DrawPlayers();
        }

        public static void RenderShadows()
        {
            var SpriteNum = 1;
            if (GameLogic.LocalMobs != null && GameLogic.LocalMobs.Count > 0)
            {
                foreach (var mob in GameLogic.LocalMobs)
                {
                    var spriteNum = mob.MobType.Sprite;
                    var origin = new Vector2(Characters[spriteNum].Width / 2f, Characters[spriteNum].Height / 2f);
                    DrawShadow(spriteNum, new Vector2(mob.X, mob.Y), mob.Rotation, origin);
                }
            }

            for (var i = 1; i != Constants.MAX_PLAYERS; i++)
            {
                if (Types.Player[i].X.CompareTo(0.0f) == 0) continue;
                var origin = new Vector2(Characters[SpriteNum].Width / 2f, Characters[SpriteNum].Height / 2f);
                DrawShadow(SpriteNum, new Vector2(Types.Player[i].X, Types.Player[i].Y), Types.Player[i].Rotation, origin);
            }
        }

        public static void RenderObjects()
        {
            if (GameLogic.LocalLoot != null && GameLogic.LocalLoot.Count > 0)
            {
                DrawTrash();
            }

            if (GameLogic.RealLoot != null && GameLogic.RealLoot.Count > 0)
            {
                DrawLoot();
            }
        }

        private static void DrawTrash()
        {
            if (GameLogic.PlayerIndex <= -1) return;
            Globals.HoveringItem = false;
            var ms = Mouse.GetState();
            var position = Game1.Camera.ScreenToWorld(new Vector2(ms.X, ms.Y));
            if (Globals.cursorOverride || Globals.HoveringGUI) position = Vector2.Zero;

            foreach (var loot in GameLogic.LocalLoot)
            {
                var item = GameLogic.Items.FirstOrDefault(i => i.Id == loot.ItemId) ?? new Item();
                Game1.spriteBatch.Draw(Objects[item.Image], new Vector2(loot.X, loot.Y), null, COLOR(item.Color), Globals.PlanetaryRotation * 10, new Vector2(32, 32), .5F, SpriteEffects.None, 0);

                // Mouse behaviors
                var Bound = new Rectangle((int)loot.X - 32 / 2, (int)loot.Y - 32 / 2, 32, 32);
                if (KC.CheckAlt() && !Bound.Contains(position))
                {
                    DrawString(Globals.Font8, item.Name, loot.X, loot.Y - 20, true, Color.Gray);
                    if (loot.Quantity > 1)
                    {
                        DrawString(Globals.Font8, loot.Quantity.ToString(), loot.X, loot.Y + 20, true, Color.Gray);
                    }
                }
                if (!Bound.Contains(position) || Globals.windowOpen) continue;
                Globals.HoveringItem = true;
                UserInterface.Active.SetCursor(Cursors[1]);
                if (KC.DoubleClick())
                {
                    ctcp.TransactItem(loot.Id, Types.Player[GameLogic.PlayerIndex].Id);
                }
                DrawString(Globals.Font8, item.Name, loot.X, loot.Y - 20, true, Color.Gray);
                if (loot.Quantity > 1)
                {
                    DrawString(Globals.Font8, loot.Quantity.ToString(), loot.X, loot.Y + 20, true, Color.Gray);
                }
            }
        }

        private static void DrawLoot()
        {
            if (GameLogic.PlayerIndex <= -1) return;
            Globals.HoveringItem = false;
            var ms = Mouse.GetState();
            var position = Game1.Camera.ScreenToWorld(new Vector2(ms.X, ms.Y));
            if (Globals.cursorOverride || Globals.HoveringGUI) position = Vector2.Zero;
            foreach (var loot in GameLogic.RealLoot)
            {
                Game1.spriteBatch.Draw(lootContainer, new Vector2(loot.X, loot.Y), null, Color.White, Globals.PlanetaryRotation * 50, new Vector2(32, 32), .5F, SpriteEffects.None, 0);
                // Mouse behaviors
                var Bound = new Rectangle((int)loot.X - 32 / 2, (int)loot.Y - 32 / 2, 32, 32);
                if (!Bound.Contains(position) || Globals.windowOpen) continue;
                Globals.HoveringItem = true;
                UserInterface.Active.SetCursor(Cursors[1]);
                if (KC.DoubleClick())
                {
                    Globals.selectedLoot = loot.Id;
                    Game1.IGUI.PopulateLoot(Globals.selectedLoot);
                    Game1.IGUI.DisplayLootDetails();
                    MenuManager.ChangeMenu(MenuManager.Menu.Loot);
                }
            }
        }

        private static void DrawSprite(int sprite, Vector2 position, float rotation, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Characters[sprite], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        private static void DrawShadow(int sprite, Vector2 position, float rotation, Vector2 origin)
        {
            Game1.spriteBatch.Draw(Characters[sprite], position + new Vector2(5, 5), null, Color.Black * .5F, rotation, origin, 1, SpriteEffects.None, 0);
            Game1.spriteBatch.Draw(Characters[sprite], position + new Vector2(3, 3), null, Color.Black * .5F, rotation, origin, 1, SpriteEffects.None, 0);
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
            Globals.HoveringPlanet = false;
            var ms = Mouse.GetState();
            var position = Game1.Camera.ScreenToWorld(new Vector2(ms.X, ms.Y));
            if (Globals.cursorOverride || Globals.Flying || Globals.HoveringGUI) position = Vector2.Zero;
            const float scale = .3F;
            foreach (var Star in GameLogic.Galaxy)
            {
                var origin = new Vector2(Planets[4].Width / 2f, Planets[4].Height / 2f);
                DrawPlanet(4, new Vector2(Star.X, Star.Y), origin, scale);
                // OnClick
                var Bound = new Rectangle((int)Star.X - (int)(Planets[4].Width * scale) / 2,
                    (int)Star.Y - (int)(Planets[4].Height * scale) / 2, (int)(Planets[4].Width * scale),
                    (int)(Planets[4].Height * scale));
                if (Bound.Contains(position) && !Globals.windowOpen && !Globals.HoveringMob && !Globals.HoveringItem)
                {
                    Globals.HoveringPlanet = true;
                    UserInterface.Active.SetCursor(Cursors[1]);
                    if (KC.Click())
                    {
                        GameLogic.selectedPlanet = Star.Id;
                        GameLogic.Selected = "";
                        GameLogic.SelectedType = "";
                    }
                }
                if (GameLogic.selectedPlanet == Star.Id)
                {
                    DrawBorder(Bound, 1, Color.DarkGray * .25F);
                    DrawString(Globals.Font10, Star.Name, Star.X, Star.Y - (int)(Planets[4].Height * scale) / 2.0F - 20, true, Color.AntiqueWhite);
                }
                // Get orbiting planets
                foreach (var planet in Star.Planets)
                {
                    var _origin = new Vector2(Planets[planet.Sprite].Width / 2f, Planets[planet.Sprite].Height / 2f);
                    var orbitalX = planet.Orbit * Math.Cos(MathHelper.TwoPi * DateTime.UtcNow.TimeOfDay.TotalMilliseconds / Math.Pow(10, 7)) + Star.X;
                    var orbitalY = planet.Orbit * Math.Sin(MathHelper.TwoPi * DateTime.UtcNow.TimeOfDay.TotalMilliseconds / Math.Pow(10, 7)) + Star.Y;
                    planet.X = (float)orbitalX;
                    planet.Y = (float)orbitalY;
                    DrawPlanet(planet.Sprite, new Vector2((float)orbitalX, (float)orbitalY), _origin, scale);
                    // OnClick
                    var _Bound = new Rectangle((int)planet.X - (int)(Planets[planet.Sprite].Width * scale) / 2,
                        (int)planet.Y - (int)(Planets[planet.Sprite].Height * scale) / 2, (int)(Planets[planet.Sprite].Width * scale),
                        (int)(Planets[planet.Sprite].Height * scale));
                    if (_Bound.Contains(position) && !Globals.windowOpen && !Globals.HoveringMob)
                    {
                        Globals.HoveringPlanet = true;
                        UserInterface.Active.SetCursor(Cursors[1]);
                        if (new KeyControl().Click())
                        {
                            GameLogic.selectedPlanet = planet.Id;
                            GameLogic.Selected = "";
                            GameLogic.SelectedType = "";
                            if (DateTime.UtcNow - TimeSpan.FromMilliseconds(500) < doubleClick)
                            {
                                Actions.Trade();
                            }
                            doubleClick = DateTime.UtcNow;
                        }
                    }
                    if (GameLogic.selectedPlanet != planet.Id) continue;
                    DrawBorder(_Bound, 1, Color.DarkGray * .25F);
                    DrawString(Globals.Font10, planet.Name, planet.X, planet.Y - (int)(Planets[planet.Sprite].Height * scale) / 2.0F - 20, true, Color.AntiqueWhite);
                }
            }
        }

        public static void DrawNebulae()
        {
            Globals.Nebula = "";
            foreach (var Nebula in GameLogic.Nebulae)
            {
                Color color;
                switch (Nebula.Type)
                {
                    case 0:
                        color = Color.MediumPurple;
                        break;
                    case 1:
                        color = Color.Crimson;
                        break;
                    case 2:
                        color = Color.DarkGoldenrod;
                        break;
                    case 3:
                        color = Color.DarkOrchid;
                        break;
                    case 4:
                        color = Color.MidnightBlue;
                        break;
                    default:
                        color = Color.Transparent;
                        break;
                }
                Game1.spriteBatch.Draw(nebula, new Vector2(Nebula.X, Nebula.Y), color);
                var nebRect = new Rectangle(Nebula.X, Nebula.Y, nebula.Width, nebula.Height);
                if (nebRect.Contains(Types.Player[GameLogic.PlayerIndex].X, Types.Player[GameLogic.PlayerIndex].Y))
                {
                    Globals.Nebula = Nebula.ID;
                }
            }
        }

        private static void DrawPlayers()
        {
            if (GameLogic.PlayerIndex <= -1) return;

            var SpriteNum = 1;

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
                    DrawString(Globals.Font10, Types.Player[i].Name, Types.Player[i].X, Types.Player[i].Y - Characters[SpriteNum].Height / 2F - 10, true, Color.AntiqueWhite);
                    // Health
                    var HealthColor1 = Color.Green;
                    var HealthColor2 = Color.DarkGreen;
                    var percentHealth = (Types.Player[i].MaxHealth - Types.Player[i].Health) / (float)Types.Player[i].MaxHealth;
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
                    var healthRect = new RectangleF(Types.Player[i].X - Characters[SpriteNum].Width / 2.0F, Types.Player[i].Y + Characters[SpriteNum].Height / 2.0F, Characters[SpriteNum].Width, 6);
                    Game1.spriteBatch.DrawLine(healthRect.Left + 2, healthRect.Bottom - 2, healthRect.Right - Characters[SpriteNum].Width * percentHealth, healthRect.Bottom - 2, HealthColor1, 4F);
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
                    DrawString(Globals.Font10, HealthDisplay, Types.Player[i].X, healthRect.Bottom + 8, true, HealthColor2);
                    // Shield
                    var shieldRect = new RectangleF(Types.Player[i].X - Characters[SpriteNum].Width / 2.0F,
                        Types.Player[i].Y + Characters[SpriteNum].Height / 2.0F + 7, Characters[SpriteNum].Width, 6);
                    if (Types.Player[i].Shield > 0 && Types.Player[i].MaxShield > 0)
                    {
                        var percentShield = (Types.Player[i].MaxShield - Types.Player[i].Shield) / (float)Types.Player[i].MaxShield;
                        Game1.spriteBatch.DrawLine(shieldRect.Left + 2, shieldRect.Bottom - 2,
                            shieldRect.Right - Characters[SpriteNum].Width * percentShield, shieldRect.Bottom - 2,
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
                        DrawString(Globals.Font8, ShieldDisplay, Types.Player[i].X, shieldRect.Bottom + 13, true, Color.Goldenrod);
                    }
                    else if (Types.Player[i].Shield <= 0 && Types.Player[i].MaxShield > 0)
                    {
                        DrawString(Globals.Font8, "Shield depleted", Types.Player[i].X, shieldRect.Bottom + 13, true, Color.Goldenrod);
                    }
                }

                // OnClick
                var Bound = new Rectangle((int)Types.Player[i].X - Characters[SpriteNum].Width / 2, (int)Types.Player[i].Y - Characters[SpriteNum].Height / 2, Characters[SpriteNum].Width, Characters[SpriteNum].Height);
                var ms = Mouse.GetState();
                var position = Game1.Camera.ScreenToWorld(new Vector2(ms.X, ms.Y));
                if (Globals.cursorOverride || Globals.Flying) position = Vector2.Zero;
                if (Bound.Contains(position) && !Globals.windowOpen)
                {
                    Globals.HoveringMob = true;
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
                    DrawString(Globals.Font8, "(" + mob.MobType.Name + ")", mob.X, mob.Y - height - 12, true, Color.AntiqueWhite);
                    DrawString(Globals.Font10, mob.Name, mob.X, mob.Y - height - 24, true, Color.AntiqueWhite);

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
                    var healthRect = new RectangleF(mob.X - Characters[spriteNum].Width / 2.0F, mob.Y + Characters[spriteNum].Height / 2.0F, Characters[spriteNum].Width, 6);
                    Game1.spriteBatch.DrawLine(healthRect.Left + 2, healthRect.Bottom - 2, healthRect.Right - Characters[spriteNum].Width * percentHealth, healthRect.Bottom - 2, healthColor1, 4F);
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
                    DrawString(Globals.Font10, healthDisplay, mob.X, healthRect.Bottom, true, healthColor2);

                    // Shield
                    var shieldRect = new RectangleF(mob.X - Characters[spriteNum].Width / 2.0F, mob.Y + Characters[spriteNum].Height / 2.0F + 7, Characters[spriteNum].Width, 6);
                    if (mob.Shield > 0 && mob.MobType.MaxShield > 0)
                    {
                        var percentShield = (mob.MobType.MaxShield - mob.Shield) / mob.MobType.MaxShield;
                        Game1.spriteBatch.DrawLine(shieldRect.Left + 2, shieldRect.Bottom - 2,
                            shieldRect.Right - Characters[spriteNum].Width * percentShield, shieldRect.Bottom - 2,
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
                        DrawString(Globals.Font8, shieldDisplay, mob.X, shieldRect.Bottom + 13, true, Color.Goldenrod);
                    }
                    else if (mob.Shield <= 0 && mob.MobType.MaxShield > 0)
                    {
                        DrawString(Globals.Font8, "Shield depleted", mob.X, shieldRect.Bottom + 13, true, Color.Goldenrod);
                    }
                }
                // OnClick
                var bound = new Rectangle((int)mob.X - Characters[spriteNum].Width / 2, (int)mob.Y - Characters[spriteNum].Height / 2,
                    Characters[spriteNum].Width, Characters[spriteNum].Height);
                var ms = Mouse.GetState();
                var position = Game1.Camera.ScreenToWorld(new Vector2(ms.X, ms.Y));
                if (Globals.cursorOverride || Globals.Flying || Globals.HoveringGUI) position = Vector2.Zero;
                if (bound.Contains(position))
                {
                    Globals.HoveringMob = true;
                    UserInterface.Active.SetCursor(CursorType.Pointer);
                    if (ms.LeftButton == ButtonState.Pressed && GameLogic.Selected != mob.Id)
                    {
                        GameLogic.Selected = mob.Id;
                        GameLogic.SelectedType = "MOB";
                        GameLogic.selectedPlanet = "";
                    }
                    if (ms.RightButton == ButtonState.Pressed && GameLogic.Selected == mob.Id)
                    {
                        if (Types.Player[GameLogic.PlayerIndex].Weap1Charge >= 100) { Actions.Attack(1, mob.Id); }
                        if (Types.Player[GameLogic.PlayerIndex].Weap2Charge >= 100) { Actions.Attack(2, mob.Id); }
                        if (Types.Player[GameLogic.PlayerIndex].Weap3Charge >= 100) { Actions.Attack(3, mob.Id); }
                        if (Types.Player[GameLogic.PlayerIndex].Weap4Charge >= 100) { Actions.Attack(4, mob.Id); }
                        if (Types.Player[GameLogic.PlayerIndex].Weap5Charge >= 100) { Actions.Attack(5, mob.Id); }
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
            for (var i = 1; i < Characters.Length; i++)
            {
                Characters[i] = manager.Load<Texture2D>("Characters/" + i);
            }
        }

        private static void LoadPlanets(ContentManager manager)
        {
            for (var i = 0; i < Planets.Length; i++)
            {
                Planets[i] = manager.Load<Texture2D>("Planets/" + i);
            }
        }

        private static void LoadCursors(ContentManager manager)
        {
            for (var i = 1; i < Cursors.Length; i++)
            {
                Cursors[i] = manager.Load<Texture2D>("Cursors/" + i);
            }
        }

        private static void LoadObjects(ContentManager manager)
        {
            for (var i = 0; i < Objects.Length; i++)
            {
                Objects[i] = manager.Load<Texture2D>("Objects/" + i);
            }
        }

        public static void DrawInfo(ContentManager manager)
        {
            if (GameLogic.PlayerIndex <= -1) return;
            var offsetX = 0;
            Game1.spriteBatch.Begin();
            if (Globals.scanner) { offsetX = 200; }

            // Draw details panel
            if (Globals.details)
            {
                var ms = Mouse.GetState();
                var position = new Vector2(ms.X, ms.Y);
                if (Globals.cursorOverride) position = Vector2.Zero;
                Game1.spriteBatch.Draw(details, new Vector2(0 + offsetX, Globals.PreferredBackBufferHeight - 200), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                // Planetary info
                if (GameLogic.selectedPlanet != "")
                {
                    var STAR = GameLogic.Galaxy.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);
                    var PLANET = GameLogic.Planets.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);

                    if (STAR != null)
                    {
                        var scale = (float)150 / Planets[4].Width;
                        Game1.spriteBatch.Draw(Planets[4], new Vector2(20 + offsetX, Globals.PreferredBackBufferHeight - 178), null, Color.White * .5F, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        DrawString(Globals.Font12, "Name: " + STAR.Name, 150 + offsetX, Globals.PreferredBackBufferHeight - 175, false, Color.DarkGray);
                        DrawString(Globals.Font10, "Classification: " + STAR.Class, 160 + offsetX, Globals.PreferredBackBufferHeight - 155, false, Color.DarkGray);
                        DrawString(Globals.Font10, " Coordinates: " + STAR.X / 100 + ":" + STAR.Y / 100, 160 + offsetX, Globals.PreferredBackBufferHeight - 140, false, Color.DarkGray);
                        DrawString(Globals.Font10, "  Belligerence: " + STAR.Belligerence, 160 + offsetX, Globals.PreferredBackBufferHeight - 125, false, Color.DarkGray);
                        DrawString(Globals.Font10, "  Planets: " + STAR.Planets.Count, 160 + offsetX, Globals.PreferredBackBufferHeight - 110, false, Color.DarkGray);
                    }

                    if (PLANET != null)
                    {
                        var scale = (float)150 / Planets[PLANET.Sprite].Width;
                        Game1.spriteBatch.Draw(Planets[PLANET.Sprite], new Vector2(20 + offsetX, Globals.PreferredBackBufferHeight - 178), null, Color.White * .5F, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        DrawString(Globals.Font12, "Name: " + PLANET.Name, 150 + offsetX, Globals.PreferredBackBufferHeight - 175, false, Color.DarkGray);
                        DrawString(Globals.Font10, "Classification: " + PLANET.Class, 160 + offsetX, Globals.PreferredBackBufferHeight - 155, false, Color.DarkGray);
                        DrawString(Globals.Font10, " Coordinates: " + Math.Round(PLANET.X / 100, 2) + ":" + Math.Round(PLANET.Y / 100, 2), 160 + offsetX, Globals.PreferredBackBufferHeight - 140, false, Color.DarkGray);
                        DrawString(Globals.Font10, "  Belligerence: " + PLANET.Belligerence, 160 + offsetX, Globals.PreferredBackBufferHeight - 125, false, Color.DarkGray);
                        var tradeRect = new Rectangle(178 + offsetX, Globals.PreferredBackBufferHeight - 38, 44, 17);
                        var mineRect = new Rectangle(233 + offsetX, Globals.PreferredBackBufferHeight - 38, 35, 17);
                        var aerRect = new Rectangle(278 + offsetX, Globals.PreferredBackBufferHeight - 38, 69, 17);
                        DrawString(Globals.Font10, "Trade", 180 + offsetX, Globals.PreferredBackBufferHeight - 37, false, tradeRect.Contains(position) ? Color.White : Color.DarkGray);
                        DrawString(Globals.Font10, "Mine", 235 + offsetX, Globals.PreferredBackBufferHeight - 37, false, mineRect.Contains(position) ? Color.White : Color.DarkGray);
                        DrawString(Globals.Font10, "Aerology", 280 + offsetX, Globals.PreferredBackBufferHeight - 37, false, aerRect.Contains(position) ? Color.White : Color.DarkGray);
                        DrawBorder(tradeRect, 1, Color.DarkGray);
                        DrawBorder(mineRect, 1, Color.DarkGray);
                        DrawBorder(aerRect, 1, Color.DarkGray);
                        if (tradeRect.Contains(position))
                        {
                            UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                            if (KC.Click())
                            {
                                Actions.Trade();
                            }
                        }
                        if (mineRect.Contains(position))
                        {
                            UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                            if (KC.Click())
                            {
                                Actions.Mine();
                            }
                        }
                        if (aerRect.Contains(position))
                        {
                            UserInterface.Active.SetCursor(Cursors[2], 32, new Point(-4, 0));
                            if (KC.Click())
                            {
                                Actions.Aerology();
                            }
                        }
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
                        DrawString(Globals.Font10, MOB.Name ?? "Null", 200 + offsetX, Globals.PreferredBackBufferHeight - 175, true, Color.DarkGray);
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
                // Player info
                if (GameLogic.SelectedType == "PLAYER")
                {
                    var PLAYER = Types.Player.FirstOrDefault(p => p.Id == GameLogic.Selected);
                    if (PLAYER != null)
                    {
                        //if (PLAYER.Id != Types.Player[GameLogic.PlayerIndex].Id)
                        {
                            var min = GameLogic.CheckLevel(PLAYER.Level);
                            var max = GameLogic.CheckLevel(PLAYER.Level + 1);
                            var percent = (float)(PLAYER.Exp - min) / (float)(max - min);
                            DrawString(Globals.Font12, PLAYER.Rank + " " + PLAYER.Name, 200 + offsetX, Globals.PreferredBackBufferHeight - 175, true, Color.DarkGray);
                            DrawString(Globals.Font12, PLAYER.Rank + " " + PLAYER.Name, 201 + offsetX, Globals.PreferredBackBufferHeight - 175, true, Color.DarkGray);
                            DrawString(Globals.Font10, "Level: " + PLAYER.Level, 20 + offsetX, Globals.PreferredBackBufferHeight - 160, false, Color.DarkGray);
                            DrawString(Globals.Font10, "Credits: " + PLAYER.Credits, 20 + offsetX, Globals.PreferredBackBufferHeight - 145, false, Color.DarkGray);
                            Game1.spriteBatch.Draw(experienceBar, new Vector2(26 + offsetX, Globals.PreferredBackBufferHeight - 35), Color.White);
                            Game1.spriteBatch.Draw(pixel, new Vector2(27 + offsetX, Globals.PreferredBackBufferHeight - 34), null, Color.MidnightBlue * .65F, 0, Vector2.Zero, new Vector2(348 * percent, 8), SpriteEffects.None, 0);
                            DrawString(Globals.Font8, PLAYER.Exp + "/" + max, 201 + offsetX, Globals.PreferredBackBufferHeight - 36, true, Color.Black);
                            DrawString(Globals.Font8, PLAYER.Exp + "/" + max, 200 + offsetX, Globals.PreferredBackBufferHeight - 36, true, Color.White);
                        }
                    }
                }

                // Click to close
                var Bound = new Rectangle(386 + offsetX, Globals.PreferredBackBufferHeight - 196, 12, 12);
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
            var offset = new Vector2(100, Globals.PreferredBackBufferHeight - 100);
            Game1.spriteBatch.Begin();

            // Override click-through
            var ms = Mouse.GetState();
            var position = new Vector2(ms.X, ms.Y);
            var GUIBounds = new Rectangle(0, Globals.PreferredBackBufferHeight - 200, 0, 200);
            if (Globals.scanner) { GUIBounds.Width += 200; }
            if (Globals.details) { GUIBounds.Width += 400; }
            Globals.HoveringGUI = GUIBounds.Contains(position);
            if (GUIBounds.Contains(position))
            {
                UserInterface.Active.SetCursor(CursorType.Default);
            }

            // Draw scanner
            if (Globals.scanner)
            {
                if (Globals.cursorOverride) position = Vector2.Zero;
                var _player = Types.Player[GameLogic.PlayerIndex];
                Game1.spriteBatch.Draw(scanner, new Vector2(0, Globals.PreferredBackBufferHeight - 200), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                Game1.spriteBatch.Draw(triangle, offset, null, Color.White, Types.Player[GameLogic.PlayerIndex].Rotation, new Vector2(8, 8), 1, SpriteEffects.None, 0);
                var playerWTS = Game1.Camera.WorldToScreen(_player.X, _player.Y);
                // Look for mobs and display if within range
                if (GameLogic.LocalMobs != null && GameLogic.LocalMobs.Count > 0)
                {
                    foreach (var m in GameLogic.LocalMobs)
                    {
                        var mobWTS = Game1.Camera.WorldToScreen(m.X, m.Y);
                        var mobPosition = offset + new Vector2(mobWTS.X - playerWTS.X, mobWTS.Y - playerWTS.Y) * scale;
                        if (!(mobPosition.X > 17) || !(mobPosition.X < 185) ||
                            !(mobPosition.Y > Globals.PreferredBackBufferHeight - 183) ||
                            !(mobPosition.Y < Globals.PreferredBackBufferHeight - 16)) continue;

                        Game1.spriteBatch.Draw(diamond, mobPosition, null, new Color(88, 170, 76), m.Rotation, new Vector2(6, 6), 1, SpriteEffects.None, 0);
                        var bound = new Rectangle(mobPosition.ToPoint() - new Point(6), new Point(12));
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

                // Look for planets and stars and display if within range
                if (GameLogic.Galaxy != null)
                {
                    foreach (var obj in GameLogic.Galaxy.Where(p =>
                        p.X >= _player.X - 2000 && p.X <= _player.X + 2000 &&
                        p.Y >= _player.Y - 2000 && p.Y <= _player.Y + 2000))
                    {
                        var starWTS = Game1.Camera.WorldToScreen(obj.X, obj.Y);
                        var starPosition = offset + new Vector2(starWTS.X - playerWTS.X, starWTS.Y - playerWTS.Y) * scale;
                        if (starPosition.X > 17 && starPosition.X < 185 &&
                            starPosition.Y > Globals.PreferredBackBufferHeight - 183 &&
                            starPosition.Y < Globals.PreferredBackBufferHeight - 16)
                        {
                            Game1.spriteBatch.Draw(star, starPosition, null, Color.White, 0, new Vector2(6, 6), 1, SpriteEffects.None, 0);
                            var bound = new Rectangle(starPosition.ToPoint() - new Point(6), new Point(12));
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
                            var satWTS = Game1.Camera.WorldToScreen(sat.X, sat.Y);
                            var satPosition = offset + new Vector2(satWTS.X - playerWTS.X, satWTS.Y - playerWTS.Y) * scale;
                            if (!(satPosition.X > 17) || !(satPosition.X < 185) ||
                                !(satPosition.Y > Globals.PreferredBackBufferHeight - 183) ||
                                !(satPosition.Y < Globals.PreferredBackBufferHeight - 16)) continue;
                            Game1.spriteBatch.Draw(circle, satPosition, null, Color.White, 0, new Vector2(6, 6), 1, SpriteEffects.None, 0);
                            var bound = new Rectangle(satPosition.ToPoint() - new Point(6), new Point(12));
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

                // Click to close
                var Bound = new Rectangle(186, Globals.PreferredBackBufferHeight - 196, 12, 12);
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
                DrawString(Globals.Font8, "Distance remaining: " + (int)GameLogic.distance / 100 + "AU", -1, 50, true, Color.DimGray);
            }
            else
            {
                DrawString(Globals.Font8, GameLogic.PlayerIndex.ToString(), -1, 30, true, Color.Green);
            }
            Game1.spriteBatch.End();
        }

        public static void DrawWeaponsBar(ContentManager manager)
        {
            if (GameLogic.PlayerIndex <= -1 || GameLogic.Items == null) return;
            if (Types.Player[GameLogic.PlayerIndex].Inventory == null) return;
            var P = Types.Player[GameLogic.PlayerIndex];
            var OL1 = P.Weap1Charge < 100 ? Color.DarkRed : Color.White;
            var OL2 = P.Weap2Charge < 100 ? Color.DarkRed : Color.White;
            var OL3 = P.Weap3Charge < 100 ? Color.DarkRed : Color.White;
            var OL4 = P.Weap4Charge < 100 ? Color.DarkRed : Color.White;
            var OL5 = P.Weap5Charge < 100 ? Color.DarkRed : Color.White;
            var stat1 = P.Weap1Charge < 100 ? P.Weap1Charge + "%" : "";
            var stat2 = P.Weap2Charge < 100 ? P.Weap2Charge + "%" : "";
            var stat3 = P.Weap3Charge < 100 ? P.Weap3Charge + "%" : "";
            var stat4 = P.Weap4Charge < 100 ? P.Weap4Charge + "%" : "";
            var stat5 = P.Weap5Charge < 100 ? P.Weap5Charge + "%" : "";
            var weap1 = GameLogic.Items.FirstOrDefault(i => i.Id == Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(w => w.Slot == 7)?.ItemId);
            var weap2 = GameLogic.Items.FirstOrDefault(i => i.Id == Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(w => w.Slot == 8)?.ItemId);
            var weap3 = GameLogic.Items.FirstOrDefault(i => i.Id == Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(w => w.Slot == 9)?.ItemId);
            var weap4 = GameLogic.Items.FirstOrDefault(i => i.Id == Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(w => w.Slot == 10)?.ItemId);
            var weap5 = GameLogic.Items.FirstOrDefault(i => i.Id == Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(w => w.Slot == 11)?.ItemId);
            Game1.spriteBatch.Begin();
            if (Globals.weaponsBar)
            {
                if (weap1 != null)
                {
                    Game1.spriteBatch.Draw(Objects[weap1.Image], new Vector2(Globals.PreferredBackBufferWidth - 40, 14), null, COLOR(weap1.Color), 0, Vector2.Zero, .5F, SpriteEffects.None, 0);
                    if (P.Weap1Charge < 100)
                        Game1.spriteBatch.Draw(pixel, new Vector2(Globals.PreferredBackBufferWidth - 40, 14), null, Color.Black * .5F, 0, Vector2.Zero, new Vector2(32, 32), SpriteEffects.None, 0);
                    DrawString(Globals.Font10, stat1, Globals.PreferredBackBufferWidth - 24, 25, true, Color.White);
                    DrawString(Globals.Font10, stat1, Globals.PreferredBackBufferWidth - 23, 25, true, Color.White);
                }
                if (weap2 != null)
                {
                    Game1.spriteBatch.Draw(Objects[weap2.Image], new Vector2(Globals.PreferredBackBufferWidth - 40, 49), null, COLOR(weap2.Color), 0, Vector2.Zero, .5F, SpriteEffects.None, 0);
                    if (P.Weap2Charge < 100)
                        Game1.spriteBatch.Draw(pixel, new Vector2(Globals.PreferredBackBufferWidth - 40, 49), null, Color.Black * .5F, 0, Vector2.Zero, new Vector2(32, 32), SpriteEffects.None, 0);
                    DrawString(Globals.Font10, stat2, Globals.PreferredBackBufferWidth - 24, 60, true, Color.White);
                    DrawString(Globals.Font10, stat2, Globals.PreferredBackBufferWidth - 23, 60, true, Color.White);
                }
                if (weap3 != null)
                {
                    Game1.spriteBatch.Draw(Objects[weap3.Image], new Vector2(Globals.PreferredBackBufferWidth - 40, 84), null, COLOR(weap3.Color), 0, Vector2.Zero, .5F, SpriteEffects.None, 0);
                    if (P.Weap3Charge < 100)
                        Game1.spriteBatch.Draw(pixel, new Vector2(Globals.PreferredBackBufferWidth - 40, 84), null, Color.Black * .5F, 0, Vector2.Zero, new Vector2(32, 32), SpriteEffects.None, 0);
                    DrawString(Globals.Font10, stat3, Globals.PreferredBackBufferWidth - 24, 95, true, Color.White);
                    DrawString(Globals.Font10, stat3, Globals.PreferredBackBufferWidth - 23, 95, true, Color.White);
                }
                if (weap4 != null)
                {
                    Game1.spriteBatch.Draw(Objects[weap4.Image], new Vector2(Globals.PreferredBackBufferWidth - 40, 119), null, COLOR(weap4.Color), 0, Vector2.Zero, .5F, SpriteEffects.None, 0);
                    if (P.Weap4Charge < 100)
                        Game1.spriteBatch.Draw(pixel, new Vector2(Globals.PreferredBackBufferWidth - 40, 119), null, Color.Black * .5F, 0, Vector2.Zero, new Vector2(32, 32), SpriteEffects.None, 0);
                    DrawString(Globals.Font10, stat4, Globals.PreferredBackBufferWidth - 24, 130, true, Color.White);
                    DrawString(Globals.Font10, stat4, Globals.PreferredBackBufferWidth - 23, 130, true, Color.White);
                }
                if (weap5 != null)
                {
                    Game1.spriteBatch.Draw(Objects[weap5.Image], new Vector2(Globals.PreferredBackBufferWidth - 40, 154), null, COLOR(weap5.Color), 0, Vector2.Zero, .5F, SpriteEffects.None, 0);
                    if (P.Weap5Charge < 100)
                        Game1.spriteBatch.Draw(pixel, new Vector2(Globals.PreferredBackBufferWidth - 40, 154), null, Color.Black * .5F, 0, Vector2.Zero, new Vector2(32, 32), SpriteEffects.None, 0);
                    DrawString(Globals.Font10, stat5, Globals.PreferredBackBufferWidth - 24, 165, true, Color.White);
                    DrawString(Globals.Font10, stat5, Globals.PreferredBackBufferWidth - 23, 165, true, Color.White);
                }
            }
            else
            {
                if (weap1 != null)
                {
                    //stat1 = Globals.status1 < 100 ? Globals.status1 + "%" : "1";
                    DrawBorder(new Rectangle(Globals.PreferredBackBufferWidth - 40, 14, 32, 32), 1, OL1);
                    DrawString(Globals.Font10, stat1, Globals.PreferredBackBufferWidth - 24, 25, true, Color.DarkGray);
                    DrawString(Globals.Font10, stat1, Globals.PreferredBackBufferWidth - 23, 25, true, Color.DarkGray);
                }
                if (weap2 != null)
                {
                    //stat2 = Globals.status1 < 100 ? Globals.status1 + "%" : "2";
                    DrawBorder(new Rectangle(Globals.PreferredBackBufferWidth - 40, 49, 32, 32), 1, OL2);
                    DrawString(Globals.Font10, stat2, Globals.PreferredBackBufferWidth - 24, 60, true, Color.DarkGray);
                    DrawString(Globals.Font10, stat2, Globals.PreferredBackBufferWidth - 23, 60, true, Color.DarkGray);
                }
                if (weap3 != null)
                {
                    //stat3 = Globals.status1 < 100 ? Globals.status1 + "%" : "3";
                    DrawBorder(new Rectangle(Globals.PreferredBackBufferWidth - 40, 84, 32, 32), 1, OL3);
                    DrawString(Globals.Font10, stat3, Globals.PreferredBackBufferWidth - 24, 95, true, Color.DarkGray);
                    DrawString(Globals.Font10, stat3, Globals.PreferredBackBufferWidth - 23, 95, true, Color.DarkGray);
                }
                if (weap4 != null)
                {
                    //stat4 = Globals.status1 < 100 ? Globals.status1 + "%" : "4";
                    DrawBorder(new Rectangle(Globals.PreferredBackBufferWidth - 40, 119, 32, 32), 1, OL4);
                    DrawString(Globals.Font10, stat4, Globals.PreferredBackBufferWidth - 24, 130, true, Color.DarkGray);
                    DrawString(Globals.Font10, stat4, Globals.PreferredBackBufferWidth - 23, 130, true, Color.DarkGray);
                }
                if (weap5 != null)
                {
                    //stat5 = Globals.status1 < 100 ? Globals.status1 + "%" : "5";
                    DrawBorder(new Rectangle(Globals.PreferredBackBufferWidth - 40, 154, 32, 32), 1, OL5);
                    DrawString(Globals.Font10, stat5, Globals.PreferredBackBufferWidth - 24, 165, true, Color.DarkGray);
                    DrawString(Globals.Font10, stat5, Globals.PreferredBackBufferWidth - 23, 165, true, Color.DarkGray);
                }
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
            Game1.spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder,
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
            var size = font.MeasureString(text);
            var pos = new Vector2(X, Y);
            if (align)
            {
                pos.X -= size.X / 2;
            }
            Game1.spriteBatch.DrawString(font, text, pos, color);
        }

        public static Color COLOR(string RGB)
        {
            if (RGB.Length < 9) return Color.White;
            int.TryParse(RGB.Substring(0, 3), out var r);
            int.TryParse(RGB.Substring(3, 3), out var g);
            int.TryParse(RGB.Substring(6, 3), out var b);
            return new Color(r, g, b);
        }
    }
}
