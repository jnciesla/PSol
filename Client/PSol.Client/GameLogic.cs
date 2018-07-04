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
        public static Vector2 Destination;
        public static bool Navigating;
        private static readonly ClientTCP ctcp = new ClientTCP();
        private static int messageTime;
        public static List<Star> Galaxy;
        public static List<Mob> LocalMobs;
        public static List<Combat> LocalCombat;

        public static bool IsMoving()
        {
            if (Globals.DirUp || Globals.DirDn || Globals.DirLt || Globals.DirRt)
            {
                ctcp.XFerPlayer();
                return true;
            }
            return false;
        }

        public static void Rotate(int dir)
        {
            switch (dir)
            {
                case 0:
                    Types.Player[PlayerIndex].Rotation -= MathHelper.ToRadians(4f);
                    if (Types.Player[PlayerIndex].Rotation <= 0)
                    {
                        Types.Player[PlayerIndex].Rotation += (float)Math.PI * 2;
                    }
                    break;
                case 1:
                    Types.Player[PlayerIndex].Rotation += MathHelper.ToRadians(4f);
                    if (Types.Player[PlayerIndex].Rotation >= (float)Math.PI * 2)
                    {
                        Types.Player[PlayerIndex].Rotation -= (float)Math.PI * 2;
                    }
                    break;
            }
        }

        public static void Navigate()
        {
            if (!Navigating) { return; }
            Vector2 start = new Vector2(Types.Player[PlayerIndex].X, Types.Player[PlayerIndex].Y);
            Vector2 direction = Vector2.Normalize(start - Destination);
            distance = Vector2.Distance(start, Destination);
            float angle = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.ToRadians(90);
            if (angle <= 0) { angle += (float)Math.PI * 2; } else if (angle >= (float)Math.PI * 2) { angle -= (float)Math.PI * 2; }
            if (Types.Player[PlayerIndex].Rotation != angle && Types.Player[PlayerIndex].Rotation <= angle)
                Rotate(1);
            if (Types.Player[PlayerIndex].Rotation != angle && Types.Player[PlayerIndex].Rotation >= angle)
                Rotate(0);
            Globals.DirUp = true;
            Console.WriteLine(distance);
            if (distance <= 50)
            {
                Navigating = false;
                InterfaceGUI.AddChats(@"We've reached our destination", Color.BurlyWood);
            }
        }

        public static void CheckMovement()
        {
            if (IsMoving())
            {
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

        }

        private static void EdgeWarning()
        {
            if (messageTime + 1000 < Game1.Tick)
            {
                messageTime = Game1.Tick;
                InterfaceGUI.AddChats("We shouldn't go beyond the edge of the mapped galaxy.", Color.DarkGoldenrod);
            }
        }

        public static void WatchCombat()
        {
            foreach (var combat in LocalCombat)
            {
                var source = new Vector2(0, 0);
                var target = new Vector2(0, 0);
                var sourcePlayer = Types.Player.ToList().Find(p => p?.Id == combat.SourceId);
                var sourceMob = LocalMobs.Find(m => m.Id == combat.SourceId);
                var targetPlayer = Types.Player.ToList().Find(p => p?.Id == combat.TargetId);
                var targetMob = LocalMobs.Find(m => m.Id == combat.TargetId);
                source = sourcePlayer != null ? new Vector2(sourcePlayer.X, sourcePlayer.Y) : source;
                source = sourceMob != null ? new Vector2(sourceMob.X, sourceMob.Y) : source;
                target = targetPlayer != null ? new Vector2(targetPlayer.X, targetPlayer.Y) : target;
                target = targetMob != null ? new Vector2(targetMob.X, targetMob.Y) : target;
                if (source.X.CompareTo(0) > 0 && source.Y.CompareTo(0) > 0 && target.X.CompareTo(0) > 0 && target.Y.CompareTo(0) > 0)
                {
                    Graphics.DrawLaser(source, target);
                }
            }
        }

    }
}
