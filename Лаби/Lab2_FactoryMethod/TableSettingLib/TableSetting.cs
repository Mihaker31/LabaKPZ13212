namespace TableSettingLib.Products
{
    // Абстрактний продукт — базовий клас сервірування столу
    public abstract class TableSetting
    {
        public string Purpose { get; protected set; } = string.Empty;
        public int GuestCount { get; protected set; }
        public string Tablecloth { get; protected set; } = string.Empty;

        protected TableSetting(int guestCount)
        {
            GuestCount = guestCount > 0 ? guestCount : 1;
        }

        public abstract string GetDescription();
    }
}