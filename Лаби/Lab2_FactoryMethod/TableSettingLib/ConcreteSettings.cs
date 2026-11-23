namespace TableSettingLib.Products
{
    // Реальний продукт 1: Сервірування для сніданку
    public class BreakfastSetting : TableSetting
    {
        public BreakfastSetting(int guestCount) : base(guestCount)
        {
            Purpose = "Сніданок";
            Tablecloth = "Лляна скатертина пастельного кольору";
        }

        public override string GetDescription()
        {
            return $"Призначення: {Purpose} (гостей: {GuestCount})\n" +
                   $"- Текстиль: {Tablecloth}\n" +
                   $"- Посуд: {GuestCount} десертних тарілок, {GuestCount} кавових чашок із блюдцями\n" +
                   $"- Прибори: {GuestCount} ножів для масла, {GuestCount} чайних ложок";
        }
    }

    // Реальний продукт 2: Святковий банкет
    public class BanquetSetting : TableSetting
    {
        public BanquetSetting(int guestCount) : base(guestCount)
        {
            Purpose = "Святковий банкет";
            Tablecloth = "Білосніжна парадна скатертина та тканинні серветки";
        }

        public override string GetDescription()
        {
            int glasses = GuestCount * 3;
            return $"Призначення: {Purpose} (гостей: {GuestCount})\n" +
                   $"- Текстиль: {Tablecloth}\n" +
                   $"- Посуд: {GuestCount} підставних та {GuestCount * 2} основних тарілок\n" +
                   $"- Скло: {glasses} келихів (вода, вино, шампанське)\n" +
                   $"- Прибори: повний банкетний набір на {GuestCount} персон";
        }
    }

    // Реальний продукт 3: Фуршет
    public class BuffetSetting : TableSetting
    {
        public BuffetSetting(int guestCount) : base(guestCount)
        {
            Purpose = "Фуршет";
            Tablecloth = "Фуршетна спідниця на високі столи";
        }

        public override string GetDescription()
        {
            int plates = GuestCount * 2;
            int glasses = (int)Math.Ceiling(GuestCount * 1.5);
            return $"Призначення: {Purpose} (гостей: {GuestCount})\n" +
                   $"- Текстиль: {Tablecloth}\n" +
                   $"- Посуд: {plates} закусочних тарілок (стопками по 10 шт.)\n" +
                   $"- Скло: {glasses} фужерів на окремій лінії напоїв\n" +
                   $"- Прибори: {plates} фуршетних виделок та шпажки";
        }
    }
}