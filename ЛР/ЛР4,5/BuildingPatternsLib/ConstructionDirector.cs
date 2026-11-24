namespace BuildingPatternsLib
{
    // Директор (керує порядком етапів будівництва)
    public class ConstructionDirector
    {
        private IBuildingBuilder? _builder;

        public void SetBuilder(IBuildingBuilder builder)
        {
            _builder = builder;
        }

        public void StartNew()
        {
            EnsureBuilder();
            _builder!.Reset();
        }

        // Виконання конкретного кроку 1..6 для покрокової візуалізації
        public string ExecuteStep(int stepNumber)
        {
            EnsureBuilder();
            switch (stepNumber)
            {
                case 1:
                    _builder!.BuildFoundation();
                    return "Крок 1/6: Закладено фундамент.";
                case 2:
                    _builder!.BuildStructure();
                    return "Крок 2/6: Зведено поверхи, стіни та вікна.";
                case 3:
                    _builder!.BuildRoof();
                    return "Крок 3/6: Змонтовано покрівлю.";
                case 4:
                    _builder!.InstallElevators();
                    return "Крок 4/6: Встановлено ліфтову шахту та ліфти.";
                case 5:
                    _builder!.InstallUtilities();
                    return "Крок 5/6: Підключено комунікації та освітлення.";
                case 6:
                    _builder!.BuildParking();
                    return "Крок 6/6: Облаштовано паркінг. Будинок здано!";
                default:
                    return "Усі кроки будівництва вже виконано.";
            }
        }

        public void ConstructFullBuilding()
        {
            EnsureBuilder();
            _builder!.Reset();
            _builder.BuildFoundation();
            _builder.BuildStructure();
            _builder.BuildRoof();
            _builder.InstallElevators();
            _builder.InstallUtilities();
            _builder.BuildParking();
        }

        public void ConstructBasicBox()
        {
            EnsureBuilder();
            _builder!.Reset();
            _builder.BuildFoundation();
            _builder.BuildStructure();
            _builder.BuildRoof();
        }

        private void EnsureBuilder()
        {
            if (_builder == null)
                throw new InvalidOperationException("Спочатку потрібно призначити будівельника (SetBuilder).");
        }
    }
}
