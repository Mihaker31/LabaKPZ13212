namespace ZooFactoryLib.AbstractProducts
{
    // Ссавець
    public abstract class Mammal
    {
        public string Name { get; set; } = string.Empty;
        public abstract string MakeSound();
        public abstract string GetInfo();
    }

    // Птах
    public abstract class Bird
    {
        public string Name { get; set; } = string.Empty;
        public abstract string DoAction();
        public abstract string GetInfo();
    }

    // Рептилія
    public abstract class Reptile
    {
        public string Name { get; set; } = string.Empty;
        public abstract string GetHabitat();
        public abstract string GetInfo();
    }
}