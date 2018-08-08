using Bindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    internal class Camera
    {
        public static Matrix transform;
        public static Viewport view;
        public static Vector2 center;

        public Camera(Viewport newView)
        {
            view = newView;
        }

        public void Update(GameTime gametime, Game1 ship)
        {
            float x = -5000, y = -5000;
            if (GameLogic.PlayerIndex > -1)
            {
                x = Types.Player[GameLogic.PlayerIndex].X;
                y = Types.Player[GameLogic.PlayerIndex].Y;

            }
            center = new Vector2(x, y) - new Vector2(Globals.PreferredBackBufferWidth / 2.0f, Globals.PreferredBackBufferHeight / 2.0f);
            transform = Matrix.CreateScale(new Vector3(1,1,1)) *
                        Matrix.CreateTranslation(new Vector3(-center, 1));
        }
    }
}
