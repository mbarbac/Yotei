//namespace Yotei.Tools.WithGenerator.Tests;

namespace Yotei.Tools
{
    namespace WithGenerator.Tests
    {
        public interface IPersona { [With<string>] string Name { get; } }
    }
}