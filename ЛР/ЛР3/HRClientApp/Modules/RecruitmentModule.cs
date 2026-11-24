using HRSingletonLib;

namespace HRClientApp.Modules
{
    // Модуль 1: Найм, звільнення та перегляд співробітників (Відділ кадрів)
    public class RecruitmentModule
    {
        public dbManager Db => dbManager.GetInstance();
        public DocumentSaver Docs => DocumentSaver.GetInstance();
        public Logger Log => Logger.GetInstance();

        public void ShowAllEmployees()
        {
            Log.Log("[RecruitmentModule] Користувач відкрив список співробітників.");
            var list = Db.GetAllEmployees();

            Console.WriteLine("--- СПИСОК СПІВРОБІТНИКІВ ВІДДІЛУ КАДРІВ ---");
            if (list.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            foreach (var emp in list)
                Console.WriteLine(emp);
        }

        public void HireEmployee()
        {
            Console.Write("ПІБ співробітника: ");
            string fullName = Console.ReadLine() ?? "Невідомий";

            Console.Write("Посада: ");
            string position = Console.ReadLine() ?? "Спеціаліст";

            Console.Write("Відділ: ");
            string department = Console.ReadLine() ?? "Загальний";

            Console.Write("Оклад (грн): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal salary) || salary <= 0)
                salary = 20000;

            var emp = Db.AddEmployee(fullName, position, department, salary);

            // Автоматичне формування наказу про прийняття на роботу через DocumentSaver
            string orderFile = $"Nakaz_Hire_{emp.Id}_{DateTime.Now:yyyyMMdd}.txt";
            string orderText =
                $"НАКАЗ ПРО ПРИЙНЯТТЯ НА РОБОТУ №{emp.Id}
" +
                $"Дата: {DateTime.Now:dd.MM.yyyy}
" +
                $"Прийняти: {emp.FullName}
" +
                $"Посада: {emp.Position}
" +
                $"Підрозділ: {emp.Department}
" +
                $"Посадовий оклад: {emp.Salary:N0} грн
";

            string savedPath = Docs.SaveDocument(orderFile, orderText);
            Log.Log($"[RecruitmentModule] Прийнято співробітника #{emp.Id} та створено наказ {orderFile}.");

            Console.WriteLine($"
Співробітника додано до БД! Наказ збережено у репозиторій:
{savedPath}");
        }

        public void DismissEmployee()
        {
            ShowAllEmployees();
            Console.Write("
Введіть ID співробітника для звільнення: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Некоректний ID.");
                return;
            }

            if (Db.RemoveEmployee(id))
            {
                string orderFile = $"Nakaz_Dismiss_{id}_{DateTime.Now:yyyyMMdd}.txt";
                Docs.SaveDocument(orderFile, $"НАКАЗ ПРО ЗВІЛЬНЕННЯ
Дата: {DateTime.Now:dd.MM.yyyy}
Звільнено співробітника з ID #{id}.
");
                Log.Log($"[RecruitmentModule] Звільнено співробітника #{id}.");
                Console.WriteLine($"Співробітника #{id} звільнено, наказ збережено ({orderFile}).");
            }
            else
            {
                Log.LogError($"[RecruitmentModule] Невдала спроба звільнення: ID #{id} не знайдено.");
                Console.WriteLine($"Співробітника з ID #{id} не знайдено.");
            }
        }
    }
}