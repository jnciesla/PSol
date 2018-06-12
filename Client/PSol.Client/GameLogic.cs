using System;
using Bindings;
using Microsoft.Xna.Framework;

namespace Client
{
    internal class GameLogic
    {
        public static int PlayerIndex = -1;
        private static ClientTCP ctcp = new ClientTCP();
        private static int messageTime;

        public static bool IsMoving()
        {
            if (Globals.DirUp || Globals.DirDn || Globals.DirLt || Globals.DirRt)
            {
                ctcp.XFerPlayer();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Rotate(int dir)
        {
            switch (dir)
            {
                case 0:
                    Types.Player[PlayerIndex].Rotation -= MathHelper.ToRadians(4f);
                    break;
                case 1:
                    Types.Player[PlayerIndex].Rotation += MathHelper.ToRadians(4f);
                    break;
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

    }
}
