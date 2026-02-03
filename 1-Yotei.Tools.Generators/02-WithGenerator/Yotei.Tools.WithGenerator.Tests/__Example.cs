namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static class Test_Example
{
    public class TypeA
    {
        public TypeA(string? name) => Name = name;
        protected TypeA(TypeA source) => Name = source.Name;
        [With] public string? Name { get; init => field = value.NotNullNotEmpty(true); }
    }

    [InheritsWith]
    public partial class TypeB : TypeA
    {
        public TypeB(string? name) : base(name) { }
        protected TypeB(TypeB source) : base(source) { }
    }

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}