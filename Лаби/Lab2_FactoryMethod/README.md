# Лабораторна робота №2. Реалізація патерну «Фабричний метод»

**Варіант 14:** Фабричний метод для сервірування столу (залежно від призначення та кількості гостей).

## Структура рішення:
1. **`TableSettingLib`** (Class Library -> `TableSettingLib.dll`):
   - `TableSetting.cs` — абстрактний продукт.
   - `ConcreteSettings.cs` — 3 конкретні продукти (`BreakfastSetting`, `BanquetSetting`, `BuffetSetting`).
   - `TableSettingCreators.cs` — абстрактний творець (`TableSettingCreator`) та 3 конкретні фабрики (`BreakfastCreator`, `BanquetCreator`, `BuffetCreator`).
2. **`TableSettingClientApp`** (Console App):
   - Підключає бібліотеку `TableSettingLib.dll` та дозволяє обрати тип події й кількість гостей через консольне меню.

## Запуск:
- Відкрийте `FactoryMethodTableSetting.sln` у Visual Studio та натисніть **Ctrl + F5**.
- Або через термінал: `dotnet run --project TableSettingClientApp`