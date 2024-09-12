using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zzz
{
    public class ParticleTriangle
    {

        public enum TipoAni
        {
            Libre,
            Fija
        }

        private TipoAni _tipoAni;

        private struct Particle
        {
            public PointF Position;
            public float Size;
            public Color Color;
            public float Rotation;
            public float Speed;
            public float RotationSpeed;

            public Particle(PointF position, float size, Color color, float rotation, float speed, float rotationSpeed)
            {
                Position = position;
                Size = size;
                Color = color;
                Rotation = rotation;
                Speed = speed;
                RotationSpeed = rotationSpeed;
            }
        }

        private struct ImVec2
        {
            public float x, y;
            public ImVec2(float x, float y) { this.x = x; this.y = y; }
        }

        private static ImVec2 Lerp(PointF a, PointF b, float t)
        {
            return new ImVec2(a.X + (b.X- a.X) * t, a.X + (b.X - a.X) * t);
        }

        private float DeltaTime => (float)(DateTime.Now - lastFrameTime).TotalSeconds;
        private DateTime lastFrameTime = DateTime.Now;

        private Random rand = new Random();
        private Particle[] particles;
        private int screenWidth, screenHeight;

        public ParticleTriangle(int screenWidth, int screenHeight, int particleCount, TipoAni animacion, Color color = default)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            particles = new Particle[particleCount];
            _tipoAni = animacion;

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = CreateRandomParticle(color);
            }
        }

        private Particle CreateRandomParticle(Color color = default)
        {
            float size = rand.Next(5, 30);// Tamaño aleatorio por defecto ente valores de 5 a 30
            float speed = (float)rand.NextDouble() * 1 + 1; // Velocidad Aleatoria de 1 a 2
            float rotation = (float) rand.NextDouble() * 360; //Rotacion Aleatoria 
            float rotationSpeed = (float)rand.NextDouble() * 4 - 2; // Velocidad de rotacion Aleatoria de 4 a -2

            if (color == default(Color))
            {
                color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            }

            return new Particle(
                new PointF(rand.Next(screenWidth), rand.Next(screenHeight)),
                size, 
                color,
                rotation,
                speed,
                rotationSpeed
            );
        }

        public void Update()
        {
            switch (_tipoAni) {
                case TipoAni.Libre:
                    for (int i = 0; i < particles.Length; i++)
                     {
                         float angle = particles[i].Rotation * (float)(Math.PI / 180);
                         particles[i].Position.X += (float)Math.Cos(angle) * particles[i].Speed;
                         particles[i].Position.Y += (float)Math.Sin(angle) * particles[i].Speed;
                         particles[i].Rotation += particles[i].RotationSpeed;


                         if (particles[i].Position.X > screenWidth || particles[i].Position.Y > screenHeight)
                         {
                             PointF posnew = new PointF(rand.Next(screenWidth),-rand.Next(screenHeight));
                             particles[i].Rotation = (float)rand.NextDouble() * 360;
                             particles[i].Speed = (float)rand.NextDouble() * 4 + 2;
                             particles[i].Position.X = posnew.X;
                             particles[i].Position.Y = posnew.Y;
                         }
                    }
                    break;
                case TipoAni.Fija:
                    for (int i = 0; i < particles.Length; i++)
                    {
                        particles[i].Position.X += particles[i].Speed;
                        particles[i].Position.Y += particles[i].Speed;
                        particles[i].Rotation += particles[i].RotationSpeed;

                        if (particles[i].Position.X > screenWidth || particles[i].Position.Y > screenHeight)
                        {
                            PointF posnew = new PointF(rand.Next(screenWidth), -rand.Next(screenHeight));
                            particles[i].Rotation = (float)rand.NextDouble() * 360;
                            particles[i].Speed = (float)rand.NextDouble() * 4 + 2;
                            particles[i].Position.X = posnew.X / particles[i].Size;
                            particles[i].Position.Y = posnew.Y / particles[i].Size;
                        }
                    }
                    break;
            }
    }

        public void Draw(Graphics g)
        {
            foreach (var particle in particles)
            {
                DrawRotatedTriangle(g, particle);
            }
        }

        private void DrawRotatedTriangle(Graphics g, Particle particle)
        {
            PointF[] triangle = new PointF[3];
            float halfSize = particle.Size / 2;

            triangle[0] = new PointF(-halfSize, halfSize);
            triangle[1] = new PointF(halfSize, halfSize);
            triangle[2] = new PointF(0, -halfSize);

            using (Matrix matrix = new Matrix())
            {
                matrix.RotateAt(particle.Rotation, particle.Position);
                matrix.Translate(particle.Position.X, particle.Position.Y, MatrixOrder.Append);
                matrix.TransformPoints(triangle);
            }

            using (Brush brush = new SolidBrush(particle.Color))
            {
                g.FillPolygon(brush, triangle);
            }
        }
    }
}
