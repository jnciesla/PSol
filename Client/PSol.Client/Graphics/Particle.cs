using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Size = size;
            TTL = ttl;
            Opacity = 1f;

            Color = new Color
            {
                R = 255,
                G = 255,
                B = 0
            };
        }

        public void Update()
        {
            Position += Velocity;
            Angle += AngularVelocity;
            Opacity -= (float)10 / TTL;
            if (Color.G >= 1) { Color.G -= 15; } else { Color.R -= 15; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color * Opacity,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}