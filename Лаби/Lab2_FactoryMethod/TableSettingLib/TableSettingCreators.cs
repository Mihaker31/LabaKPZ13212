using TableSettingLib.Products;

namespace TableSettingLib.Creators
{
    // Абстрактний творець (фабрика)
    public abstract class TableSettingCreator
    {
        // Фабричний метод
        public abstract TableSetting CreateSetting(int guestCount);

        // Загальний метод підготовки столу, що працює з абстрактним продуктом
        public string ServeTable(int guestCount)
        {
            TableSetting setting = CreateSetting(guestCount);
            return "=== СЕРВІРУВАННЯ ГОТОВЕ ===\n" + setting.GetDescription();
        }
    }

    // Конкретна фабрика 1: Сніданок
    public class BreakfastCreator : TableSettingCreator
    {
        public override TableSetting CreateSetting(int guestCount) => new BreakfastSetting(guestCount);
    }

    // Конкретна фабрика 2: Банкет
    public class BanquetCreator : TableSettingCreator
    {
        public override TableSetting CreateSetting(int guestCount) => new BanquetSetting(guestCount);
    }

    // Конкретна фабрика 3: Фуршет
    public class BuffetCreator : TableSettingCreator
    {
        public override TableSetting CreateSetting(int guestCount) => new BuffetSetting(guestCount);
    }
}