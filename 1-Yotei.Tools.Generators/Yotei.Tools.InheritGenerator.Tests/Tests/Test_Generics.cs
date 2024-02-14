namespace Yotei.Tools.InheritGenerator.Tests
{
    // ====================================================
    
    public interface IFoo<T>
    {
        int Count { get; }
        IFoo<T> Add(T item);
        IFoo<T> Me { get; }
    }

    public class Foo<T> : IFoo<T>
    {
        readonly List<T> Items = [];

        public int Count => Items.Count;
        public virtual Foo<T> Add(T item) { Items.Add(item); return this; }
        IFoo<T> IFoo<T>.Add(T item) => Add(item);

        public virtual Foo<T> Me { get; init; } = default!;
        IFoo<T> IFoo<T>.Me => Me;
    }

    // ====================================================

    //[Inherit<IFoo<IGeneric>>("T")]
    public partial interface IBar<T> : IFoo<T>
    {
        new IBar<T> Add(T item);
        new IBar<T> Me { get; }
    }

    //[Inherit<IBar<IGeneric>>("T")]
    //[Inherit<Foo<IGeneric>>("T")]
    public partial class Bar<T> : Foo<T>, IBar<T>
    {
        public override Bar<T> Add(T item) => (Bar<T>)base.Add(item);
        IBar<T> IBar<T>.Add(T item) => Add(item);

        IBar<T> IBar<T>.Me
        {
            get => (IBar<T>)base.Me;
        }
    }

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