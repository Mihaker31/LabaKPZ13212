namespace HRSingletonLib
{
    // Завдання 2: Глобальна точка доступу до репозиторію документів (Singleton)
    public sealed class DocumentSaver
    {
        private static DocumentSaver? _instance;
        private static readonly object _lock = new object();

        private string _repositoryPath;

        public Guid InstanceId { get; }
        public string RepositoryPath => _repositoryPath;

        private DocumentSaver()
        {
            InstanceId = Guid.NewGuid();
            _repositoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HR_Documents");
            Directory.CreateDirectory(_repositoryPath);
        }

        public static DocumentSaver GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DocumentSaver();
                }
            }
            return _instance;
        }

        public void SetRepositoryPath(string newPath)
        {
            if (string.IsNullOrWhiteSpace(newPath))
                return;

            _repositoryPath = newPath;
            Directory.CreateDirectory(_repositoryPath);
            Logger.GetInstance().Log($"[DocumentSaver] Змінено шлях репозиторію на: {_repositoryPath}");
        }

        public string SaveDocument(string fileName, string content)
        {
            if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                fileName += ".txt";

            string fullPath = Path.Combine(_repositoryPath, fileName);
            File.WriteAllText(fullPath, content);
            Logger.GetInstance().Log($"[DocumentSaver] Збережено документ: {fileName}");
            return fullPath;
        }

        public string LoadDocument(string fileName)
        {
            string fullPath = Path.Combine(_repositoryPath, fileName);
            if (!File.Exists(fullPath))
            {
                Logger.GetInstance().LogError($"[DocumentSaver] Документ не знайдено: {fileName}");
                return "Документ не знайдено у репозиторії.";
            }

            Logger.GetInstance().Log($"[DocumentSaver] Прочитано документ: {fileName}");
            return File.ReadAllText(fullPath);
        }

        public List<string> GetDocumentList()
        {
            var files = Directory.GetFiles(_repositoryPath);
            var result = files.Select(Path.GetFileName)
                              .Where(n => n != null)
                              .Cast<string>()
                              .ToList();
            Logger.GetInstance().Log($"[DocumentSaver] Отримано список документів (кількість: {result.Count})");
            return result;
        }
    }
}