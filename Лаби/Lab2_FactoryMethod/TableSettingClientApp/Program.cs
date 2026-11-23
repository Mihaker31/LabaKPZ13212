using System;
using System.Text;
using TableSettingLib.Creators;

namespace TableSettingClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("===========================================================");
            Console.WriteLine(" Лабораторна робота №2: Патерн 'Фабричний метод' (DLL)     ");
            Console.WriteLine(" Варіант 14: Сервірування столу                            ");
            Console.WriteLine("===========================================================\n");

            while (true)
            {
                Console.WriteLine("Оберіть призначення столу:");
                Console.WriteLine("1 - Сніданок (BreakfastCreator)");
                Console.WriteLine("2 - Святковий банкет (BanquetCreator)");
                Console.WriteLine("3 - Фуршет (BuffetCreator)");
                Console.WriteLine("0 - Вихід");
                Console.Write("\nВаш вибір: ");

                string? choice = Console.ReadLine();
                if (choice == "0") break;

                TableSettingCreator? creator = choice switch
                {
                    "1" => new BreakfastCreator(),
                    "2" => new BanquetCreator(),
                    "3" => new BuffetCreator(),
                    _ => null
                };

                if (creator == null)
                {
                    Console.WriteLine("\nНевірний вибір. Спробуйте ще раз.\n");
                    continue;
                }

                Console.Write("Введіть кількість гостей: ");
                if (!int.TryParse(Console.ReadLine(), out int guests) || guests <= 0)
                {
                    Console.WriteLine("Некоректне число, встановлено значення за замовчуванням: 1 гість.");
                    guests = 1;
                }

                Console.WriteLine();
                Console.WriteLine(creator.ServeTable(guests));
                Console.WriteLine(new string('-', 55));
                Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Роботу програми завершено.");
        }
    }
}