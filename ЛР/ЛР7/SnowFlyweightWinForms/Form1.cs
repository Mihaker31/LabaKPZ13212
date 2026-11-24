using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using SnowFlyweightLib;

namespace SnowFlyweightWinForms
{
    public partial class Form1 : Form
    {
        private readonly SnowfallSimulation _simulation = new SnowfallSimulation();
        private readonly System.Windows.Forms.Timer _animTimer = new System.Windows.Forms.Timer();

        private SnowCanvasPanel canvas = null!;
        private readonly List<CheckBox> _presetChecks = new List<CheckBox>();
        private TrackBar trkCount = null!;
        private TrackBar trkWind = null!;
        private TrackBar trkSpeed = null!;
        private Label lblCountVal = null!;
        private Label lblWindVal = null!;
        private Label lblSpeedVal = null!;
        private Button btnToggle = null!;
        private TextBox txtStats = null!;
        private bool _isRunning = true;

        public Form1()
        {
            InitializeLayout();

            _animTimer.Interval = 20; // ~50 FPS
            _animTimer.Tick += AnimTimer_Tick;

            RebuildSnow();
            _animTimer.Start();
        }

        private void InitializeLayout()
        {
            Text = "Лаб 7: Патерн Легковаговик (Flyweight) — Варіант 14: Імітація снігу";
            Size = new Size(1160, 720);
            MinimumSize = new Size(980, 620);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5f);

