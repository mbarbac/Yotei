namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Example
{
    public class TypeA<T>
    {
        public TypeA(string? name) => Name = name;
        protected TypeA(TypeA<T> source) => Name = source.Name;
        [With] public string? Name { get; init => field = value.NotNullNotEmpty(true); }
    }

    [InheritsWith]
    public partial class TypeB<T> : TypeA<T>
    {
        public TypeB(string? name) : base(name) { }
        protected TypeB(TypeB<T> source) : base(source) { }
    }

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}