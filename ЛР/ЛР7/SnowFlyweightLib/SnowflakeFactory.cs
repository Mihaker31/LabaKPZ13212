using System.Drawing;

namespace SnowFlyweightLib
{
    // Фабрика легковаговиків (Flyweight Factory): кешує та повторно використовує типи сніжинок
    public class SnowflakeFactory
    {
        private readonly Dictionary<string, SnowflakeType> _cache = new Dictionary<string, SnowflakeType>();

        public int CachedTypesCount => _cache.Count;

        public SnowflakeType GetSnowflakeType(string name, SnowflakeShape shape, Color color, float baseSize)
        {
            string key = $"{name}_{shape}_{color.ToArgb()}_{baseSize:F1}";
            if (!_cache.TryGetValue(key, out SnowflakeType? type))
            {
                type = new SnowflakeType(name, shape, color, baseSize);
                _cache[key] = type;
            }
            return type;
        }

        public IEnumerable<SnowflakeType> GetAllCachedTypes() => _cache.Values;
    }
}
