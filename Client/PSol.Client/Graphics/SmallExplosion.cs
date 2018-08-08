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
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Opacity <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public void Create(Vector2 position)
        {
            int total = 100 + random.Next(50); ;
            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle(position));
            }
        }

        private Particle GenerateNewParticle(Vector2 position)
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 velocity = new Vector2(
                2f * (float)(random.NextDouble() * 2 - 1),
                2f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 1f * (float)(random.NextDouble() * 2 - 1);

            float size = (float)random.NextDouble();
            int ttl = 750 + random.Next(500);

            return new Particle(texture, position, velocity, angle, angularVelocity, size, ttl);
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
