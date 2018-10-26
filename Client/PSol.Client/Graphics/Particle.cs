using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    public class Particle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color;
        public float Size { get; set; }
        public int TTL { get; set; }
        public float Opacity { get; set; }
        public bool SteadyColor { get; set; }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, float size, int ttl, Color color, bool steadyColor = false)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Size = size;
            TTL = ttl;
            Opacity = 1f;
            Color = color;
            SteadyColor = steadyColor;
        }

        public void Update()
        {
            Position += Velocity;
            Angle += AngularVelocity;
            if (SteadyColor) return;
            Opacity -= (float)10 / TTL;
            if (Color.R <= 0) return;
            if (Color.G >= 1) { Color.G -= 15; } else { Color.R -= 5; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            var origin = new Vector2(Texture.Width / 2.0F, Texture.Height / 2.0F);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color * Opacity,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}