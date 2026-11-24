using System.Drawing;
using System.Text;
using TableAdapterLib;

namespace TableAdapterWinForms
{
    public partial class Form1 : Form
    {
        private DataGridView dataGridView = null!;
        private TextBox txtFilePreview = null!;
        private Label lblStatus = null!;
        private TextBox txtNewColName = null!;

        private ITableStorage _currentAdapter = new TextGridAdapter();

        public Form1()
        {
            InitializeLayout();
            PopulateSampleData();
        }

        private void InitializeLayout()
        {
            Text = "Лаб 6: Патерн Адаптер — Варіант 14 (DataGridView <-> Текстовий та Бінарний файли)";
            Size = new Size(1060, 720);
            MinimumSize = new Size(920, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5f);

            // Top Toolbar Panel
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 128,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Group 1: Adapter File Operations
            var grpAdapter = new GroupBox
            {
                Text = "Операції через Адаптер (ITableStorage)",
                Location = new Point(10, 8),
                Size = new Size(575, 110),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            var btnSaveTxt = MakeButton("Зберегти в TXT (TextGridAdapter)", 12, 28, 268, 34, Color.FromArgb(42, 122, 226), Color.White);
            btnSaveTxt.Click += BtnSaveTxt_Click;

            var btnLoadTxt = MakeButton("Завантажити з TXT (TextGridAdapter)", 12, 68, 268, 34, Color.FromArgb(228, 238, 252), Color.FromArgb(20, 60, 120));
            btnLoadTxt.Click += BtnLoadTxt_Click;

            var btnSaveBin = MakeButton("Зберегти в BIN (BinaryGridAdapter)", 292, 28, 270, 34, Color.FromArgb(46, 150, 96), Color.White);
            btnSaveBin.Click += BtnSaveBin_Click;

            var btnLoadBin = MakeButton("Завантажити з BIN (BinaryGridAdapter)", 292, 68, 270, 34, Color.FromArgb(225, 245, 234), Color.FromArgb(20, 95, 55));
            btnLoadBin.Click += BtnLoadBin_Click;

            grpAdapter.Controls.AddRange(new Control[] { btnSaveTxt, btnLoadTxt, btnSaveBin, btnLoadBin });

            // Group 2: Table Editing Controls
            var grpTable = new GroupBox
            {
                Text = "Керування таблицею (DataGridView)",
                Location = new Point(595, 8),
                Size = new Size(435, 110),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            var btnSample = MakeButton("Тестові дані", 12, 28, 126, 34, Color.FromArgb(238, 241, 246), Color.Black);
            btnSample.Click += (s, e) => PopulateSampleData();

            var btnClear = MakeButton("Очистити таблицю", 146, 28, 140, 34, Color.FromArgb(250, 232, 232), Color.DarkRed);
            btnClear.Click += (s, e) =>
            {
                dataGridView.Rows.Clear();
                lblStatus.Text = "Таблицю очищено. Спробуйте тепер завантажити дані з TXT або BIN файлу!";
            };

            var btnDelRow = MakeButton("Видалити рядок", 294, 28, 128, 34, Color.FromArgb(238, 241, 246), Color.Black);
            btnDelRow.Click += BtnDelRow_Click;

            txtNewColName = new TextBox
            {
                Location = new Point(12, 72),
                Size = new Size(274, 26),
                PlaceholderText = "Назва нового стовпця...",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            var btnAddCol = MakeButton("+ Додати стовпець", 294, 68, 128, 32, Color.FromArgb(238, 241, 246), Color.Black);
            btnAddCol.Click += BtnAddCol_Click;

            grpTable.Controls.AddRange(new Control[] { btnSample, btnClear, btnDelRow, txtNewColName, btnAddCol });

            topPanel.Controls.AddRange(new Control[] { grpAdapter, grpTable });

            // Split container for DataGridView (top) and File Preview (bottom)
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 330,
                Padding = new Padding(10)
            };

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(245, 249, 255) },
                ColumnHeadersHeight = 32,
                RowTemplate = { Height = 28 }
            };
            split.Panel1.Controls.Add(dataGridView);

            var grpPreview = new GroupBox
            {
                Text = "Попередній перегляд збереженого / відкритого файлу (TXT текст або HEX-дамп BIN)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            txtFilePreview = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9.5f),
                BackColor = Color.FromArgb(28, 32, 38),
                ForeColor = Color.FromArgb(220, 236, 250)
            };
            grpPreview.Controls.Add(txtFilePreview);
            split.Panel2.Controls.Add(grpPreview);

            // Bottom status bar
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.FromArgb(232, 236, 242),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Text = "Готово до роботи. Відредагуйте таблицю та збережіть через текстовий або бінарний адаптер."
            };

            Controls.Add(split);
            Controls.Add(topPanel);
            Controls.Add(lblStatus);
        }

