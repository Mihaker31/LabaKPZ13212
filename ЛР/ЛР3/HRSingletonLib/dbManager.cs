using HRSingletonLib.Models;

namespace HRSingletonLib
{
    // Завдання 1: Глобальна точка доступу до бази даних (Singleton)
    public sealed class dbManager
    {
        private static dbManager? _instance;
        private static readonly object _lock = new object();

        private string _connectionString;
        private bool _isConnected;
        private readonly List<Employee> _employees;
        private int _nextId;

        public Guid InstanceId { get; }

        private dbManager()
        {
            InstanceId = Guid.NewGuid();
            _connectionString = "Server=localhost;Database=HR_Department_DB;Trusted_Connection=True;";
            _isConnected = false;
            _employees = new List<Employee>
            {
                new Employee { Id = 1, FullName = "Коваленко Олександр Іванович", Position = "Старший інженер", Department = "ІТ-відділ", Salary = 45000, HireDate = new DateTime(2023, 5, 12) },
                new Employee { Id = 2, FullName = "Мельник Ірина Петрівна", Position = "HR-менеджер", Department = "Відділ кадрів", Salary = 32000, HireDate = new DateTime(2024, 2, 1) },
                new Employee { Id = 3, FullName = "Бондаренко Дмитро Сергійович", Position = "Бухгалтер", Department = "Фінансовий відділ", Salary = 36000, HireDate = new DateTime(2024, 9, 15) }
            };
            _nextId = 4;
        }

        public static dbManager GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new dbManager();
                }
            }
            return _instance;
        }

        public bool IsConnected => _isConnected;
        public string ConnectionString => _connectionString;

        public void Connect(string? customConnectionString = null)
        {
            if (!string.IsNullOrWhiteSpace(customConnectionString))
                _connectionString = customConnectionString;

            _isConnected = true;
            Logger.GetInstance().Log($"[dbManager] Підключено до БД: {_connectionString}");
        }

        public void Disconnect()
        {
            _isConnected = false;
            Logger.GetInstance().Log("[dbManager] Відключено від БД.");
        }

        public Employee AddEmployee(string fullName, string position, string department, decimal salary)
        {
            EnsureConnected();
            var emp = new Employee
            {
                Id = _nextId++,
                FullName = fullName,
                Position = position,
                Department = department,
                Salary = salary,
                HireDate = DateTime.Now
            };
            _employees.Add(emp);
            Logger.GetInstance().Log($"[dbManager] INSERT Employee: {emp.FullName} ({emp.Position})");
            return emp;
        }

        public bool RemoveEmployee(int id)
        {
            EnsureConnected();
            var emp = _employees.FirstOrDefault(e => e.Id == id);
            if (emp == null)
                return false;

            _employees.Remove(emp);
            Logger.GetInstance().Log($"[dbManager] DELETE Employee #{id}: {emp.FullName}");
            return true;
        }

        public IReadOnlyList<Employee> GetAllEmployees()
        {
            EnsureConnected();
            Logger.GetInstance().Log("[dbManager] SELECT * FROM Employees");
            return _employees.AsReadOnly();
        }

        public string ExecuteQuery(string sql)
        {
            if (!_isConnected)
            {
                Logger.GetInstance().LogError($"[dbManager] Спроба виконати запит без підключення: {sql}");
                return "Помилка: немає активного підключення до бази даних.";
            }

            Logger.GetInstance().Log($"[dbManager] Виконано SQL-запит: {sql}");
            return $"Запит [{sql}] успішно виконано. Записів у таблиці Employees: {_employees.Count}.";
        }

        private void EnsureConnected()
        {
            if (!_isConnected)
                Connect();
        }
    }
}