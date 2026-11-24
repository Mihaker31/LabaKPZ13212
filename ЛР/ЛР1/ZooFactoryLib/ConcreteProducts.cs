using ZooFactoryLib.AbstractProducts;

namespace ZooFactoryLib.ConcreteProducts
{
    // ==========================================
    // Тварини Савани
    // ==========================================
    public class Lion : Mammal
    {
        public Lion() => Name = "Лев";
        public override string MakeSound() => "Ррр-мяу! (потужний рик)";
        public override string GetInfo() => $"[Ссавець] {Name}: {MakeSound()}";
    }

    public class Ostrich : Bird
    {
        public Ostrich() => Name = "Страус";
        public override string DoAction() => "Швидкий біг по савані до 70 км/год";
        public override string GetInfo() => $"[Птах] {Name}: {DoAction()}";
    }

    public class Crocodile : Reptile
    {
        public Crocodile() => Name = "Нільський крокодил";
        public override string GetHabitat() => "Береги річок та водойм савани";
        public override string GetInfo() => $"[Рептилія] {Name}: ареал — {GetHabitat()}";
    }

    // ==========================================
    // Тварини Арктики
    // ==========================================
    public class PolarBear : Mammal
    {
        public PolarBear() => Name = "Білий ведмідь";
        public override string MakeSound() => "Глухе ревіння серед крижаних просторів";
        public override string GetInfo() => $"[Ссавець] {Name}: {MakeSound()}";
    }

    public class Penguin : Bird
    {
        public Penguin() => Name = "Імператорський пінгвін";
        public override string DoAction() => "Вправно пірнає у крижану воду за рибою";
        public override string GetInfo() => $"[Птах] {Name}: {DoAction()}";
    }

    public class SnowLizard : Reptile
    {
        public SnowLizard() => Name = "Полярна (снігова) ящірка";
        public override string GetHabitat() => "Скелясті льодовикові тріщини";
        public override string GetInfo() => $"[Рептилія] {Name}: ареал — {GetHabitat()}";
    }

    // ==========================================
    // Тварини Джунглів
    // ==========================================
    public class Monkey : Mammal
    {
        public Monkey() => Name = "Шимпанзе";
        public override string MakeSound() => "У-у-а-а! (перегукування в кронах дерев)";
        public override string GetInfo() => $"[Ссавець] {Name}: {MakeSound()}";
    }

    public class Parrot : Bird
    {
        public Parrot() => Name = "Папуга Ара";
        public override string DoAction() => "Літає між тропічними ліанами та копіює звуки";
        public override string GetInfo() => $"[Птах] {Name}: {DoAction()}";
    }

    public class Chameleon : Reptile
    {
        public Chameleon() => Name = "Мадагаскарський хамелеон";
        public override string GetHabitat() => "Густе гілля дерев вологого тропічного лісу";
        public override string GetInfo() => $"[Рептилія] {Name}: ареал — {GetHabitat()}";
    }
}