            // Right control panel
            var rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Group 1: Snowflake types set
            var grpTypes = new GroupBox
            {
                Text = "Набір сніжинок (Flyweight типи)",
                Location = new Point(10, 10),
                Size = new Size(298, 165),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            for (int i = 0; i < SnowfallSimulation.DefaultPresets.Length; i++)
            {
                var preset = SnowfallSimulation.DefaultPresets[i];
                var chk = new CheckBox
                {
                    Text = $"{preset.Name} ({preset.Shape}, розмір {preset.BaseSize})",
                    Checked = true,
                    Location = new Point(12, 26 + i * 26),
                    Size = new Size(274, 24),
                    Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                    Tag = preset
                };
                chk.CheckedChanged += (s, e) => RebuildSnow();
                _presetChecks.Add(chk);
                grpTypes.Controls.Add(chk);
            }

            // Group 2: Parameters
            var grpParams = new GroupBox
            {
                Text = "Параметри снігопаду",
                Location = new Point(10, 182),
                Size = new Size(298, 250),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            lblCountVal = new Label
            {
                Text = "Кількість сніжинок: 450",
                Location = new Point(12, 26),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            trkCount = new TrackBar
            {
                Location = new Point(10, 46),
                Size = new Size(276, 40),
                Minimum = 50,
                Maximum = 2500,
                TickFrequency = 250,
                Value = 450
            };
            trkCount.Scroll += (s, e) =>
            {
                lblCountVal.Text = $"Кількість сніжинок: {trkCount.Value}";
                RebuildSnow();
            };

            lblWindVal = new Label
            {
                Text = "Вітер: +0.6",
                Location = new Point(12, 92),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            trkWind = new TrackBar
            {
                Location = new Point(10, 112),
                Size = new Size(276, 40),
                Minimum = -50,
                Maximum = 50,
                TickFrequency = 10,
                Value = 6
            };
            trkWind.Scroll += (s, e) =>
            {
                _simulation.Wind = trkWind.Value / 10f;
                lblWindVal.Text = $"Вітер: {_simulation.Wind:+0.0;-0.0;0.0}";
            };

            lblSpeedVal = new Label
            {
                Text = "Швидкість падіння: 1.0x",
                Location = new Point(12, 156),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            trkSpeed = new TrackBar
            {
                Location = new Point(10, 176),
                Size = new Size(276, 40),
                Minimum = 2,
                Maximum = 30,
                TickFrequency = 5,
                Value = 10
            };
            trkSpeed.Scroll += (s, e) =>
            {
                _simulation.SpeedMultiplier = trkSpeed.Value / 10f;
                lblSpeedVal.Text = $"Швидкість падіння: {_simulation.SpeedMultiplier:0.0}x";
            };

            btnToggle = new Button
            {
                Text = "Пауза",
                Location = new Point(12, 212),
                Size = new Size(132, 30),
                BackColor = Color.FromArgb(42, 122, 226),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnToggle.Click += (s, e) =>
            {
                _isRunning = !_isRunning;
                btnToggle.Text = _isRunning ? "Пауза" : "Продовжити";
                if (_isRunning) _animTimer.Start(); else _animTimer.Stop();
            };

            var btnBoost = new Button
            {
                Text = "+500 сніжинок",
                Location = new Point(152, 212),
                Size = new Size(134, 30),
                BackColor = Color.FromArgb(46, 150, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnBoost.Click += (s, e) =>
            {
                trkCount.Value = Math.Min(trkCount.Maximum, trkCount.Value + 500);
                lblCountVal.Text = $"Кількість сніжинок: {trkCount.Value}";
                RebuildSnow();
            };

            grpParams.Controls.AddRange(new Control[]
            {
                lblCountVal, trkCount, lblWindVal, trkWind, lblSpeedVal, trkSpeed, btnToggle, btnBoost
            });

            // Group 3: Flyweight Memory & Cache Stats
            var grpStats = new GroupBox
            {
                Text = "Статистика патерну Flyweight",
                Location = new Point(10, 440),
                Size = new Size(298, 228),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            txtStats = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 24),
                Size = new Size(278, 194),
                Font = new Font("Consolas", 8.8f),
                BackColor = Color.FromArgb(248, 250, 253),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            grpStats.Controls.Add(txtStats);

            rightPanel.Controls.AddRange(new Control[] { grpTypes, grpParams, grpStats });

            // Left Snowfall Canvas
            canvas = new SnowCanvasPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 24, 48)
            };
            canvas.Paint += Canvas_Paint;

            Controls.Add(canvas);
            Controls.Add(rightPanel);
        }

        private void RebuildSnow()
        {
            var enabled = _presetChecks
                .Where(c => c.Checked && c.Tag is SnowflakePreset)
                .Select(c => (SnowflakePreset)c.Tag!)
                .ToList();

            _simulation.RebuildSnowfall(trkCount.Value, enabled, Math.Max(600, canvas.Width), Math.Max(500, canvas.Height));
            UpdateStatsPanel();
            canvas.Invalidate();
        }

        private void UpdateStatsPanel()
        {
            int total = _simulation.SnowflakeCount;
            int cached = _simulation.CachedFlyweightCount;

            // Умовна оцінка пам'яті: один важкий графічний тип ~4 КБ, легкий контекст ~40 байт
            double withoutFlyweightKb = total * 4.04;
            double withFlyweightKb = (cached * 4.0) + (total * 0.04);
            double savedPercent = withoutFlyweightKb > 0 ? (1.0 - withFlyweightKb / withoutFlyweightKb) * 100.0 : 0;

            var sb = new StringBuilder();
            sb.AppendLine($"Сніжинок на екрані: {total}");
            sb.AppendLine($"Flyweight-типів у кеші: {cached}");
            sb.AppendLine("-----------------------------");
            sb.AppendLine($"Без Flyweight: ~{withoutFlyweightKb:N1} КБ");
            sb.AppendLine($"З Flyweight:   ~{withFlyweightKb:N1} КБ");
            sb.AppendLine($"Економія RAM:   {savedPercent:N1}%");
            sb.AppendLine("-----------------------------");
            sb.AppendLine("Розподіл за видами:");

            foreach (var kv in _simulation.GetCountsByType().OrderByDescending(k => k.Value))
            {
                sb.AppendLine($"  {kv.Key,-12}: {kv.Value} шт.");
            }

            txtStats.Text = sb.ToString();
        }

        private void AnimTimer_Tick(object? sender, EventArgs e)
        {
            _simulation.Update(canvas.Width, canvas.Height);
            canvas.Invalidate();
        }

        private void Canvas_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = canvas.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Градієнт нічного зимового неба
            using (var sky = new LinearGradientBrush(rect, Color.FromArgb(10, 18, 42), Color.FromArgb(30, 56, 98), 90f))
                g.FillRectangle(sky, rect);

            // Місяць із легким сяйвом
            int moonX = rect.Width - 130;
            int moonY = 48;
            using (var glow = new SolidBrush(Color.FromArgb(38, 220, 238, 255)))
                g.FillEllipse(glow, moonX - 20, moonY - 20, 100, 100);
            using (var moon = new SolidBrush(Color.FromArgb(245, 248, 228)))
                g.FillEllipse(moon, moonX, moonY, 60, 60);

            // Засніжені пагорби на горизонті
            using (var hillFar = new SolidBrush(Color.FromArgb(185, 206, 228)))
                g.FillEllipse(hillFar, -120, rect.Height - 105, rect.Width + 240, 220);
            using (var hillNear = new SolidBrush(Color.FromArgb(232, 242, 252)))
                g.FillEllipse(hillNear, -80, rect.Height - 62, rect.Width + 160, 150);

            // Малювання всіх сніжинок через спільні об'єкти Flyweight
            _simulation.DrawAll(g);

            // Оверлей-індикатор у лівому верхньому куті
            using var hudFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            string hud = $"Сніжинок: {_simulation.SnowflakeCount}  |  Унікальних Flyweight у кеші: {_simulation.CachedFlyweightCount}";
            using var hudBg = new SolidBrush(Color.FromArgb(135, 10, 18, 36));
            var sz = g.MeasureString(hud, hudFont);
            g.FillRectangle(hudBg, 10, 10, sz.Width + 16, sz.Height + 10);
            g.DrawString(hud, hudFont, Brushes.AliceBlue, 18, 15);
        }
    }
}
