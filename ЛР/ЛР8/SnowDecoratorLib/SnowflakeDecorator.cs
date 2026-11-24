using System.Drawing;

namespace SnowDecoratorLib
{
    // Абстрактний базовий декоратор (Decorator)
    public abstract class SnowflakeDecorator : ISnowflakeRenderer
    {
        protected readonly ISnowflakeRenderer _wrapped;

        protected SnowflakeDecorator(ISnowflakeRenderer inner)
        {
            _wrapped = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public virtual string Description => _wrapped.Description;

        public virtual void Draw(Graphics g, SnowflakeState state)
        {
            _wrapped.Draw(g, state);
        }
    }
}
