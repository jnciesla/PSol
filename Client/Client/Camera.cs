using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client
{
    class Camera
    {
        public float zoom = 1;
        public Matrix transform;
        Viewport view;
        Vector2 center;

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
            center = new Vector2(Types.Player[0].X, Types.Player[0].Y) - new Vector2(512/zoom, 384/zoom);
            transform = Matrix.CreateScale(new Vector3(zoom, zoom, 0)) * Matrix.CreateTranslation(new Vector3(-center.X * zoom, -center.Y * zoom, 0));
        }
    }
}
