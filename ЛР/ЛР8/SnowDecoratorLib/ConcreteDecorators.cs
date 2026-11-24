using System.Drawing;
using System.Drawing.Drawing2D;

namespace SnowDecoratorLib
{
    // Декоратор 1: М'яке морозне сяйво навколо сніжинки
    public class GlowDecorator : SnowflakeDecorator
    {
        private readonly Color _glowColor;
        private readonly int _intensity;

        public GlowDecorator(ISnowflakeRenderer inner, Color glowColor, int intensity = 3)
            : base(inner)
        {
            _glowColor = glowColor;
            _intensity = Math.Clamp(intensity, 1, 5);
        }

        public override string Description => $"Glow({_wrapped.Description})";

        public override void Draw(Graphics g, SnowflakeState state)
        {
            // Малюємо ореол ПІД основною сніжинкою
            for (int layer = _intensity; layer >= 1; layer--)
            {
                float glowR = state.Radius * (1.0f + layer * 0.32f);
                int alpha = Math.Clamp(52 / layer, 12, 65);
                using var brush = new SolidBrush(Color.FromArgb(alpha, _glowColor));
                g.FillEllipse(brush, state.X - glowR, state.Y - glowR, glowR * 2, glowR * 2);
            }

            // Делегуємо малювання обгорнутому об'єкту
            base.Draw(g, state);
        }
    }

    // Декоратор 2: Морозні гілочки-дендрити на променях сніжинки
    public class BranchDecorator : SnowflakeDecorator
    {
        private readonly int _branchLevels;
        private readonly Color _branchColor;

        public BranchDecorator(ISnowflakeRenderer inner, Color branchColor, int branchLevels = 2)
            : base(inner)
        {
            _branchColor = branchColor;
            _branchLevels = Math.Clamp(branchLevels, 1, 3);
        }

        public override string Description => $"Branch({_wrapped.Description})";

        public override void Draw(Graphics g, SnowflakeState state)
        {
            base.Draw(g, state);

            GraphicsState saved = g.Save();
            g.TranslateTransform(state.X, state.Y);
            g.RotateTransform(state.Angle);

            float r = state.Radius;
            using var pen = new Pen(_branchColor, Math.Max(1.0f, r * 0.09f));
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;

            for (int i = 0; i < 6; i++)
            {
                double rad = i * Math.PI / 3.0;
                for (int lvl = 1; lvl <= _branchLevels; lvl++)
                {
                    float frac = 0.42f + (lvl - 1) * 0.25f;
                    float bx = (float)(Math.Cos(rad) * r * frac);
                    float by = (float)(Math.Sin(rad) * r * frac);
                    float bLen = r * (0.36f - (lvl - 1) * 0.08f);

                    double a1 = rad - 0.58;
                    double a2 = rad + 0.58;
                    g.DrawLine(pen, bx, by, bx + (float)(Math.Cos(a1) * bLen), by + (float)(Math.Sin(a1) * bLen));
                    g.DrawLine(pen, bx, by, bx + (float)(Math.Cos(a2) * bLen), by + (float)(Math.Sin(a2) * bLen));
                }
            }

            g.Restore(saved);
        }
    }

    // Декоратор 3: Мерехтливі кристалічні іскри
    public class SparkleDecorator : SnowflakeDecorator
    {
        private readonly Color _sparkleColor;

        public SparkleDecorator(ISnowflakeRenderer inner, Color sparkleColor)
            : base(inner)
        {
            _sparkleColor = sparkleColor;
        }

        public override string Description => $"Sparkle({_wrapped.Description})";

        public override void Draw(Graphics g, SnowflakeState state)
        {
            base.Draw(g, state);

            // Мерехтіння залежить від фази анімації
            float pulse = (float)(0.45 + 0.55 * Math.Abs(Math.Sin(state.Phase * 2.2)));
            int alpha = Math.Clamp((int)(245 * pulse), 70, 255);
            float sr = state.Radius * (0.45f + 0.28f * pulse);

            using var pen = new Pen(Color.FromArgb(alpha, _sparkleColor), Math.Max(1.1f, state.Radius * 0.09f));
            g.DrawLine(pen, state.X - sr, state.Y, state.X + sr, state.Y);
            g.DrawLine(pen, state.X, state.Y - sr, state.X, state.Y + sr);

            using var core = new SolidBrush(Color.FromArgb(alpha, Color.White));
            float cr = Math.Max(1.6f, state.Radius * 0.16f);
            g.FillEllipse(core, state.X - cr, state.Y - cr, cr * 2, cr * 2);
        }
    }

    // Декоратор 4: Сніговий шлейф руху за сніжинкою
    public class TrailDecorator : SnowflakeDecorator
    {
        private readonly float _trailLength;

        public TrailDecorator(ISnowflakeRenderer inner, float trailLength = 26f)
            : base(inner)
        {
            _trailLength = trailLength;
        }

        public override string Description => $"Trail({_wrapped.Description})";

        public override void Draw(Graphics g, SnowflakeState state)
        {
            float dx = -state.VelocityX * (_trailLength / 4.2f);
            float dy = -Math.Max(1.0f, state.SpeedY) * (_trailLength / 4.2f);

            int segments = 5;
            for (int i = segments; i >= 1; i--)
            {
                float t = i / (float)segments;
                float tx = state.X + dx * t;
                float ty = state.Y + dy * t;
                float tr = state.Radius * (0.36f * (1f - t * 0.75f));
                int alpha = Math.Clamp((int)(110 * (1f - t * 0.85f)), 10, 110);

                using var brush = new SolidBrush(Color.FromArgb(alpha, 210, 238, 255));
                g.FillEllipse(brush, tx - tr, ty - tr, tr * 2, tr * 2);
            }

            base.Draw(g, state);
        }
    }
}
