using System.Text;
using HRSingletonLib;
using HRClientApp.Modules;

namespace HRClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Ініціалізація трьох незалежних модулів програми
            var recruitmentModule = new RecruitmentModule();
            var documentModule = new DocumentArchiveModule();
            var auditModule = new AuditAndReportsModule();

            Logger.GetInstance().Log("=== ЗАПУСК ПРОГРАМИ 'ВІДДІЛ КАДРІВ' ===");
            dbManager.GetInstance().Connect();

            while (true)
            {
                Console.WriteLine("===========================================================");
                Console.WriteLine(" Лабораторна робота №3: Патерн Singleton (DLL)             ");
                Console.WriteLine(" Варіант 14: Відділ кадрів                                 ");
                Console.WriteLine("===========================================================");
                Console.WriteLine(" [Модуль 1: Кадри (dbManager + DocumentSaver + Logger)]");
                Console.WriteLine("  1 - Переглянути всіх співробітників");
                Console.WriteLine("  2 - Прийняти співробітника на роботу (+ наказ у репозиторій)");
                Console.WriteLine("  3 - Звільнити співробітника (+ наказ у репозиторій)");
                Console.WriteLine(" [Модуль 2: Документообіг (DocumentSaver + Logger)]");
                Console.WriteLine("  4 - Переглянути список документів у репозиторії");
                Console.WriteLine("  5 - Створити кадровий документ");
                Console.WriteLine("  6 - Прочитати документ");
                Console.WriteLine(" [Модуль 3: Аудит і звітність (Logger + dbManager + DocumentSaver)]");
                Console.WriteLine("  7 - Сформувати зведений звіт відділу кадрів");
                Console.WriteLine("  8 - Переглянути журнал операцій (Logger)");
                Console.WriteLine("  9 - Перевірити єдиність екземплярів Singleton між модулями");
                Console.WriteLine("  0 - Вихід");
                Console.Write("
Ваш вибір: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                if (choice == "0")
                {
                    dbManager.GetInstance().Disconnect();
                    Logger.GetInstance().Log("=== ЗАВЕРШЕННЯ РОБОТИ ПРОГРАМИ ===");
                    break;
                }

                switch (choice)
                {
                    case "1": recruitmentModule.ShowAllEmployees(); break;
                    case "2": recruitmentModule.HireEmployee(); break;
                    case "3": recruitmentModule.DismissEmployee(); break;
                    case "4": documentModule.ShowDocuments(); break;
                    case "5": documentModule.CreateCustomDocument(); break;
                    case "6": documentModule.ReadDocument(); break;
                    case "7": auditModule.GenerateSummaryReport(); break;
                    case "8": auditModule.ShowLogs(); break;
                    case "9": auditModule.VerifySingletonAcrossModules(recruitmentModule, documentModule); break;
                    default:
                        Console.WriteLine("Невірний пункт меню. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("
Натисніть будь-яку клавішу для повернення в меню...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}