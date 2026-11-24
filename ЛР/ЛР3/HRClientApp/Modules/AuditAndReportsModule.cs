using HRSingletonLib;

namespace HRClientApp.Modules
{
    // Модуль 3: Аудит, перегляд логів та перевірка єдиності екземплярів Singleton між модулями
    public class AuditAndReportsModule
    {
        public dbManager Db => dbManager.GetInstance();
        public DocumentSaver Docs => DocumentSaver.GetInstance();
        public Logger Log => Logger.GetInstance();

        public void ShowLogs()
        {
            Console.WriteLine($"--- ЖУРНАЛ ОПЕРАЦІЙ ({Log.LogFilePath}) ---");
            Console.WriteLine(Log.ReadLogs());
        }

        public void GenerateSummaryReport()
        {
            var employees = Db.GetAllEmployees();
            decimal totalPayroll = employees.Sum(e => e.Salary);

            string reportName = $"HR_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string content =
                $"ЗВЕДЕНИЙ ЗВІТ ВІДДІЛУ КАДРІВ
" +
                $"Сформовано: {DateTime.Now:dd.MM.yyyy HH:mm:ss}
" +
                $"Кількість співробітників: {employees.Count}
" +
                $"Загальний фонд оплати праці: {totalPayroll:N0} грн
";

            string path = Docs.SaveDocument(reportName, content);
            Log.Log($"[AuditAndReportsModule] Згенеровано зведений звіт: {reportName}");
            Console.WriteLine(content);
            Console.WriteLine($"Звіт збережено у репозиторій: {path}");
        }

        public void VerifySingletonAcrossModules(RecruitmentModule m1, DocumentArchiveModule m2)
        {
            Console.WriteLine("=== ПЕРЕВІРКА ЄДИНОГО ЕКЗЕМПЛЯРА (SINGLETON) У РІЗНИХ МОДУЛЯХ ===
");

            bool sameDb = ReferenceEquals(m1.Db, Db);
            bool sameDocs = ReferenceEquals(m1.Docs, m2.Docs) && ReferenceEquals(m2.Docs, Docs);
            bool sameLogger = ReferenceEquals(m1.Log, m2.Log) && ReferenceEquals(m2.Log, Log);

            Console.WriteLine($"1. dbManager:");
            Console.WriteLine($"   - RecruitmentModule InstanceId:     {m1.Db.InstanceId}");
            Console.WriteLine($"   - AuditAndReportsModule InstanceId: {Db.InstanceId}");
            Console.WriteLine($"   - Один і той самий об'єкт?          {(sameDb ? "ТАК (True)" : "НІ (False)")}
");

            Console.WriteLine($"2. DocumentSaver:");
            Console.WriteLine($"   - RecruitmentModule InstanceId:     {m1.Docs.InstanceId}");
            Console.WriteLine($"   - DocumentArchiveModule InstanceId: {m2.Docs.InstanceId}");
            Console.WriteLine($"   - AuditAndReportsModule InstanceId: {Docs.InstanceId}");
            Console.WriteLine($"   - Один і той самий об'єкт?          {(sameDocs ? "ТАК (True)" : "НІ (False)")}
");

            Console.WriteLine($"3. Logger:");
            Console.WriteLine($"   - RecruitmentModule InstanceId:     {m1.Log.InstanceId}");
            Console.WriteLine($"   - DocumentArchiveModule InstanceId: {m2.Log.InstanceId}");
            Console.WriteLine($"   - AuditAndReportsModule InstanceId: {Log.InstanceId}");
            Console.WriteLine($"   - Один і той самий об'єкт?          {(sameLogger ? "ТАК (True)" : "НІ (False)")}");

            Log.Log("[AuditAndReportsModule] Виконано перевірку єдиності екземплярів Singleton.");
        }
    }
}