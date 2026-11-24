using System.Drawing;
using WordGameProxyLib;

namespace WordGameWinForms
{
    public partial class Form1 : Form
    {
        private readonly WordGameProxy _proxy = new WordGameProxy();
        private readonly Dictionary<char, Button> _letterButtons = new Dictionary<char, Button>();

        // Game UI
        private ComboBox cmbCategory = null!;
        private ComboBox cmbRole = null!;
        private Button btnNewGame = null!;
        private Label lblWordDisplay = null!;
        private Label lblHintDisplay = null!;
        private Label lblFeedback = null!;
        private Label lblAttempts = null!;
        private ProgressBar prgAttempts = null!;
        private TextBox txtWholeWord = null!;
        private Button btnGuessWord = null!;
        private Button btnHint = null!;
        private Button btnReveal = null!;

        // Proxy Monitor UI
        private Label lblLazyStatus = null!;
        private Label lblProxyCounters = null!;
        private ListBox lstProxyLog = null!;

        private const string UkrainianAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";

        public Form1()
        {
            InitializeLayout();
            RefreshUI();
        }

        private void InitializeLayout()
        {
            Text = "Лаб 9: Патерн Proxy (Замісник) — Варіант 14: Гра «Вгадай слово»";
            Size = new Size(1220, 760);
            MinimumSize = new Size(1060, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(244, 246, 250);
            Font = new Font("Segoe UI", 9.5f);

            // Right Panel: Live Proxy Monitor
            var rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 450,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(24, 29, 38)
            };

            var lblMonitorTitle = new Label
            {
                Text = "МОНІТОР PROXY (ПЕРЕХОПЛЕННЯ ВИКЛИКІВ)",
                Location = new Point(12, 12),
                Size = new Size(424, 24),
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(130, 210, 255)
            };

            lblLazyStatus = new Label
            {
                Text = "RealWordGameService: НЕ СТВОРЕНО (Lazy Init)",
                Location = new Point(12, 42),
                Size = new Size(424, 24),
                Font = new Font("Consolas", 9.5f, FontStyle.Bold),
                ForeColor = Color.Gold
            };

            lblProxyCounters = new Label
            {
                Text = "Усього: 0 | До RealSubject: 0 | Кеш/Блок Proxy: 0",
                Location = new Point(12, 70),
                Size = new Size(424, 24),
                Font = new Font("Consolas", 9.2f),
                ForeColor = Color.LightGray
            };

            lstProxyLog = new ListBox
            {
                Location = new Point(12, 102),
                Size = new Size(424, 560),
                BackColor = Color.FromArgb(16, 20, 26),
                ForeColor = Color.FromArgb(215, 232, 246),
                Font = new Font("Consolas", 8.5f),
                BorderStyle = BorderStyle.FixedSingle,
                HorizontalScrollbar = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            var btnClearLog = new Button
            {
                Text = "Очистити журнал Proxy",
                Location = new Point(12, 672),
                Size = new Size(424, 32),
                BackColor = Color.FromArgb(46, 56, 72),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            btnClearLog.Click += (s, e) =>
            {
                _proxy.ClearLog();
                RefreshProxyLog();
            };

            rightPanel.Controls.AddRange(new Control[]
            {
                lblMonitorTitle, lblLazyStatus, lblProxyCounters, lstProxyLog, btnClearLog
            });

            // Left Panel: Game Interface
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14)
            };

            // Top settings bar
            var grpTop = new GroupBox
            {
                Text = "Налаштування гри та ролі доступу (Protection Proxy)",
                Location = new Point(14, 10),
                Size = new Size(718, 86),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            var lblCat = new Label
            {
                Text = "Категорія:",
                Location = new Point(12, 26),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cmbCategory = new ComboBox
            {
                Location = new Point(12, 46),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };
            cmbCategory.Items.AddRange(RealWordGameService.AvailableCategories);
            cmbCategory.SelectedIndex = 0;

            var lblRole = new Label
            {
                Text = "Роль користувача (Proxy):",
                Location = new Point(208, 26),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cmbRole = new ComboBox
            {
                Location = new Point(208, 46),
                Width = 225,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };
            cmbRole.Items.AddRange(new object[]
            {
                "Гість (1 підказка, без підглядання)",
                "VIP-гравець (3 підказки)",
                "Адмін (повний доступ + розкриття)"
            });
            cmbRole.SelectedIndex = 0;
            cmbRole.SelectedIndexChanged += (s, e) =>
            {
                _proxy.CurrentRole = cmbRole.SelectedIndex switch
                {
                    1 => UserRole.VipPlayer,
                    2 => UserRole.Admin,
                    _ => UserRole.Guest
                };
            };

            btnNewGame = new Button
            {
                Text = "Почати нову гру",
                Location = new Point(450, 38),
                Size = new Size(254, 36),
                BackColor = Color.FromArgb(42, 122, 226),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            btnNewGame.Click += BtnNewGame_Click;

            grpTop.Controls.AddRange(new Control[] { lblCat, cmbCategory, lblRole, cmbRole, btnNewGame });

            // Word Display Card
            var grpWord = new GroupBox
            {
                Text = "Загадане слово",
                Location = new Point(14, 104),
                Size = new Size(718, 200),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White
            };

            lblWordDisplay = new Label
            {
                Text = "Натисніть «Почати нову гру»",
                Location = new Point(14, 28),
                Size = new Size(690, 58),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 48, 92),
                BackColor = Color.FromArgb(238, 244, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblAttempts = new Label
            {
                Text = "Спроби: 6 / 6",
                Location = new Point(14, 96),
                Size = new Size(160, 22),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            prgAttempts = new ProgressBar
            {
                Location = new Point(180, 96),
                Size = new Size(524, 20),
                Minimum = 0,
                Maximum = 6,
                Value = 6
            };

            lblHintDisplay = new Label
            {
                Text = "Підказка: натисніть кнопку «Підказка» (повторний клік поверне результат із кешу Proxy)",
                Location = new Point(14, 124),
                Size = new Size(690, 26),
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(70, 80, 100)
            };

            lblFeedback = new Label
            {
                Text = "Очікування запуску гри. Зверніть увагу справа: RealWordGameService ще не створено!",
                Location = new Point(14, 156),
                Size = new Size(690, 32),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 140)
            };

            grpWord.Controls.AddRange(new Control[]
            {
                lblWordDisplay, lblAttempts, prgAttempts, lblHintDisplay, lblFeedback
            });

            // Actions Bar (Whole word + Hint + Admin Reveal)
            var grpActions = new GroupBox
            {
                Text = "Дії гравця (проходять через WordGameProxy)",
                Location = new Point(14, 312),
                Size = new Size(718, 92),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            txtWholeWord = new TextBox
            {
                Location = new Point(14, 34),
                Size = new Size(215, 32),
                Font = new Font("Segoe UI", 11f),
                PlaceholderText = "Введіть усе слово..."
            };

            btnGuessWord = new Button
            {
                Text = "Вгадати слово",
                Location = new Point(236, 32),
                Size = new Size(138, 34),
                BackColor = Color.FromArgb(46, 150, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold)
            };
            btnGuessWord.Click += BtnGuessWord_Click;

            btnHint = new Button
            {
                Text = "Отримати підказку",
                Location = new Point(384, 32),
                Size = new Size(156, 34),
                BackColor = Color.FromArgb(236, 168, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold)
            };
            btnHint.Click += BtnHint_Click;

            btnReveal = new Button
            {
                Text = "Розкрити (Admin)",
                Location = new Point(550, 32),
                Size = new Size(154, 34),
                BackColor = Color.FromArgb(182, 64, 82),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold)
            };
            btnReveal.Click += BtnReveal_Click;

            grpActions.Controls.AddRange(new Control[] { txtWholeWord, btnGuessWord, btnHint, btnReveal });

            // Keyboard Panel (Ukrainian Letters)
            var grpKeyboard = new GroupBox
            {
                Text = "Екранна клавіатура (повторний клік по літері демонструє Кешуючий Proxy)",
                Location = new Point(14, 412),
                Size = new Size(718, 286),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White
            };

            var flow = new FlowLayoutPanel
            {
                Location = new Point(12, 26),
                Size = new Size(694, 248),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(4)
            };

            foreach (char ch in UkrainianAlphabet)
            {
                var btn = new Button
                {
                    Text = ch.ToString(),
                    Size = new Size(56, 46),
                    Margin = new Padding(4),
                    Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                    BackColor = Color.FromArgb(236, 241, 248),
                    FlatStyle = FlatStyle.Flat,
                    Tag = ch
                };
                btn.Click += LetterButton_Click;
                _letterButtons[ch] = btn;
                flow.Controls.Add(btn);
            }

            grpKeyboard.Controls.Add(flow);

            leftPanel.Controls.AddRange(new Control[] { grpTop, grpWord, grpActions, grpKeyboard });

            Controls.Add(leftPanel);
            Controls.Add(rightPanel);
        }

        private void BtnNewGame_Click(object? sender, EventArgs e)
        {
            string category = cmbCategory.SelectedItem?.ToString() ?? "Програмування";
            _proxy.StartNewGame(category);

            // Скидаємо кольори кнопок клавіатури
            foreach (var btn in _letterButtons.Values)
            {
                btn.BackColor = Color.FromArgb(236, 241, 248);
                btn.ForeColor = Color.Black;
            }

            txtWholeWord.Clear();
            lblHintDisplay.Text = "Підказка: натисніть «Отримати підказку»";
            lblFeedback.ForeColor = Color.FromArgb(30, 80, 140);
            lblFeedback.Text = $"Нову гру розпочато! Категорія: «{_proxy.Category}». Оберіть літеру.";
            RefreshUI();
        }

        private void LetterButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not char ch)
                return;

            // Кнопки навмисно залишаються активними, щоб можна було повторно натиснути
            // ту саму літеру і побачити миттєве спрацювання кешу Proxy!
            var result = _proxy.GuessLetter(ch);

            if (!result.ServedFromProxy)
            {
                btn.BackColor = result.IsSuccess ? Color.FromArgb(182, 238, 198) : Color.FromArgb(248, 198, 198);
                btn.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.DarkRed;
            }

            lblFeedback.ForeColor = result.ServedFromProxy
                ? Color.DarkOrange
                : (result.IsSuccess ? Color.DarkGreen : Color.Crimson);
            lblFeedback.Text = result.Message;

            RefreshUI();
        }

        private void BtnGuessWord_Click(object? sender, EventArgs e)
        {
            var result = _proxy.GuessWord(txtWholeWord.Text);
            lblFeedback.ForeColor = result.ServedFromProxy
                ? Color.DarkOrange
                : (result.IsSuccess ? Color.DarkGreen : Color.Crimson);
            lblFeedback.Text = result.Message;
            txtWholeWord.Clear();
            RefreshUI();
        }

        private void BtnHint_Click(object? sender, EventArgs e)
        {
            string hint = _proxy.GetHint();
            lblHintDisplay.Text = $"Підказка: {hint}";
            RefreshUI();
        }

        private void BtnReveal_Click(object? sender, EventArgs e)
        {
            string response = _proxy.RevealSecretWord();
            lblFeedback.ForeColor = response.StartsWith("ДОСТУП") ? Color.Crimson : Color.DarkMagenta;
            lblFeedback.Text = response;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (_proxy.IsRealServiceInitialized)
            {
                lblWordDisplay.Text = _proxy.MaskedWord;
                lblLazyStatus.Text = "RealWordGameService: ІНІЦІАЛІЗОВАНО (Активний)";
                lblLazyStatus.ForeColor = Color.LightGreen;
            }
            else
            {
                lblWordDisplay.Text = "Натисніть «Почати нову гру»";
                lblLazyStatus.Text = "RealWordGameService: НЕ СТВОРЕНО (Lazy Init)";
                lblLazyStatus.ForeColor = Color.Gold;
            }

            lblAttempts.Text = $"Спроби: {_proxy.AttemptsLeft} / {_proxy.MaxAttempts}";
            lblAttempts.ForeColor = _proxy.AttemptsLeft <= 2 ? Color.Crimson : Color.DarkGreen;
            prgAttempts.Maximum = _proxy.MaxAttempts;
            prgAttempts.Value = Math.Clamp(_proxy.AttemptsLeft, 0, _proxy.MaxAttempts);

            lblProxyCounters.Text = $"Усього: {_proxy.TotalCalls} | RealSubject: {_proxy.ForwardedCalls} | Proxy кеш/блок: {_proxy.CachedOrBlockedCalls}";
            RefreshProxyLog();
        }

        private void RefreshProxyLog()
        {
            lstProxyLog.BeginUpdate();
            lstProxyLog.Items.Clear();
            foreach (var entry in _proxy.ActionLog)
                lstProxyLog.Items.Add(entry);
            if (lstProxyLog.Items.Count > 0)
                lstProxyLog.TopIndex = lstProxyLog.Items.Count - 1;
            lstProxyLog.EndUpdate();
        }
    }
}
