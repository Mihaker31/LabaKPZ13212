using System.Drawing;
using System.Drawing.Drawing2D;
using SnowDecoratorLib;

namespace SnowDecoratorWinForms
{
    public partial class Form1 : Form
    {
        private readonly List<SnowflakeState> _snowflakes = new List<SnowflakeState>();
        private readonly SnowflakeState _previewFlake = new SnowflakeState
        {
            X = 130,
            Y = 95,
            Radius = 46,
            Angle = 15,
            SpeedY = 2.4f,
            VelocityX = 0.8f,
            RotationSpeed = 0.6f,
            Phase = 0
        };

        private ISnowflakeRenderer _renderer = new BasicSnowflake(BaseSnowflakeShape.SixRayStar, Color.White);
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private float _wind = 0.6f;

        // UI Controls
        private ComboBox cmbBaseShape = null!;
        private ComboBox cmbColorScheme = null!;
        private CheckBox chkGlow = null!;
        private CheckBox chkBranches = null!;
        private CheckBox chkSparkle = null!;
        private CheckBox chkTrail = null!;
        private TrackBar trkCount = null!;
        private TrackBar trkWind = null!;
        private Label lblCount = null!;
        private Label lblWind = null!;
        private TextBox txtChain = null!;
        private DoubleBufferedPanel pnlPreview = null!;
        private DoubleBufferedPanel pnlSky = null!;

