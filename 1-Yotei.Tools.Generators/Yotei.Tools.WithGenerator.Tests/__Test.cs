namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static class Test_Example
{
    public interface IPersona { [With<string>] string Name { get; } }
    [InheritWiths<string>] public class Persona(string name) : IPersona { public string Name { get; } = name; }

    //[Enforced]
    [Fact]
    public static void Test_()
    {
    }
}