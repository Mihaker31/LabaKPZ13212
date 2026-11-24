using System.Drawing;

namespace SnowFlyweightLib
{
    // Контекст (Context): зберігає лише унікальні (зовнішні) координати та швидкість кожної сніжинки
    public class Snowflake
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float SpeedY { get; set; }
        public float DriftPhase { get; set; }
        public float Scale { get; set; }
        public float Angle { get; set; }
        public float RotationSpeed { get; set; }

        // Посилання на спільний об'єкт-легковаговик
        public SnowflakeType Type { get; }

        public Snowflake(float x, float y, float speedY, float driftPhase, float scale, float angle, float rotationSpeed, SnowflakeType type)
        {
            X = x;
            Y = y;
            SpeedY = speedY;
            DriftPhase = driftPhase;
            Scale = scale;
            Angle = angle;
            RotationSpeed = rotationSpeed;
            Type = type;
        }

        public void Update(int width, int height, float wind, float speedMultiplier)
        {
            DriftPhase += 0.04f;
            Y += SpeedY * speedMultiplier;
            X += wind + (float)Math.Sin(DriftPhase) * 0.65f;
            Angle = (Angle + RotationSpeed) % 360f;

            if (Y > height + 18)
            {
                Y = -18;
                X = Random.Shared.Next(0, Math.Max(1, width));
            }
            if (X < -20) X = width + 18;
            if (X > width + 20) X = -18;
        }

        public void Draw(Graphics g)
        {
            Type.Draw(g, X, Y, Scale, Angle);
        }
    }
}
