using System.Text;

namespace TableAdapterLib
{
    // Адаптований клас 2 (Adaptee): працює лише з бінарним потоком і TableData, не знає про DataGridView
    public class BinaryFileSerializer
    {
        private const string MagicHeader = "TBLBIN01";

        public void WriteBinaryFile(TableData data, string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(fs, Encoding.UTF8);

            writer.Write(MagicHeader);
            writer.Write(data.Headers.Count);
            foreach (var header in data.Headers)
            {
                writer.Write(header ?? string.Empty);
            }

            writer.Write(data.Rows.Count);
            foreach (var row in data.Rows)
            {
                for (int c = 0; c < data.Headers.Count; c++)
                {
                    string val = c < row.Length ? (row[c] ?? string.Empty) : string.Empty;
                    writer.Write(val);
                }
            }
        }

        public TableData ReadBinaryFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Бінарний файл не знайдено.", filePath);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fs, Encoding.UTF8);

            string magic = reader.ReadString();
            if (magic != MagicHeader)
                throw new InvalidDataException("Невірний формат бінарного файлу таблиці.");

            var data = new TableData();
            int colCount = reader.ReadInt32();
            for (int c = 0; c < colCount; c++)
            {
                data.Headers.Add(reader.ReadString());
            }

            int rowCount = reader.ReadInt32();
            for (int r = 0; r < rowCount; r++)
            {
                var row = new string[colCount];
                for (int c = 0; c < colCount; c++)
                {
                    row[c] = reader.ReadString();
                }
                data.Rows.Add(row);
            }

            return data;
        }
    }
}
