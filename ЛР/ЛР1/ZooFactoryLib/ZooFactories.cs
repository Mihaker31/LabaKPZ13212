using ZooFactoryLib.AbstractProducts;
using ZooFactoryLib.ConcreteProducts;

namespace ZooFactoryLib.Factories
{
    // ==========================================
    // Абстрактна фабрика
    // ==========================================
    public abstract class ZooZoneFactory
    {
        public abstract string ZoneName { get; }

        // Фабричні методи для створення компонентів біому
        public abstract Mammal CreateMammal();
        public abstract Bird CreateBird();
        public abstract Reptile CreateReptile();
    }

    // ==========================================
    // Конкретна фабрика 1: Савана
    // ==========================================
    public class SavannaZooFactory : ZooZoneFactory
    {
        public override string ZoneName => "Африканська Савана";
        public override Mammal CreateMammal() => new Lion();
        public override Bird CreateBird() => new Ostrich();
        public override Reptile CreateReptile() => new Crocodile();
    }

    // ==========================================
    // Конкретна фабрика 2: Арктика
    // ==========================================
    public class ArcticZooFactory : ZooZoneFactory
    {
        public override string ZoneName => "Арктична зона";
        public override Mammal CreateMammal() => new PolarBear();
        public override Bird CreateBird() => new Penguin();
        public override Reptile CreateReptile() => new SnowLizard();
    }

    // ==========================================
    // Конкретна фабрика 3: Джунглі
    // ==========================================
    public class JungleZooFactory : ZooZoneFactory
    {
        public override string ZoneName => "Тропічні Джунглі";
        public override Mammal CreateMammal() => new Monkey();
        public override Bird CreateBird() => new Parrot();
        public override Reptile CreateReptile() => new Chameleon();
    }
}