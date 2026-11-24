namespace TableAdapterLib
{
    // Внутрішня структура даних, з якою працюють файлові серіалізатори (Adaptee)
    public class TableData
    {
        public List<string> Headers { get; set; } = new List<string>();
        public List<string[]> Rows { get; set; } = new List<string[]>();
    }
}
