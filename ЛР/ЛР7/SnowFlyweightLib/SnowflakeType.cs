using System.Drawing;
using System.Drawing.Drawing2D;

namespace SnowFlyweightLib
{
    public enum SnowflakeShape
    {
        Star6,
        Crystal,
        Fluffy,
        Hexagon,
        Sparkle
    }

    // Легковаговик (Flyweight): зберігає спільний (внутрішній) незмінний стан сніжинки
    public class SnowflakeType
    {
        public string Name { get; }
        public SnowflakeShape Shape { get; }
        public Color Color { get; }
        public float BaseSize { get; }

        public SnowflakeType(string name, SnowflakeShape shape, Color color, float baseSize)
        {
            Name = name;
            Shape = shape;
            Color = color;
            BaseSize = baseSize;
        }

        // Зовнішній стан (x, y, scale, angle) передається ззовні об'єктом-контекстом Snowflake
        public void Draw(Graphics g, float x, float y, float scale, float angleDegrees)
        {
            float r = BaseSize * scale;
            GraphicsState state = g.Save();

            g.TranslateTransform(x, y);
            g.RotateTransform(angleDegrees);

            switch (Shape)
            {
                case SnowflakeShape.Star6:
                    DrawStar6(g, r);
                    break;
                case SnowflakeShape.Crystal:
                    DrawCrystal(g, r);
                    break;
                case SnowflakeShape.Fluffy:
                    DrawFluffy(g, r);
                    break;
                case SnowflakeShape.Hexagon:
                    DrawHexagon(g, r);
                    break;
                case SnowflakeShape.Sparkle:
                    DrawSparkle(g, r);
                    break;
            }

            g.Restore(state);
        }

        private void DrawStar6(Graphics g, float r)
        {
            using var pen = new Pen(Color, Math.Max(1.1f, r * 0.14f));
            for (int i = 0; i < 6; i++)
            {
                double rad = i * Math.PI / 3.0;
                float dx = (float)(Math.Cos(rad) * r);
                float dy = (float)(Math.Sin(rad) * r);
                g.DrawLine(pen, 0, 0, dx, dy);

                // Маленькі гілочки на променях
                float bx = dx * 0.62f;
                float by = dy * 0.62f;
                double leftA = rad - 0.55;
                double rightA = rad + 0.55;
                float branchLen = r * 0.35f;
                g.DrawLine(pen, bx, by, bx + (float)(Math.Cos(leftA) * branchLen), by + (float)(Math.Sin(leftA) * branchLen));
                g.DrawLine(pen, bx, by, bx + (float)(Math.Cos(rightA) * branchLen), by + (float)(Math.Sin(rightA) * branchLen));
            }
        }

        private void DrawCrystal(Graphics g, float r)
        {
            using var pen = new Pen(Color, Math.Max(1.0f, r * 0.12f));
            for (int i = 0; i < 6; i++)
            {
                double rad = i * Math.PI / 3.0;
                float dx = (float)(Math.Cos(rad) * r);
                float dy = (float)(Math.Sin(rad) * r);
                g.DrawLine(pen, 0, 0, dx, dy);
            }
            // Внутрішнє кільце кристала
            float ir = r * 0.48f;
            PointF[] hex = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                double rad = i * Math.PI / 3.0;
                hex[i] = new PointF((float)(Math.Cos(rad) * ir), (float)(Math.Sin(rad) * ir));
            }
            g.DrawPolygon(pen, hex);
        }

        private void DrawFluffy(Graphics g, float r)
        {
            using var outerBrush = new SolidBrush(Color.FromArgb(95, Color));
            using var coreBrush = new SolidBrush(Color.FromArgb(225, Color));
            g.FillEllipse(outerBrush, -r, -r, r * 2, r * 2);
            g.FillEllipse(coreBrush, -r * 0.55f, -r * 0.55f, r * 1.1f, r * 1.1f);
        }

        private void DrawHexagon(Graphics g, float r)
        {
            PointF[] pts = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                double rad = i * Math.PI / 3.0;
                pts[i] = new PointF((float)(Math.Cos(rad) * r), (float)(Math.Sin(rad) * r));
            }
            using var fill = new SolidBrush(Color.FromArgb(140, Color));
            using var pen = new Pen(Color, 1.3f);
            g.FillPolygon(fill, pts);
            g.DrawPolygon(pen, pts);
        }

        private void DrawSparkle(Graphics g, float r)
        {
            using var pen = new Pen(Color, Math.Max(1.1f, r * 0.16f));
            g.DrawLine(pen, -r, 0, r, 0);
            g.DrawLine(pen, 0, -r, 0, r);
            float d = r * 0.55f;
            g.DrawLine(pen, -d, -d, d, d);
            g.DrawLine(pen, -d, d, d, -d);
        }
    }
}
