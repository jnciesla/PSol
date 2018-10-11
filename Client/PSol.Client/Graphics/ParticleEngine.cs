using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    public class ParticleEngine
    {
        private readonly Random random;
        public Vector2 EmitterLocation { get; set; }
        private readonly List<Particle> particles;
        private readonly List<Texture2D> textures;

        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
        }

        public void Update(bool moving)
        {
            var total = 30;

            for (var i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle(moving));
            }

            for (var particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (!(particles[particle].Opacity <= 0)) continue;
                particles.RemoveAt(particle);
                particle--;
            }
        }

        private Particle GenerateNewParticle(bool moving)
        {
            var texture = textures[random.Next(textures.Count)];
            var position = EmitterLocation;
            var velocity = new Vector2(1.25f * (float)(random.NextDouble() * 2 - 1), 1.25f * (float)(random.NextDouble() * 2 - 1));
            var angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            var size = (float)random.NextDouble();
            var ttl = 1;
            if (moving)
            {
                ttl = 200 + random.Next(100);
            }
            var Color = new Color
            {
                R = 255,
                G = 255,
                B = 0
            };

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