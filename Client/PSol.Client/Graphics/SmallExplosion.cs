using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    public class SmallExplosion
    {
        private readonly Random random;
        public Vector2 EmitterLocation { get; set; }
        private readonly List<Particle> particles;
        private readonly List<Texture2D> textures;

        public SmallExplosion(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
        }

        public void Update()
        {
            for (var particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (!(particles[particle].Opacity <= 0)) continue;
                particles.RemoveAt(particle);
                particle--;
                if (particles.Count == 0)
                {
                    // Remove from the list of explosions when it's over
                    Game1.Explosion.Remove(this);
                }
            }
        }

        public void Create(Vector2 position)
        {
            var total = 100 + random.Next(50); ;
            for (var i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle(position));
            }
        }

        private Particle GenerateNewParticle(Vector2 position)
        {
            var textureInt = random.Next(textures.Count);
            var texture = textures[textureInt];
            var velocity = new Vector2(
                2f * (float)(random.NextDouble() * 2 - 1),
                2f * (float)(random.NextDouble() * 2 - 1));
            var angularVelocity = 1f * (float)(random.NextDouble() * 2 - 1);

            var size = (float)random.NextDouble();
            var ttl = 750 + random.Next(500);
            var Color = new Color { R = 255, G = 255, B = 0 };
            if (textureInt == 0 && random.Next(2) == 1 && textures.Count > 1)
            {
                Color = Color.Black;
            }
            return new Particle(texture, position, velocity, 0, angularVelocity, size, ttl, Color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var t in particles)
            {
                t.Draw(spriteBatch);
            }
        }
    }
}
