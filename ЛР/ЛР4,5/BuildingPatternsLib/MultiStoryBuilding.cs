namespace BuildingPatternsLib
{
    public enum RoofStyle
    {
        FlatPanel,
        MembraneModern,
        GreenTerrace
    }

    // Складний продукт (Лаб 4) та Конкретний Прототип (Лаб 5)
    public class MultiStoryBuilding : IBuildingPrototype
    {
        private static int _globalCounter = 1;

        public int Id { get; private set; }
        public string Name { get; set; }
        public string BuildingClass { get; set; } = string.Empty;

        // 6 складових будинку
        public string Foundation { get; set; } = "Не збудовано";
        public string FloorsAndWalls { get; set; } = "Не збудовано";
        public string Roof { get; set; } = "Не збудовано";
        public string Elevators { get; set; } = "Не встановлено";
        public string Utilities { get; set; } = "Не підключено";
        public string Parking { get; set; } = "Не облаштовано";

        // Прапорці виконаних кроків для покрокової візуалізації
        public bool HasFoundation { get; set; }
        public bool HasStructure { get; set; }
        public bool HasRoof { get; set; }
        public bool HasElevators { get; set; }
        public bool HasUtilities { get; set; }
        public bool HasParking { get; set; }

        // Візуальні параметри
        public int FloorsCount { get; set; } = 9;
        public string WallColorHex { get; set; } = "#C8CFD8";
        public RoofStyle RoofKind { get; set; } = RoofStyle.FlatPanel;

        // Дані прототипу (для відстеження клонів)
        public bool IsClone { get; private set; }
        public int? ClonedFromId { get; private set; }

        public int CompletedSteps =>
            (HasFoundation ? 1 : 0) +
            (HasStructure ? 1 : 0) +
            (HasRoof ? 1 : 0) +
            (HasElevators ? 1 : 0) +
            (HasUtilities ? 1 : 0) +
            (HasParking ? 1 : 0);

        public MultiStoryBuilding()
        {
            Id = _globalCounter++;
            Name = $"Будинок #{Id}";
        }

        // Глибоке клонування (Патерн Прототип)
        public MultiStoryBuilding Clone()
        {
            var copy = new MultiStoryBuilding
            {
                BuildingClass = BuildingClass,
                Foundation = Foundation,
                FloorsAndWalls = FloorsAndWalls,
                Roof = Roof,
                Elevators = Elevators,
                Utilities = Utilities,
                Parking = Parking,
                HasFoundation = HasFoundation,
                HasStructure = HasStructure,
                HasRoof = HasRoof,
                HasElevators = HasElevators,
                HasUtilities = HasUtilities,
                HasParking = HasParking,
                FloorsCount = FloorsCount,
                WallColorHex = WallColorHex,
                RoofKind = RoofKind,
                IsClone = true,
                ClonedFromId = Id
            };
            copy.Name = $"{BuildingClass} #{copy.Id} (копія #{Id})";
            return copy;
        }

        public string GetSpecification()
        {
            string origin = IsClone
                ? $"КОПІЯ (Прототип від будинку #{ClonedFromId})"
                : "ОРИГІНАЛ (збудовано Будівельником)";

            return $"=== {Name} ===\r\n" +
                   $"Статус: {origin}\r\n" +
                   $"Клас житла: {BuildingClass} | Поверхів: {FloorsCount} | Готовність: {CompletedSteps}/6\r\n" +
                   $"------------------------------------------------\r\n" +
                   $"1. Фундамент: {Foundation}\r\n" +
                   $"2. Каркас і стіни: {FloorsAndWalls}\r\n" +
                   $"3. Покрівля: {Roof}\r\n" +
                   $"4. Ліфти: {Elevators}\r\n" +
                   $"5. Комунікації: {Utilities}\r\n" +
                   $"6. Паркінг: {Parking}";
        }

        public override string ToString()
        {
            string badge = IsClone ? "[Клон]" : "[Оригінал]";
            return $"{badge} {Name} ({FloorsCount} пов., {CompletedSteps}/6)";
        }
    }
}
