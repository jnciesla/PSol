using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    interface ILaser
    {
        bool IsComplete { get; }

        void Update();
        void Draw(SpriteBatch spriteBatch);
    }
}
