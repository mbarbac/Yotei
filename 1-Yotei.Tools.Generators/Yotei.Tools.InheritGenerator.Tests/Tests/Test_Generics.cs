namespace Yotei.Tools.InheritGenerator.Tests
{
    // ====================================================
    
    public interface IFoo<T>
    {
        int Count { get; }
        IFoo<T> Add(T item);
    }

    public class Foo<T> : IFoo<T>
    {
        readonly List<T> Items = [];

        public int Count => Items.Count;
        public Foo<T> Add(T item) { Items.Add(item); return this; }
        IFoo<T> IFoo<T>.Add(T item) => Add(item);
    }

    // ====================================================

    [Inherit<IFoo<IGeneric>>("T")]
    public partial interface IBar<T> { }

    [Inherit<IFoo<IGeneric>>("T")]
    [Inherit<Foo<IGeneric>>("T")]
    public partial class Bar<T> { }

    // ====================================================
    //[Enforced]
    public static class Test_Generics
    {
        //[Enforced]
        [Fact]
        public static void Test_()
        {
        }
    }
}