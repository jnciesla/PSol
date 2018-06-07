using Bindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client
{
    internal class Camera
	{
		public float zoom = 1;
		public Matrix transform;
	    private Viewport view;
	    private Vector2 center;

		public Camera(Viewport newView)
		{
			view = newView;
		}

		public void ZoomController()
		{
			if (Globals.ZoomIn)
			{
				if (zoom <= 1.5)
				{
					zoom += (float)0.025;
				}
			}
			if (Globals.ZoomOut)
			{
				if (zoom >= 0.5)
				{
					zoom -= (float)0.025;
				}
			}
			if (Globals.ZoomDefault)
			{
				zoom = 1;
			}
		}

		public void Update(GameTime gametime, Game1 ship)
		{
			if (GameLogic.PlayerIndex <= -1) return;
			center = new Vector2(Types.Player[GameLogic.PlayerIndex].X, Types.Player[GameLogic.PlayerIndex].Y) -
					 new Vector2(512 / zoom, 384 / zoom);
			transform = Matrix.CreateScale(new Vector3(zoom, zoom, 0)) *
						Matrix.CreateTranslation(new Vector3(-center.X * zoom, -center.Y * zoom, 0));
		}
	}
}
