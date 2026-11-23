using ZooFactoryLib.AbstractProducts;
using ZooFactoryLib.Factories;

namespace ZooFactoryLib
{
    public class ZooSector
    {
        private readonly Mammal _mammal;
        private readonly Bird _bird;
        private readonly Reptile _reptile;
        public string ZoneTitle { get; }

        public ZooSector(ZooZoneFactory factory)
        {
            ZoneTitle = factory.ZoneName;
            _mammal = factory.CreateMammal();
            _bird = factory.CreateBird();
            _reptile = factory.CreateReptile();
        }

        public void DisplayInhabitants()
        {
            Console.WriteLine($"=== СЕКТОР ЗООПАРКУ: {ZoneTitle.ToUpper()} ===");
            Console.WriteLine(_mammal.GetInfo());
            Console.WriteLine(_bird.GetInfo());
            Console.WriteLine(_reptile.GetInfo());
            Console.WriteLine(new string('-', 55));
        }
    }
}