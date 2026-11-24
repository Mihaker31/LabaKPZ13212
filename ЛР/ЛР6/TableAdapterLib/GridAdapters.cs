using System.Windows.Forms;

namespace TableAdapterLib
{
    // Спільний допоміжний конвертер між DataGridView та TableData
    internal static class GridConverterHelper
    {
        public static TableData ExtractFromGrid(DataGridView grid)
        {
            grid.EndEdit();
            var data = new TableData();

            foreach (DataGridViewColumn col in grid.Columns)
            {
                data.Headers.Add(col.HeaderText);
            }

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                var cells = new string[grid.Columns.Count];
                for (int c = 0; c < grid.Columns.Count; c++)
                {
                    cells[c] = row.Cells[c].Value?.ToString() ?? string.Empty;
                }
                data.Rows.Add(cells);
            }

            return data;
        }

        public static void PopulateGrid(DataGridView grid, TableData data)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();

            for (int c = 0; c < data.Headers.Count; c++)
            {
                grid.Columns.Add($"col_{c}", data.Headers[c]);
            }

            foreach (var row in data.Rows)
            {
                grid.Rows.Add(row);
            }
        }
    }

    // Адаптер 1: адаптує TextFileSerializer до інтерфейсу ITableStorage для DataGridView
    public class TextGridAdapter : ITableStorage
    {
        private readonly TextFileSerializer _serializer;

        public string FormatName => "Текстовий файл (UTF-8, CSV/TXT)";

        public TextGridAdapter(char delimiter = ';')
        {
            _serializer = new TextFileSerializer(delimiter);
        }

        public void SaveTable(DataGridView grid, string filePath)
        {
            TableData data = GridConverterHelper.ExtractFromGrid(grid);
            _serializer.WriteTextFile(data, filePath);
        }

        public void LoadTable(DataGridView grid, string filePath)
        {
            TableData data = _serializer.ReadTextFile(filePath);
            GridConverterHelper.PopulateGrid(grid, data);
        }
    }

    // Адаптер 2: адаптує BinaryFileSerializer до інтерфейсу ITableStorage для DataGridView
    public class BinaryGridAdapter : ITableStorage
    {
        private readonly BinaryFileSerializer _serializer;

        public string FormatName => "Бінарний файл (BinaryWriter/Reader)";

        public BinaryGridAdapter()
        {
            _serializer = new BinaryFileSerializer();
        }

        public void SaveTable(DataGridView grid, string filePath)
        {
            TableData data = GridConverterHelper.ExtractFromGrid(grid);
            _serializer.WriteBinaryFile(data, filePath);
        }

        public void LoadTable(DataGridView grid, string filePath)
        {
            TableData data = _serializer.ReadBinaryFile(filePath);
            GridConverterHelper.PopulateGrid(grid, data);
        }
    }
}
