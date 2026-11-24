using System.Text;

namespace TableAdapterLib
{
    // Адаптований клас 1 (Adaptee): працює лише з текстовими файлами і TableData, не знає про DataGridView
    public class TextFileSerializer
    {
        private readonly char _delimiter;

        public TextFileSerializer(char delimiter = ';')
        {
            _delimiter = delimiter;
        }

        public void WriteTextFile(TableData data, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(_delimiter, data.Headers.Select(EscapeCell)));
            foreach (var row in data.Rows)
            {
                sb.AppendLine(string.Join(_delimiter, row.Select(EscapeCell)));
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public TableData ReadTextFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Текстовий файл не знайдено.", filePath);

            var lines = File.ReadAllLines(filePath, Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToList();

            var result = new TableData();
            if (lines.Count == 0)
                return result;

            result.Headers = ParseLine(lines[0]);
            for (int i = 1; i < lines.Count; i++)
            {
                var cells = ParseLine(lines[i]);
                while (cells.Count < result.Headers.Count)
                    cells.Add(string.Empty);
                result.Rows.Add(cells.Take(result.Headers.Count).ToArray());
            }

            return result;
        }

        private string EscapeCell(string value)
        {
            value ??= string.Empty;
            if (value.Contains(_delimiter) || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return "\"" + value.Replace("\"", "\"\"").Replace("\r\n", "\\n").Replace("\n", "\\n") + "\"";
            }
            return value;
        }

        private List<string> ParseLine(string line)
        {
            var cells = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == _delimiter)
                    {
                        cells.Add(current.ToString().Replace("\\n", Environment.NewLine));
                        current.Clear();
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
            }
            cells.Add(current.ToString().Replace("\\n", Environment.NewLine));
            return cells;
        }
    }
}
