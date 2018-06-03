using System;
using Bindings;
using Microsoft.Xna.Framework;

namespace Client
{
	class GameLogic
	{
		Camera camera;
		public static int PlayerIndex = -1;
		private static ClientTCP ctcp = new ClientTCP();

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
			if (dir == 0)
			{
				Types.Player[PlayerIndex].Rotation -= MathHelper.ToRadians(4f);
			}
			else if (dir == 1)
			{
				Types.Player[PlayerIndex].Rotation += MathHelper.ToRadians(4f);
			}
		}

		public static void CheckMovement()
		{
			if (IsMoving())
			{
				if (Globals.DirLt) { Rotate(0); }
				if (Globals.DirRt) { Rotate(1); }
				if (Globals.DirUp)
				{
					var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation),
						-(float)Math.Sin(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation));
					Types.Player[PlayerIndex].X += direction.X * 4f;
					Types.Player[PlayerIndex].Y += direction.Y * 4f;
				}
				if (Globals.DirDn)
				{
					var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation),
						-(float)Math.Sin(MathHelper.ToRadians(90) - Types.Player[PlayerIndex].Rotation));
					Types.Player[PlayerIndex].X -= direction.X * 1f;
					Types.Player[PlayerIndex].Y -= direction.Y * 1f;
				}
			}

		}

	}
}
