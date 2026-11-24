namespace HRSingletonLib.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"#{Id} | {FullName} | {Position} ({Department}) | Оклад: {Salary:N0} грн | Прийнято: {HireDate:dd.MM.yyyy}";
        }
    }
}