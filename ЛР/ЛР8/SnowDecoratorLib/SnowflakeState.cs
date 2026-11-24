namespace SnowDecoratorLib
{
    public enum BaseSnowflakeShape
    {
        SixRayStar,
        HexagonPlate,
        IceDot
    }

    // Стан окремої сніжинки на полотні (передається декораторам для малювання)
    public class SnowflakeState
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public float Angle { get; set; }
        public float SpeedY { get; set; }
        public float VelocityX { get; set; }
        public float RotationSpeed { get; set; }
        public float Phase { get; set; }

        public void Update(int width, int height, float wind)
        {
            Phase += 0.05f;
            VelocityX = wind + (float)Math.Sin(Phase) * 0.7f;
            X += VelocityX;
            Y += SpeedY;
            Angle = (Angle + RotationSpeed) % 360f;

            if (Y > height + Radius * 3)
            {
                Y = -Radius * 3;
                X = Random.Shared.Next(0, Math.Max(1, width));
            }
            if (X < -Radius * 3) X = width + Radius * 2;
            if (X > width + Radius * 3) X = -Radius * 2;
        }
    }
}
