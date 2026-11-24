using System.Drawing;

namespace SnowDecoratorLib
{
    // Базовий інтерфейс компонента (Component)
    public interface ISnowflakeRenderer
    {
        string Description { get; }
        void Draw(Graphics g, SnowflakeState state);
    }
}
