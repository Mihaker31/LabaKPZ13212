using HRSingletonLib;

namespace HRClientApp.Modules
{
    // Модуль 2: Кадровий документообіг (робота з репозиторієм документів)
    public class DocumentArchiveModule
    {
        public DocumentSaver Docs => DocumentSaver.GetInstance();
        public Logger Log => Logger.GetInstance();

        public void ShowDocuments()
        {
            Log.Log("[DocumentArchiveModule] Перегляд списку кадрових документів.");
            var files = Docs.GetDocumentList();

            Console.WriteLine($"--- РЕПОЗИТОРІЙ ДОКУМЕНТІВ ({Docs.RepositoryPath}) ---");
            if (files.Count == 0)
            {
                Console.WriteLine("Документів ще немає.");
                return;
            }

            for (int i = 0; i < files.Count; i++)
                Console.WriteLine($"{i + 1}. {files[i]}");
        }

        public void CreateCustomDocument()
        {
            Console.Write("Назва документа (наприклад, Zayava_Vidpustka): ");
            string name = Console.ReadLine() ?? $"Doc_{DateTime.Now:HHmmss}";

            Console.Write("Текст документа: ");
            string text = Console.ReadLine() ?? string.Empty;

            string path = Docs.SaveDocument(name, text);
            Log.Log($"[DocumentArchiveModule] Користувач створив документ '{name}'.");
            Console.WriteLine($"Документ збережено: {path}");
        }

        public void ReadDocument()
        {
            ShowDocuments();
            Console.Write("
Введіть точну назву файлу для перегляду: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("
--- ВМІСТ ДОКУМЕНТА ---");
            Console.WriteLine(Docs.LoadDocument(name));
        }
    }
}