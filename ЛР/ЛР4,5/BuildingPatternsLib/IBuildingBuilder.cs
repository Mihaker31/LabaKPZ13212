namespace BuildingPatternsLib
{
    // Інтерфейс Будівельника (Лаб 4) — 6 кроків зведення
    public interface IBuildingBuilder
    {
        string BuilderTitle { get; }
        void Reset();
        void BuildFoundation();
        void BuildStructure();
        void BuildRoof();
        void InstallElevators();
        void InstallUtilities();
        void BuildParking();
        MultiStoryBuilding GetBuilding();
    }
}