        private Button MakeButton(string text, int x, int y, int w, int h, Color bg, Color fg)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = bg,
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
        }

        private void PopulateSampleData()
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            dataGridView.Columns.Add("colId", "Код");
            dataGridView.Columns.Add("colModel", "Модель телефону");
            dataGridView.Columns.Add("colBrand", "Бренд");
            dataGridView.Columns.Add("colRam", "ОЗП (ГБ)");
            dataGridView.Columns.Add("colPrice", "Ціна (грн)");

            dataGridView.Rows.Add("101", "Galaxy S24 Ultra", "Samsung", "12", "49999");
            dataGridView.Rows.Add("102", "iPhone 16 Pro", "Apple", "8", "54999");
            dataGridView.Rows.Add("103", "Pixel 9 Pro", "Google", "16", "44500");
            dataGridView.Rows.Add("104", "Xiaomi 14", "Xiaomi", "12", "34999");

            lblStatus.Text = "Завантажено демонстраційні дані (4 рядки, 5 стовпців).";
        }

        private void BtnAddCol_Click(object? sender, EventArgs e)
        {
            string name = txtNewColName.Text.Trim();
            if (string.IsNullOrEmpty(name))
                name = $"Стовпець {dataGridView.Columns.Count + 1}";

            dataGridView.Columns.Add($"col_custom_{dataGridView.Columns.Count}", name);
            txtNewColName.Clear();
            lblStatus.Text = $"Додано стовпець '{name}'.";
        }

        private void BtnDelRow_Click(object? sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null && !dataGridView.CurrentRow.IsNewRow)
            {
                dataGridView.Rows.Remove(dataGridView.CurrentRow);
                lblStatus.Text = "Обраний рядок видалено.";
            }
        }

        private void BtnSaveTxt_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "Текстові файли (*.txt;*.csv)|*.txt;*.csv",
                FileName = "table_data.txt"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _currentAdapter = new TextGridAdapter(';');
                _currentAdapter.SaveTable(dataGridView, dlg.FileName);
                ShowTextPreview(dlg.FileName);
                lblStatus.Text = $"Успішно збережено через TextGridAdapter -> {dlg.FileName} ({new FileInfo(dlg.FileName).Length} байт)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка збереження", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoadTxt_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Текстові файли (*.txt;*.csv)|*.txt;*.csv"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _currentAdapter = new TextGridAdapter(';');
                _currentAdapter.LoadTable(dataGridView, dlg.FileName);
                ShowTextPreview(dlg.FileName);
                lblStatus.Text = $"Успішно завантажено з TXT через TextGridAdapter <- {dlg.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка читання", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveBin_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "Бінарні файли (*.bin)|*.bin",
                FileName = "table_data.bin"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _currentAdapter = new BinaryGridAdapter();
                _currentAdapter.SaveTable(dataGridView, dlg.FileName);
                ShowBinaryHexPreview(dlg.FileName);
                lblStatus.Text = $"Успішно збережено через BinaryGridAdapter -> {dlg.FileName} ({new FileInfo(dlg.FileName).Length} байт)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка збереження", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoadBin_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Бінарні файли (*.bin)|*.bin"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _currentAdapter = new BinaryGridAdapter();
                _currentAdapter.LoadTable(dataGridView, dlg.FileName);
                ShowBinaryHexPreview(dlg.FileName);
                lblStatus.Text = $"Успішно завантажено з BIN через BinaryGridAdapter <- {dlg.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка читання", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTextPreview(string path)
        {
            string raw = File.ReadAllText(path, Encoding.UTF8);
            txtFilePreview.Text = $"[ТЕКСТОВИЙ ФАЙЛ: {Path.GetFileName(path)} | {new FileInfo(path).Length} байт]\r\n" +
                                  "------------------------------------------------------------\r\n" + raw;
        }

        private void ShowBinaryHexPreview(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var sb = new StringBuilder();
            sb.AppendLine($"[БІНАРНИЙ ФАЙЛ (HEX-ДАМП): {Path.GetFileName(path)} | {bytes.Length} байт]");
            sb.AppendLine("------------------------------------------------------------");

            int maxBytes = Math.Min(bytes.Length, 512);
            for (int i = 0; i < maxBytes; i += 16)
            {
                sb.Append($"{i:X4}: ");
                for (int j = 0; j < 16; j++)
                {
                    if (i + j < maxBytes)
                        sb.Append($"{bytes[i + j]:X2} ");
                    else
                        sb.Append("   ");
                }
                sb.Append(" | ");
                for (int j = 0; j < 16 && i + j < maxBytes; j++)
                {
                    byte b = bytes[i + j];
                    sb.Append(b >= 32 && b < 127 ? (char)b : '.');
                }
                sb.AppendLine();
            }

            if (bytes.Length > maxBytes)
                sb.AppendLine($"... (показано перші {maxBytes} із {bytes.Length} байт)");

            txtFilePreview.Text = sb.ToString();
        }
    }
}
