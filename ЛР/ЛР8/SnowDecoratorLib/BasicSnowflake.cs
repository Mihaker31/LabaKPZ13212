using System.Drawing;
using System.Drawing.Drawing2D;

namespace SnowDecoratorLib
{
    // Базовий компонент (ConcreteComponent) — проста сніжинка без додаткових ефектів
    public class BasicSnowflake : ISnowflakeRenderer
    {
        public BaseSnowflakeShape BaseShape { get; }
        public Color BaseColor { get; }

        public BasicSnowflake(BaseSnowflakeShape shape, Color color)
        {
            BaseShape = shape;
            BaseColor = color;
        }

        public string Description => BaseShape switch
        {
            BaseSnowflakeShape.SixRayStar => "BasicSnowflake(6-променева)",
            BaseSnowflakeShape.HexagonPlate => "BasicSnowflake(Шестикутник)",
            _ => "BasicSnowflake(Сніжна кулька)"
        };

        public void Draw(Graphics g, SnowflakeState state)
        {
            GraphicsState saved = g.Save();
            g.TranslateTransform(state.X, state.Y);
            g.RotateTransform(state.Angle);

            float r = state.Radius;

            switch (BaseShape)
            {
                case BaseSnowflakeShape.SixRayStar:
                    using (var pen = new Pen(BaseColor, Math.Max(1.4f, r * 0.14f)))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        for (int i = 0; i < 6; i++)
                        {
                            double rad = i * Math.PI / 3.0;
                            g.DrawLine(pen, 0, 0, (float)(Math.Cos(rad) * r), (float)(Math.Sin(rad) * r));
                        }
                    }
                    break;

                case BaseSnowflakeShape.HexagonPlate:
                    PointF[] pts = new PointF[6];
                    for (int i = 0; i < 6; i++)
                    {
                        double rad = i * Math.PI / 3.0;
                        pts[i] = new PointF((float)(Math.Cos(rad) * r * 0.78f), (float)(Math.Sin(rad) * r * 0.78f));
                    }
                    using (var fill = new SolidBrush(Color.FromArgb(145, BaseColor)))
                    using (var pen = new Pen(BaseColor, Math.Max(1.2f, r * 0.11f)))
                    {
                        g.FillPolygon(fill, pts);
                        g.DrawPolygon(pen, pts);
                    }
                    break;

                case BaseSnowflakeShape.IceDot:
                    using (var brush = new SolidBrush(Color.FromArgb(225, BaseColor)))
                    {
                        float dr = r * 0.52f;
                        g.FillEllipse(brush, -dr, -dr, dr * 2, dr * 2);
                    }
                    break;
            }

            g.Restore(saved);
        }
    }
}
