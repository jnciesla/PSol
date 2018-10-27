using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;
using Microsoft.Xna.Framework;
using PSol.Data.Models;

namespace PSol.Client
{
    internal class GameLogic
    {
        public static int PlayerIndex = -1;
        public static string Selected = "";
        public static string SelectedType = "";
        public static string selectedPlanet = "";
        public static int selectedMapItem = -1;
        public static float distance;
        public static float navAngle = -1;
        public static Vector2 Destination;
        public static bool Navigating;
        private static readonly ClientTCP ctcp = new ClientTCP();
        private static int messageTime;
        private static int navTimer;
        public static List<Star> Galaxy;
        public static List<Nebula> Nebulae = new List<Nebula>();
        public static List<Planet> Planets = new List<Planet>();
        public static List<Item> Items;
        public static List<Mob> LocalMobs;
        public static List<Combat> LocalCombat;
        public static List<Inventory> LocalLoot;
        public static List<Loot> RealLoot;

        private static Random random;

        public static bool IsMoving()
        {
            if (!Globals.DirUp && !Globals.DirDn && !Globals.DirLt && !Globals.DirRt) return false;
            ctcp.XFerPlayer();
            return true;
        }

        public static void Rotate(int dir, int amount = 4)
        {
            switch (dir)
            {
                case 0:
                    Types.Player[PlayerIndex].Rotation -= MathHelper.ToRadians(amount);
                    if (Types.Player[PlayerIndex].Rotation <= 0)
                    {
                        Types.Player[PlayerIndex].Rotation += (float)Math.PI * 2;
                    }
                    break;
                case 1:
                    Types.Player[PlayerIndex].Rotation += MathHelper.ToRadians(amount);
                    if (Types.Player[PlayerIndex].Rotation >= (float)Math.PI * 2)
                    {
                        Types.Player[PlayerIndex].Rotation -= (float)Math.PI * 2;
                    }
                    break;
            }
        }

        public static double GetAngleFromPlayer(Point point)
        {
            var mousePosition = new Vector2(point.X, point.Y);
            var playerPosition = new Vector2(Globals.PreferredBackBufferWidth / 2F, Globals.PreferredBackBufferHeight / 2F);
            var dPos = playerPosition - mousePosition;
            var angle = (float)Math.Atan2(dPos.Y, dPos.X) * (180 / Math.PI) - 90;
            var rad = MathHelper.ToRadians((float)angle);
            return rad;
        }

        public static void Navigate(bool initial = false)
        {
            if (!Navigating)
            {
                return;
            }
            var start = new Vector2(Types.Player[PlayerIndex].X, Types.Player[PlayerIndex].Y);
            var direction = Vector2.Normalize(start - Destination);
            distance = Vector2.Distance(start, Destination);
            if (navAngle == -1 && distance < 750)
            {
                Navigating = false;
                InterfaceGUI.AddChats(@"Too close to engage autonavigation", Color.BurlyWood);
            }
            if (navTimer + 1000 <= Game1.Tick)
            {
                navTimer = Game1.Tick;
                navAngle = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.ToRadians(90);
                if (navAngle < 0)
                {
                    navAngle += (float)Math.PI * 2;
                }
                else if (navAngle > (float)Math.PI * 2)
                {
                    navAngle -= (float)Math.PI * 2;
                }
            }
            Globals.DirUp = true;
            Console.WriteLine(Math.Abs(Types.Player[PlayerIndex].Rotation - navAngle));
            if (Math.Abs(Types.Player[PlayerIndex].Rotation - navAngle) <= .05F)
            {
                Types.Player[PlayerIndex].Rotation = navAngle;
            }
            else
            {
                Rotate(Math.Abs(Types.Player[PlayerIndex].Rotation - navAngle) >= 3.14 ? 1 : 0);
            }

            if (distance >= 150) return;
            Navigating = false;
            InterfaceGUI.AddChats(@"We've reached our destination", Color.BurlyWood);
        }

        public static void CheckMovement()
        {
            if (!IsMoving()) return;
            var newPosX = Types.Player[PlayerIndex].X;
            var newPosY = Types.Player[PlayerIndex].Y;
            if (Globals.DirLt) { Rotate(0); }
            if (Globals.DirRt) { Rotate(1); }
            if (Globals.DirUp)
            {
                var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation),
                    -(float)Math.Sin(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation));
                newPosX += direction.X * 4f;
                newPosY += direction.Y * 4f;
                if (newPosX > Globals.playArea.Left && newPosX < Globals.playArea.Right)
                {
                    Types.Player[PlayerIndex].X = newPosX;
                }
                else
                {
                    EdgeWarning();
                }

                if (newPosY > Globals.playArea.Top && newPosY < Globals.playArea.Bottom)
                {
                    Types.Player[PlayerIndex].Y = newPosY;
                }
                else
                {
                    EdgeWarning();
                }
            }
            if (Globals.DirDn)
            {
                var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation),
                    -(float)Math.Sin(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation));
                newPosX -= direction.X * 2f;
                newPosY -= direction.Y * 2f;
                if (newPosX > Globals.playArea.Left && newPosX < Globals.playArea.Right)
                {
                    Types.Player[PlayerIndex].X = newPosX;
                }
                else
                {
                    EdgeWarning();
                }

                if (newPosY > Globals.playArea.Top && newPosY < Globals.playArea.Bottom)
                {
                    Types.Player[PlayerIndex].Y = newPosY;
                }
                else
                {
                    EdgeWarning();
                }
            }

        }

        private static void EdgeWarning()
        {
            if (messageTime + 1000 >= Game1.Tick) return;
            messageTime = Game1.Tick;
            InterfaceGUI.AddChats("We shouldn't go beyond the edge of the mapped galaxy.", Color.DarkGoldenrod);
        }

        public static void CollectPlanets()
        {
            foreach (var s in Galaxy)
            {
                Planets.AddRange(s.Planets.ToList());
            }
        }

        public static void WatchCombat()
        {
            random = new Random();
            foreach (var combat in LocalCombat)
            {
                var source = new Vector2(combat.SourceX, combat.SourceY);
                var target = new Vector2(combat.TargetX, combat.TargetY);
                if (source.X.CompareTo(0) <= 0 || source.Y.CompareTo(0) <= 0 || target.X.CompareTo(0) <= 0 || target.Y.CompareTo(0) <= 0) continue;
                Graphics.DrawLaser(source, target);
                if (combat.TargetId == "dead")
                {
                    var tempLarge = new SmallExplosion(Graphics.largeExplosionTextures, Vector2.Zero);
                    Game1.Explosion.Add(tempLarge);
                    tempLarge.CreateFinal(new Vector2(target.X - 20 + random.Next(40), target.Y - 20 + random.Next(40)));
                }
                var tempDamage = new DamageText(combat.WeaponDamage.ToString(), new Vector2(target.X - 20 + random.Next(40), target.Y - 20 + random.Next(40)), 0);
                Game1.DamageTexts.Add(tempDamage);
                var tempSmall = new SmallExplosion(Graphics.smallExplosionTextures, Vector2.Zero);
                Game1.Explosion.Add(tempSmall);
                tempSmall.Create(new Vector2(target.X - 20 + random.Next(40), target.Y - 20 + random.Next(40)));
            }
        }
    }
}