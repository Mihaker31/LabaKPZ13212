namespace BuildingPatternsLib
{
    // Будівельник 1: Економ-клас (9 поверхів, панельний)
    public class EconomyPanelBuilder : IBuildingBuilder
    {
        private MultiStoryBuilding _building = new MultiStoryBuilding();
        public string BuilderTitle => "Економ-клас (панельний, 9 пов.)";

        public void Reset()
        {
            _building = new MultiStoryBuilding
            {
                BuildingClass = "Економ-клас",
                FloorsCount = 9,
                WallColorHex = "#CBD2D9",
                RoofKind = RoofStyle.FlatPanel
            };
            _building.Name = $"Економ #{_building.Id}";
        }

        public void BuildFoundation()
        {
            _building.Foundation = "Стрічковий залізобетонний фундамент";
            _building.HasFoundation = true;
        }

        public void BuildStructure()
        {
            _building.FloorsAndWalls = "9 поверхів, збірні залізобетонні панелі";
            _building.HasStructure = true;
        }

        public void BuildRoof()
        {
            _building.Roof = "Плоский руберойдний дах із технічним поверхом";
            _building.HasRoof = true;
        }

        public void InstallElevators()
        {
            _building.Elevators = "1 пасажирський ліфт на під'їзд (400 кг)";
            _building.HasElevators = true;
        }

        public void InstallUtilities()
        {
            _building.Utilities = "Централізоване опалення, базові лічильники";
            _building.HasUtilities = true;
        }

        public void BuildParking()
        {
            _building.Parking = "Відкрита гостьова стоянка у дворі";
            _building.HasParking = true;
        }

        public MultiStoryBuilding GetBuilding() => _building;
    }

    // Будівельник 2: Комфорт-клас (16 поверхів, цегляний)
    public class ComfortBrickBuilder : IBuildingBuilder
    {
        private MultiStoryBuilding _building = new MultiStoryBuilding();
        public string BuilderTitle => "Комфорт-клас (цегляний, 16 пов.)";

        public void Reset()
        {
            _building = new MultiStoryBuilding
            {
                BuildingClass = "Комфорт-клас",
                FloorsCount = 16,
                WallColorHex = "#D99B78",
                RoofKind = RoofStyle.MembraneModern
            };
            _building.Name = $"Комфорт #{_building.Id}";
        }

        public void BuildFoundation()
        {
            _building.Foundation = "Монолітна залізобетонна плита";
            _building.HasFoundation = true;
        }

        public void BuildStructure()
        {
            _building.FloorsAndWalls = "16 поверхів, червона цегла + утеплення мінватою";
            _building.HasStructure = true;
        }

        public void BuildRoof()
        {
            _building.Roof = "Мембранна експлуатована покрівля з парапетом";
            _building.HasRoof = true;
        }

        public void InstallElevators()
        {
            _building.Elevators = "2 ліфти (пасажирський + вантажопасажирський)";
            _building.HasElevators = true;
        }

        public void InstallUtilities()
        {
            _building.Utilities = "Дахова котельня, поквартирні теплолічильники";
            _building.HasUtilities = true;
        }

        public void BuildParking()
        {
            _building.Parking = "Наземний багаторівневий паркінг із шлагбаумом";
            _building.HasParking = true;
        }

        public MultiStoryBuilding GetBuilding() => _building;
    }

    // Будівельник 3: Преміум-клас (25 поверхів, монолітно-каркасний)
    public class PremiumMonolithBuilder : IBuildingBuilder
    {
        private MultiStoryBuilding _building = new MultiStoryBuilding();
        public string BuilderTitle => "Преміум-клас (моноліт, 25 пов.)";

        public void Reset()
        {
            _building = new MultiStoryBuilding
            {
                BuildingClass = "Преміум-клас",
                FloorsCount = 25,
                WallColorHex = "#5A7D9A",
                RoofKind = RoofStyle.GreenTerrace
            };
            _building.Name = $"Преміум #{_building.Id}";
        }

        public void BuildFoundation()
        {
            _building.Foundation = "Пальово-ростверковий фундамент глибокого закладення";
            _building.HasFoundation = true;
        }

        public void BuildStructure()
        {
            _building.FloorsAndWalls = "25 поверхів, монолітний каркас, панорамне скління";
            _building.HasStructure = true;
        }

        public void BuildRoof()
        {
            _building.Roof = "Озеленена тераса на даху із зоною відпочинку";
            _building.HasRoof = true;
        }

        public void InstallElevators()
        {
            _building.Elevators = "4 швидкісні безшумні ліфти зі спуском у паркінг";
            _building.HasElevators = true;
        }

        public void InstallUtilities()
        {
            _building.Utilities = "Система 'Розумний дім', клімат-контроль, рекуперація";
            _building.HasUtilities = true;
        }

        public void BuildParking()
        {
            _building.Parking = "Дворівневий підземний паркінг із зарядками для електрокарів";
            _building.HasParking = true;
        }

        public MultiStoryBuilding GetBuilding() => _building;
    }
}
