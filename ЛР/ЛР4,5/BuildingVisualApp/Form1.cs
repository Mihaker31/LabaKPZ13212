using System.Drawing;
using System.Drawing.Drawing2D;
using BuildingPatternsLib;

namespace BuildingVisualApp
{
    public partial class Form1 : Form
    {
        private readonly ConstructionDirector _director = new ConstructionDirector();
        private IBuildingBuilder _currentBuilder;
        private MultiStoryBuilding? _activeBuilding;
        private int _currentStep = 0;

        private readonly List<MultiStoryBuilding> _districtBuildings = new List<MultiStoryBuilding>();
        private readonly System.Windows.Forms.Timer _stepTimer = new System.Windows.Forms.Timer();

        // UI Controls
        private ComboBox cmbClass = null!;
        private Button btnStartNew = null!;
        private Button btnNextStep = null!;
        private Button btnAutoBuild = null!;
        private Button btnBuildInstant = null!;
        private ProgressBar progressSteps = null!;
        private Label lblStepStatus = null!;

        private ListBox lstDistrict = null!;
        private NumericUpDown numCloneCount = null!;
        private Button btnClone = null!;
        private Button btnCustomizeClone = null!;
        private Button btnClearDistrict = null!;
        private Label lblDistrictStats = null!;

        private TextBox txtSpec = null!;
        private CanvasPanel pnlConstruction = null!;
        private CanvasPanel pnlDistrict = null!;

        public Form1()
        {
            _currentBuilder = new EconomyPanelBuilder();
            InitializeLayout();

            _stepTimer.Interval = 700;
            _stepTimer.Tick += StepTimer_Tick;

            StartNewConstruction();
        }

