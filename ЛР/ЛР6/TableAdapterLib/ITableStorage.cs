using System.Windows.Forms;

namespace TableAdapterLib
{
    // Цільовий інтерфейс (Target), з яким працює клієнт (Form1)
    public interface ITableStorage
    {
        string FormatName { get; }
        void SaveTable(DataGridView grid, string filePath);
        void LoadTable(DataGridView grid, string filePath);
    }
}
