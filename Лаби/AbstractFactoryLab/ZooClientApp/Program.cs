using System;
using System.Text;
using ZooFactoryLib;
using ZooFactoryLib.Factories;

namespace ZooClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==========================================================");
            Console.WriteLine(" Лабораторна робота №1: Патерн 'Абстрактна фабрика' (DLL) ");
            Console.WriteLine(" Тема: Моделювання зон зоопарку                           ");
            Console.WriteLine("==========================================================\n");

            while (true)
            {
                Console.WriteLine("Оберіть зону зоопарку для заселення:");
                Console.WriteLine("1 - Савана (SavannaZooFactory)");
                Console.WriteLine("2 - Арктика (ArcticZooFactory)");
                Console.WriteLine("3 - Джунглі (JungleZooFactory)");
                Console.WriteLine("4 - Заселити всі зони одночасно");
                Console.WriteLine("0 - Вихід з програми");
                Console.Write("\nВаш вибір: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                if (choice == "0") break;

                ZooZoneFactory? selectedFactory = choice switch
                {
                    "1" => new SavannaZooFactory(),
                    "2" => new ArcticZooFactory(),
                    "3" => new JungleZooFactory(),
                    "4" => null,
                    _ => null
                };

                if (choice == "4")
                {
                    ZooZoneFactory[] allFactories = {
                        new SavannaZooFactory(),
                        new ArcticZooFactory(),
                        new JungleZooFactory()
                    };

                    foreach (var f in allFactories)
                    {
                        ZooSector sector = new ZooSector(f);
                        sector.DisplayInhabitants();
                    }
                }
                else if (selectedFactory != null)
                {
                    ZooSector sector = new ZooSector(selectedFactory);
                    sector.DisplayInhabitants();
                }
                else
                {
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.\n");
                }

                Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Роботу програми завершено.");
        }
    }
}