        private void InitializeLayout()
        {
            Text = "Лаб 4 (Будівельник) + Лаб 5 (Прототип) — Варіант 14: Багатоповерхові будинки";
            Size = new Size(1240, 780);
            MinimumSize = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(244, 246, 250);
            Font = new Font("Segoe UI", 9.5f);

            // Left control panel
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 370,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Group 1: Lab 4 Builder
            var grpBuilder = new GroupBox
            {
                Text = "Лаб 4: Будівельник (покрокове зведення)",
                Location = new Point(10, 10),
                Size = new Size(348, 225),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            var lblCfg = new Label
            {
                Text = "Конфігурація будинку:",
                Location = new Point(12, 26),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            cmbClass = new ComboBox
            {
                Location = new Point(12, 48),
                Width = 324,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };
            cmbClass.Items.AddRange(new object[]
            {
                "Економ-клас (панельний, 9 пов.)",
                "Комфорт-клас (цегляний, 16 пов.)",
                "Преміум-клас (моноліт, 25 пов.)"
            });
            cmbClass.SelectedIndex = 0;
            cmbClass.SelectedIndexChanged += (s, e) => StartNewConstruction();

            btnStartNew = new Button
            {
                Text = "Новий майданчик",
                Location = new Point(12, 84),
                Size = new Size(156, 32),
                Font = new Font("Segoe UI", 9f),
                BackColor = Color.FromArgb(236, 240, 245)
            };
            btnStartNew.Click += (s, e) => StartNewConstruction();

            btnNextStep = new Button
            {
                Text = "Наступний крок >>",
                Location = new Point(178, 84),
                Size = new Size(158, 32),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Color.FromArgb(42, 122, 226),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnNextStep.Click += (s, e) => ExecuteNextStep();

            btnAutoBuild = new Button
            {
                Text = "Анімація кроків",
                Location = new Point(12, 122),
                Size = new Size(156, 32),
                Font = new Font("Segoe UI", 9f),
                BackColor = Color.FromArgb(46, 154, 98),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAutoBuild.Click += BtnAutoBuild_Click;

            btnBuildInstant = new Button
            {
                Text = "Збудувати одразу",
                Location = new Point(178, 122),
                Size = new Size(158, 32),
                Font = new Font("Segoe UI", 9f),
                BackColor = Color.FromArgb(236, 240, 245)
            };
            btnBuildInstant.Click += BtnBuildInstant_Click;

            progressSteps = new ProgressBar
            {
                Location = new Point(12, 164),
                Size = new Size(324, 18),
                Minimum = 0,
                Maximum = 6
            };

            lblStepStatus = new Label
            {
                Text = "Готово до початку будівництва (0/6)",
                Location = new Point(12, 188),
                Size = new Size(324, 28),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(40, 60, 90)
            };

            grpBuilder.Controls.AddRange(new Control[]
            {
                lblCfg, cmbClass, btnStartNew, btnNextStep, btnAutoBuild, btnBuildInstant, progressSteps, lblStepStatus
            });

            // Group 2: Lab 5 Prototype
            var grpProto = new GroupBox
            {
                Text = "Лаб 5: Прототип (клонування об'єктів)",
                Location = new Point(10, 242),
                Size = new Size(348, 255),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            lblDistrictStats = new Label
            {
                Text = "Об'єктів у районі: 0 (оригіналів: 0, клонів: 0)",
                Location = new Point(12, 24),
                Size = new Size(324, 20),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.DimGray
            };

            lstDistrict = new ListBox
            {
                Location = new Point(12, 46),
                Size = new Size(324, 118),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            lstDistrict.SelectedIndexChanged += LstDistrict_SelectedIndexChanged;

            var lblQty = new Label
            {
                Text = "Копій:",
                Location = new Point(12, 174),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            numCloneCount = new NumericUpDown
            {
                Location = new Point(62, 170),
                Width = 60,
                Minimum = 1,
                Maximum = 20,
                Value = 3,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            btnClone = new Button
            {
                Text = "Клонувати обраний (Prototype)",
                Location = new Point(130, 168),
                Size = new Size(206, 30),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Color.FromArgb(128, 70, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClone.Click += BtnClone_Click;

            btnCustomizeClone = new Button
            {
                Text = "Змінити колір обраної копії",
                Location = new Point(12, 208),
                Size = new Size(200, 32),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                BackColor = Color.FromArgb(236, 240, 245)
            };
            btnCustomizeClone.Click += BtnCustomizeClone_Click;

            btnClearDistrict = new Button
            {
                Text = "Очистити",
                Location = new Point(220, 208),
                Size = new Size(116, 32),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                BackColor = Color.FromArgb(248, 232, 232)
            };
            btnClearDistrict.Click += BtnClearDistrict_Click;

            grpProto.Controls.AddRange(new Control[]
            {
                lblDistrictStats, lstDistrict, lblQty, numCloneCount, btnClone, btnCustomizeClone, btnClearDistrict
            });

            // Group 3: Specification
            var grpSpec = new GroupBox
            {
                Text = "Технічний паспорт обраного будинку",
                Location = new Point(10, 505),
                Size = new Size(348, 220),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            txtSpec = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 24),
                Size = new Size(328, 186),
                Font = new Font("Consolas", 8.8f),
                BackColor = Color.FromArgb(250, 251, 253),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            grpSpec.Controls.Add(txtSpec);

            leftPanel.Controls.AddRange(new Control[] { grpBuilder, grpProto, grpSpec });

            // Right visualization area (Split top: Builder Site, bottom: Prototype District)
            var rightSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 365,
                SplitterWidth = 6,
                Padding = new Padding(8)
            };

            pnlConstruction = new CanvasPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(228, 242, 255),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlConstruction.Paint += PnlConstruction_Paint;

            pnlDistrict = new CanvasPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(236, 246, 238),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlDistrict.Paint += PnlDistrict_Paint;
            pnlDistrict.MouseClick += PnlDistrict_MouseClick;

            rightSplit.Panel1.Controls.Add(pnlConstruction);
            rightSplit.Panel2.Controls.Add(pnlDistrict);

            Controls.Add(rightSplit);
            Controls.Add(leftPanel);
        }

        private IBuildingBuilder CreateSelectedBuilder()
        {
            return cmbClass.SelectedIndex switch
            {
                1 => new ComfortBrickBuilder(),
                2 => new PremiumMonolithBuilder(),
                _ => new EconomyPanelBuilder()
            };
        }

        private void StartNewConstruction()
        {
            _stepTimer.Stop();
            _currentBuilder = CreateSelectedBuilder();
            _director.SetBuilder(_currentBuilder);
            _director.StartNew();
            _activeBuilding = _currentBuilder.GetBuilding();
            _currentStep = 0;

            progressSteps.Value = 0;
            lblStepStatus.Text = $"Майданчик готовий ({_currentBuilder.BuilderTitle}). Натисніть 'Наступний крок'.";
            txtSpec.Text = _activeBuilding.GetSpecification();
            pnlConstruction.Invalidate();
        }

        private void ExecuteNextStep()
        {
            if (_currentStep >= 6)
            {
                _stepTimer.Stop();
                return;
            }

            _currentStep++;
            string status = _director.ExecuteStep(_currentStep);
            _activeBuilding = _currentBuilder.GetBuilding();

            progressSteps.Value = _currentStep;
            lblStepStatus.Text = status;
            txtSpec.Text = _activeBuilding.GetSpecification();

            if (_currentStep == 6)
            {
                _stepTimer.Stop();
                AddBuildingToDistrict(_activeBuilding);
            }

            pnlConstruction.Invalidate();
        }

        private void BtnAutoBuild_Click(object? sender, EventArgs e)
        {
            if (_currentStep >= 6)
                StartNewConstruction();

            _stepTimer.Start();
        }

        private void StepTimer_Tick(object? sender, EventArgs e)
        {
            ExecuteNextStep();
        }

        private void BtnBuildInstant_Click(object? sender, EventArgs e)
        {
            _stepTimer.Stop();
            _currentBuilder = CreateSelectedBuilder();
            _director.SetBuilder(_currentBuilder);
            _director.ConstructFullBuilding();
            _activeBuilding = _currentBuilder.GetBuilding();
            _currentStep = 6;

            progressSteps.Value = 6;
            lblStepStatus.Text = "Будинок повністю зведено Директором (6/6) і додано до району.";
            txtSpec.Text = _activeBuilding.GetSpecification();

            AddBuildingToDistrict(_activeBuilding);
            pnlConstruction.Invalidate();
        }

        private void AddBuildingToDistrict(MultiStoryBuilding building)
        {
            if (!_districtBuildings.Contains(building))
            {
                _districtBuildings.Add(building);
                RefreshDistrictUI(selectLast: true);
            }
        }

        private void BtnClone_Click(object? sender, EventArgs e)
        {
            MultiStoryBuilding? source = lstDistrict.SelectedItem as MultiStoryBuilding ?? _activeBuilding;
            if (source == null || source.CompletedSteps == 0)
            {
                MessageBox.Show("Спочатку збудуйте будинок (або виконайте хоча б 1 крок), щоб створити його копії.",
                    "Прототип", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_districtBuildings.Contains(source))
                _districtBuildings.Add(source);

            int copies = (int)numCloneCount.Value;
            for (int i = 0; i < copies; i++)
            {
                // Виклик патерну Прототип — глибоке клонування без повторного виклику Будівельника
                MultiStoryBuilding clone = source.Clone();
                _districtBuildings.Add(clone);
            }

            RefreshDistrictUI(selectLast: true);
        }

        private void BtnCustomizeClone_Click(object? sender, EventArgs e)
        {
            if (lstDistrict.SelectedItem is not MultiStoryBuilding selected)
            {
                MessageBox.Show("Оберіть будинок зі списку району.", "Прототип", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Зміна кольору фасаду демонструє незалежність глибокої копії від оригіналу
            string[] palette = { "#E0BBE4", "#95B8D1", "#B8E0D2", "#F4C2C2", "#E6D5A9", "#A2C4C9" };
            selected.WallColorHex = palette[Random.Shared.Next(palette.Length)];
            selected.Name += " *";

            RefreshDistrictUI(selectLast: false);
            txtSpec.Text = selected.GetSpecification();
        }

        private void BtnClearDistrict_Click(object? sender, EventArgs e)
        {
            _districtBuildings.Clear();
            RefreshDistrictUI(selectLast: false);
        }

        private void RefreshDistrictUI(bool selectLast)
        {
            int prevIdx = lstDistrict.SelectedIndex;
            lstDistrict.Items.Clear();
            foreach (var b in _districtBuildings)
                lstDistrict.Items.Add(b);

            if (_districtBuildings.Count > 0)
            {
                lstDistrict.SelectedIndex = selectLast
                    ? _districtBuildings.Count - 1
                    : Math.Clamp(prevIdx, 0, _districtBuildings.Count - 1);
            }

            int clones = _districtBuildings.Count(b => b.IsClone);
            int originals = _districtBuildings.Count - clones;
            lblDistrictStats.Text = $"Об'єктів у районі: {_districtBuildings.Count} (оригіналів: {originals}, клонів: {clones})";
            pnlDistrict.Invalidate();
        }

        private void LstDistrict_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstDistrict.SelectedItem is MultiStoryBuilding b)
            {
                txtSpec.Text = b.GetSpecification();
                pnlDistrict.Invalidate();
            }
        }

        private void PnlConstruction_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = pnlConstruction.ClientRectangle;

            // Фон неба і землі
            using (var sky = new LinearGradientBrush(rect, Color.FromArgb(182, 220, 252), Color.FromArgb(236, 247, 255), 90f))
                g.FillRectangle(sky, rect);

            int groundY = rect.Bottom - 36;
            using (var grass = new SolidBrush(Color.FromArgb(124, 178, 108)))
                g.FillRectangle(grass, 0, groundY, rect.Width, 36);
            using (var soil = new SolidBrush(Color.FromArgb(139, 108, 78)))
                g.FillRectangle(soil, 0, groundY + 6, rect.Width, 30);

            // Заголовок області
            using var hdrFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            g.DrawString("ЛАБ 4: БУДІВЕЛЬНИЙ МАЙДАНЧИК (ПОКРОКОВИЙ BUILDER)", hdrFont, Brushes.MidnightBlue, 12, 10);

            // Схематичний будівельний кран, поки будинок будується
            if (_currentStep > 0 && _currentStep < 6)
            {
                using var cranePen = new Pen(Color.DarkOrange, 3f);
                int craneX = rect.Width - 90;
                g.DrawLine(cranePen, craneX, groundY, craneX, 42);
                g.DrawLine(cranePen, craneX + 18, 52, rect.Width / 2 - 20, 52);
                g.DrawLine(Pens.DimGray, rect.Width / 2 + 10, 52, rect.Width / 2 + 10, 95);
            }

            // Візуалізація активного будинку по центру
            var buildingBounds = new Rectangle(rect.Width / 2 - 110, 35, 220, rect.Height - 35);
            BuildingRenderer.DrawSingleBuilding(g, _activeBuilding, buildingBounds, isSelected: false, showLabels: false);

            // Чек-лист кроків ліворуч на полотні
            string[] stepNames =
            {
                "1. Фундамент",
                "2. Каркас і поверхи",
                "3. Покрівля",
                "4. Ліфти",
                "5. Комунікації",
                "6. Паркінг"
            };
            using var stepFont = new Font("Segoe UI", 9f, FontStyle.Regular);
            using var doneFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            for (int i = 0; i < stepNames.Length; i++)
            {
                bool done = i < _currentStep;
                string mark = done ? "[✓] " : "[ ] ";
                g.DrawString(mark + stepNames[i], done ? doneFont : stepFont,
                    done ? Brushes.DarkGreen : Brushes.SlateGray, 14, 42 + i * 22);
            }
        }

        private void PnlDistrict_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = pnlDistrict.ClientRectangle;

            using (var sky = new LinearGradientBrush(rect, Color.FromArgb(215, 236, 248), Color.FromArgb(245, 250, 244), 90f))
                g.FillRectangle(sky, rect);

            int groundY = rect.Bottom - 36;
            using (var grass = new SolidBrush(Color.FromArgb(132, 184, 118)))
                g.FillRectangle(grass, 0, groundY, rect.Width, 36);

            using var hdrFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            g.DrawString("ЛАБ 5: ЗАБУДОВА МІКРОРАЙОНУ (КЛОНУВАННЯ PROTOTYPE) — клікніть по будинку для вибору",
                hdrFont, Brushes.DarkSlateBlue, 12, 10);

            if (_districtBuildings.Count == 0)
            {
                using var hintFont = new Font("Segoe UI", 10f, FontStyle.Italic);
                g.DrawString("Завершіть будівництво будинку зверху, а потім натисніть 'Клонувати обраний' зліва.",
                    hintFont, Brushes.DimGray, 20, rect.Height / 2 - 10);
                return;
            }

            int count = _districtBuildings.Count;
            int slotW = Math.Clamp((rect.Width - 24) / count, 88, 165);
            int maxVisible = Math.Max(1, (rect.Width - 24) / slotW);
            int startIndex = Math.Max(0, count - maxVisible);

            MultiStoryBuilding? selected = lstDistrict.SelectedItem as MultiStoryBuilding;

            for (int i = startIndex; i < count; i++)
            {
                int slotIdx = i - startIndex;
                var slotRect = new Rectangle(12 + slotIdx * slotW, 34, slotW - 4, rect.Height - 36);
                BuildingRenderer.DrawSingleBuilding(g, _districtBuildings[i], slotRect,
                    isSelected: ReferenceEquals(_districtBuildings[i], selected), showLabels: true);
            }
        }

        private void PnlDistrict_MouseClick(object? sender, MouseEventArgs e)
        {
            if (_districtBuildings.Count == 0) return;

            var rect = pnlDistrict.ClientRectangle;
            int count = _districtBuildings.Count;
            int slotW = Math.Clamp((rect.Width - 24) / count, 88, 165);
            int maxVisible = Math.Max(1, (rect.Width - 24) / slotW);
            int startIndex = Math.Max(0, count - maxVisible);

            int clickedSlot = (e.X - 12) / slotW;
            int idx = startIndex + clickedSlot;
            if (idx >= 0 && idx < _districtBuildings.Count)
            {
                lstDistrict.SelectedIndex = idx;
            }
        }
    }
}