        public Form1()
        {
            InitializeLayout();
            RebuildDecoratorChain();
            GenerateSnowflakes(140);

            _timer.Interval = 22; // ~45 FPS
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void InitializeLayout()
        {
            Text = "Лаб 8: Патерн Декоратор (Decorator) — Варіант 14: Декоратор для сніжинок";
            Size = new Size(1180, 740);
            MinimumSize = new Size(980, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5f);

            var rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Group 1: Base Component (ConcreteComponent)
            var grpBase = new GroupBox
            {
                Text = "1. Базова сніжинка (BasicSnowflake)",
                Location = new Point(10, 8),
                Size = new Size(318, 122),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            var lblShape = new Label
            {
                Text = "Базова форма:",
                Location = new Point(12, 26),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cmbBaseShape = new ComboBox
            {
                Location = new Point(12, 46),
                Width = 294,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cmbBaseShape.Items.AddRange(new object[]
            {
                "6-променева зірка (SixRayStar)",
                "Крижаний шестикутник (HexagonPlate)",
                "Сніжна кулька (IceDot)"
            });
            cmbBaseShape.SelectedIndex = 0;
            cmbBaseShape.SelectedIndexChanged += (s, e) => RebuildDecoratorChain();

            var lblCol = new Label
            {
                Text = "Кольорова палітра:",
                Location = new Point(12, 76),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cmbColorScheme = new ComboBox
            {
                Location = new Point(140, 72),
                Width = 166,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cmbColorScheme.Items.AddRange(new object[]
            {
                "Крижано-білий",
                "Блакитний іній",
                "Золотистий мороз",
                "Північне сяйво"
            });
            cmbColorScheme.SelectedIndex = 0;
            cmbColorScheme.SelectedIndexChanged += (s, e) => RebuildDecoratorChain();

            grpBase.Controls.AddRange(new Control[] { lblShape, cmbBaseShape, lblCol, cmbColorScheme });

            // Group 2: Stackable Decorators
            var grpDec = new GroupBox
            {
                Text = "2. Декоратори (комбінуються динамічно)",
                Location = new Point(10, 136),
                Size = new Size(318, 152),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            chkBranches = MakeCheck("BranchDecorator — гілочки-дендрити", 26, true);
            chkGlow = MakeCheck("GlowDecorator — морозне сяйво (ореол)", 54, true);
            chkSparkle = MakeCheck("SparkleDecorator — мерехтливі іскри", 82, true);
            chkTrail = MakeCheck("TrailDecorator — сніговий шлейф руху", 110, false);

            grpDec.Controls.AddRange(new Control[] { chkBranches, chkGlow, chkSparkle, chkTrail });

            // Group 3: Live Magnified Preview + Chain Info
            var grpPreview = new GroupBox
            {
                Text = "3. Збільшений перегляд і ланцюжок обгорток",
                Location = new Point(10, 294),
                Size = new Size(318, 252),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            pnlPreview = new DoubleBufferedPanel
            {
                Location = new Point(12, 24),
                Size = new Size(294, 156),
                BackColor = Color.FromArgb(14, 24, 48),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlPreview.Paint += PnlPreview_Paint;

            txtChain = new TextBox
            {
                Location = new Point(12, 188),
                Size = new Size(294, 54),
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 8.5f),
                BackColor = Color.FromArgb(248, 250, 253)
            };
            grpPreview.Controls.AddRange(new Control[] { pnlPreview, txtChain });

            // Group 4: Snowfall controls
            var grpEnv = new GroupBox
            {
                Text = "4. Параметри снігопаду",
                Location = new Point(10, 552),
                Size = new Size(318, 136),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            lblCount = new Label
            {
                Text = "Кількість сніжинок: 140",
                Location = new Point(12, 24),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            trkCount = new TrackBar
            {
                Location = new Point(10, 42),
                Size = new Size(296, 36),
                Minimum = 20,
                Maximum = 350,
                TickFrequency = 50,
                Value = 140
            };
            trkCount.Scroll += (s, e) =>
            {
                lblCount.Text = $"Кількість сніжинок: {trkCount.Value}";
                GenerateSnowflakes(trkCount.Value);
            };

            lblWind = new Label
            {
                Text = "Вітер: +0.6",
                Location = new Point(12, 78),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            trkWind = new TrackBar
            {
                Location = new Point(10, 96),
                Size = new Size(296, 36),
                Minimum = -40,
                Maximum = 40,
                TickFrequency = 10,
                Value = 6
            };
            trkWind.Scroll += (s, e) =>
            {
                _wind = trkWind.Value / 10f;
                lblWind.Text = $"Вітер: {_wind:+0.0;-0.0;0.0}";
            };

            grpEnv.Controls.AddRange(new Control[] { lblCount, trkCount, lblWind, trkWind });

            rightPanel.Controls.AddRange(new Control[] { grpBase, grpDec, grpPreview, grpEnv });

            // Main Snowfall Canvas (Left)
            pnlSky = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 22, 46)
            };
            pnlSky.Paint += PnlSky_Paint;

            Controls.Add(pnlSky);
            Controls.Add(rightPanel);
        }

        private CheckBox MakeCheck(string text, int y, bool isChecked)
        {
            var chk = new CheckBox
            {
                Text = text,
                Location = new Point(12, y),
                Size = new Size(296, 24),
                Checked = isChecked,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            chk.CheckedChanged += (s, e) => RebuildDecoratorChain();
            return chk;
        }

        private (Color mainCol, Color glowCol, Color sparkleCol) GetCurrentPalette()
        {
            return cmbColorScheme.SelectedIndex switch
            {
                1 => (Color.FromArgb(190, 232, 255), Color.DeepSkyBlue, Color.Cyan),
                2 => (Color.FromArgb(255, 244, 210), Color.Gold, Color.LightYellow),
                3 => (Color.FromArgb(226, 210, 255), Color.MediumOrchid, Color.Aqua),
                _ => (Color.White, Color.LightSkyBlue, Color.White)
            };
        }

        private void RebuildDecoratorChain()
        {
            var shape = cmbBaseShape.SelectedIndex switch
            {
                1 => BaseSnowflakeShape.HexagonPlate,
                2 => BaseSnowflakeShape.IceDot,
                _ => BaseSnowflakeShape.SixRayStar
            };

            var (mainCol, glowCol, sparkleCol) = GetCurrentPalette();

            // Динамічне складання ланцюжка декораторів
            ISnowflakeRenderer current = new BasicSnowflake(shape, mainCol);

            if (chkBranches.Checked)
                current = new BranchDecorator(current, mainCol, branchLevels: 2);

            if (chkGlow.Checked)
                current = new GlowDecorator(current, glowCol, intensity: 3);

            if (chkSparkle.Checked)
                current = new SparkleDecorator(current, sparkleCol);

            if (chkTrail.Checked)
                current = new TrailDecorator(current, trailLength: 26f);

            _renderer = current;
            txtChain.Text = "Активний ланцюжок:\r\n" + _renderer.Description;

            pnlPreview.Invalidate();
            pnlSky.Invalidate();
        }

        private void GenerateSnowflakes(int count)
        {
            _snowflakes.Clear();
            int w = Math.Max(700, pnlSky.Width);
            int h = Math.Max(600, pnlSky.Height);

            for (int i = 0; i < count; i++)
            {
                _snowflakes.Add(new SnowflakeState
                {
                    X = Random.Shared.Next(0, w),
                    Y = Random.Shared.Next(0, h),
                    Radius = 6f + (float)Random.Shared.NextDouble() * 11f,
                    Angle = Random.Shared.Next(0, 360),
                    SpeedY = 1.0f + (float)Random.Shared.NextDouble() * 2.2f,
                    RotationSpeed = -1.5f + (float)Random.Shared.NextDouble() * 3.0f,
                    Phase = (float)(Random.Shared.NextDouble() * Math.PI * 2)
                });
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            int w = Math.Max(100, pnlSky.Width);
            int h = Math.Max(100, pnlSky.Height);

            foreach (var f in _snowflakes)
                f.Update(w, h, _wind);

            _previewFlake.Angle = (_previewFlake.Angle + 0.6f) % 360f;
            _previewFlake.Phase += 0.05f;
            _previewFlake.VelocityX = _wind;
            _previewFlake.X = pnlPreview.Width / 2f;
            _previewFlake.Y = pnlPreview.Height / 2f + 6f;

            pnlSky.Invalidate();
            pnlPreview.Invalidate();
        }

        private void PnlPreview_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = pnlPreview.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var bg = new LinearGradientBrush(rect, Color.FromArgb(12, 22, 46), Color.FromArgb(26, 46, 86), 90f))
                g.FillRectangle(bg, rect);

            _renderer.Draw(g, _previewFlake);
        }

        private void PnlSky_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = pnlSky.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Градієнт нічного неба
            using (var sky = new LinearGradientBrush(rect, Color.FromArgb(8, 16, 38), Color.FromArgb(26, 52, 92), 90f))
                g.FillRectangle(sky, rect);

            // Засніжені кучугури знизу
            using (var hill1 = new SolidBrush(Color.FromArgb(182, 205, 228)))
                g.FillEllipse(hill1, -120, rect.Height - 105, rect.Width + 240, 210);
            using (var hill2 = new SolidBrush(Color.FromArgb(232, 242, 252)))
                g.FillEllipse(hill2, -80, rect.Height - 62, rect.Width + 160, 150);

            // Малюємо кожну сніжинку через поточний декорований рендерер
            foreach (var f in _snowflakes)
                _renderer.Draw(g, f);

            // Верхній HUD-бейдж
            using var hudFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            string hud = $"Сніжинок: {_snowflakes.Count}  |  {_renderer.Description}";
            var sz = g.MeasureString(hud, hudFont);
            using var bg = new SolidBrush(Color.FromArgb(145, 10, 18, 36));
            g.FillRectangle(bg, 10, 10, Math.Min(rect.Width - 20, sz.Width + 18), sz.Height + 10);
            g.DrawString(hud, hudFont, Brushes.AliceBlue, 18, 15);
        }
    }
}
