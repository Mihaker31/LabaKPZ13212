namespace HRSingletonLib
{
    // Завдання 3: Логування операцій у файл (Singleton)
    public sealed class Logger
    {
        private static Logger? _instance;
        private static readonly object _lock = new object();

        private readonly string _logFilePath;

        public Guid InstanceId { get; }
        public string LogFilePath => _logFilePath;

        private Logger()
        {
            InstanceId = Guid.NewGuid();
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hr_operations.log");
        }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Logger();
                }
            }
            return _instance;
        }

        public void Log(string action)
        {
            lock (_lock)
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {action}{Environment.NewLine}";
                File.AppendAllText(_logFilePath, line);
            }
        }

        public void LogError(string errorMessage)
        {
            lock (_lock)
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {errorMessage}{Environment.NewLine}";
                File.AppendAllText(_logFilePath, line);
            }
        }

        public string ReadLogs()
        {
            lock (_lock)
            {
                return File.Exists(_logFilePath)
                    ? File.ReadAllText(_logFilePath)
                    : "Журнал операцій порожній.";
            }
        }
    }
}