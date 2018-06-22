using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PSol.Client
{
    class Laser : ILaser
    {
        public List<Beam> Segments = new List<Beam>();
        public bool IsComplete { get { return Alpha <= 0; } }

        public float Alpha { get; set; }
        public float AlphaMultiplier { get; set; }
        public float FadeOutRate { get; set; }
        public Color Tint { get; set; }

        static Random rand = new Random();

        public Laser(Vector2 source, Vector2 dest) : this(source, dest, new Color(0.9f, 0.8f, 1f)) { }

        public Laser(Vector2 source, Vector2 dest, Color color)
        {
            Segments = CreateBolt(source, dest, 2);

            Tint = color;
            Alpha = 1f;
            AlphaMultiplier = 0.6f;
            FadeOutRate = 0.03f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alpha <= 0)
                return;

            foreach (var segment in Segments)
                segment.Draw(spriteBatch, Tint * (Alpha * AlphaMultiplier));
        }

        public virtual void Update()
        {
            Alpha -= FadeOutRate;
        }

        protected static List<Beam> CreateBolt(Vector2 source, Vector2 dest, float thickness)
        {
            var results = new List<Beam>();
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++)
                positions.Add(Rand(0, 1));

            positions.Sort();

            const float Sway = 80;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Rand(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(new Beam(prevPoint, point, thickness));
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(new Beam(prevPoint, dest, thickness));

            return results;
        }

        private static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }

        public class Beam
        {
            public Vector2 A;
            public Vector2 B;
            public float Thickness;
            public float Alpha = 1F;
            public float FadeOutRate;

            public Beam() { }
            public Beam(Vector2 a, Vector2 b, float thickness = 1)
            {
                A = a;
                B = b;
                Thickness = thickness;
                FadeOutRate = 0.03f;
            }

            public void Draw(SpriteBatch spriteBatch, Color tint)
            {
                Vector2 tangent = B - A;
                float theta = (float)Math.Atan2(tangent.Y, tangent.X);

                const float ImageThickness = 8;
                float thicknessScale = Thickness / ImageThickness;
                Vector2 middleOrigin = new Vector2(0, Graphics.laserMid.Height / 2f);
                Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

                spriteBatch.Draw(Graphics.laserMid, A, null, tint * Alpha, theta, middleOrigin, middleScale, SpriteEffects.None, 0f);
            }

            public virtual void Update()
            {
                Alpha -= FadeOutRate;
            }
        }
    }
}
