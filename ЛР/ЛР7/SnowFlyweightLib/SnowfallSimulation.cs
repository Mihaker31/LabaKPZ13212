using System.Drawing;

namespace SnowFlyweightLib
{
    public record SnowflakePreset(string Name, SnowflakeShape Shape, Color Color, float BaseSize);

    // Клієнтський менеджер симуляції снігопаду
    public class SnowfallSimulation
    {
        private readonly List<Snowflake> _snowflakes = new List<Snowflake>();
        private readonly SnowflakeFactory _factory = new SnowflakeFactory();

        public float Wind { get; set; } = 0.6f;
        public float SpeedMultiplier { get; set; } = 1.0f;

        public int SnowflakeCount => _snowflakes.Count;
        public int CachedFlyweightCount => _factory.CachedTypesCount;

        public static readonly SnowflakePreset[] DefaultPresets =
        {
            new SnowflakePreset("Зірка", SnowflakeShape.Star6, Color.FromArgb(245, 252, 255), 10f),
            new SnowflakePreset("Кристал", SnowflakeShape.Crystal, Color.FromArgb(185, 228, 255), 9f),
            new SnowflakePreset("Пухнаста", SnowflakeShape.Fluffy, Color.FromArgb(255, 255, 255), 6.5f),
            new SnowflakePreset("Шестикутник", SnowflakeShape.Hexagon, Color.FromArgb(200, 218, 255), 7.5f),
            new SnowflakePreset("Іскра", SnowflakeShape.Sparkle, Color.FromArgb(215, 248, 255), 6f)
        };

        public void RebuildSnowfall(int targetCount, IReadOnlyList<SnowflakePreset> enabledPresets, int canvasWidth, int canvasHeight)
        {
            _snowflakes.Clear();
            if (enabledPresets.Count == 0 || targetCount <= 0)
                return;

            int w = Math.Max(100, canvasWidth);
            int h = Math.Max(100, canvasHeight);

            for (int i = 0; i < targetCount; i++)
            {
                var preset = enabledPresets[Random.Shared.Next(enabledPresets.Count)];
                // Отримання спільного об'єкта з кешу фабрики легковаговиків!
                SnowflakeType sharedType = _factory.GetSnowflakeType(preset.Name, preset.Shape, preset.Color, preset.BaseSize);

                float x = Random.Shared.Next(0, w);
                float y = Random.Shared.Next(0, h);
                float speedY = 0.9f + (float)Random.Shared.NextDouble() * 2.4f;
                float drift = (float)(Random.Shared.NextDouble() * Math.PI * 2);
                float scale = 0.55f + (float)Random.Shared.NextDouble() * 0.95f;
                float angle = Random.Shared.Next(0, 360);
                float rotSpeed = -1.8f + (float)Random.Shared.NextDouble() * 3.6f;

                _snowflakes.Add(new Snowflake(x, y, speedY, drift, scale, angle, rotSpeed, sharedType));
            }
        }

        public void Update(int width, int height)
        {
            foreach (var flake in _snowflakes)
            {
                flake.Update(width, height, Wind, SpeedMultiplier);
            }
        }

        public void DrawAll(Graphics g)
        {
            foreach (var flake in _snowflakes)
            {
                flake.Draw(g);
            }
        }

        public Dictionary<string, int> GetCountsByType()
        {
            return _snowflakes.GroupBy(f => f.Type.Name)
                              .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
