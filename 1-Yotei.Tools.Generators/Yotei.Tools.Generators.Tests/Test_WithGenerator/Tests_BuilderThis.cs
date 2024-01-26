#pragma warning disable IDE0065

using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.Generators.Tests_WithGenerator
{
    using IBuilderThis;
    using TBuilderThis;

    // ====================================================
    //[Enforced]
    public static partial class Test_BuilderThis
    {
        //[Enforced]
        [Fact]
        public static void Test_Concrete_Foo()
        {
            var source = new TOther.Foo("James", 1);
            Assert.Equal("James", source.Name);
            Assert.Equal(1, source.Level);

            var target = source.WithName("Bond");
            Assert.Same(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal("Bond", target.Name); Assert.Equal("Bond", source.Name);

            target = source.WithLevel(5);
            Assert.Same(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal(5, target.Level); Assert.Equal(5, source.Level);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo()
        {
            IOther.IFoo<string> source = new TOther.Foo("James", 2);
            Assert.Equal("James", source.Name);
            Assert.Equal(2, ((TOther.Foo)source).Level);

            var target = source.WithName("Bond");
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.Same(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal("Bond", target.Name); Assert.Equal("Bond", source.Name);
            Assert.Equal(2, ((TOther.Foo)target).Level); Assert.Equal(2, ((TOther.Foo)source).Level);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_Bar()
        {
            var source = new TOther.Bar<int>("Bond", 2, new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal(2, source.Level);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.WithName("James");
            Assert.Same(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Equal("James", target.Name); Assert.Equal("James", source.Name);

            target = source.WithLevel(5);
            Assert.Same(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Equal(5, target.Level); Assert.Equal(5, source.Level);

            target = source.WithMyPair(null);
            Assert.Same(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Null(target.MyPair); Assert.Null(source.MyPair);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IBar_On_Bar()
        {
            IOther.IBar<int, string> source = new TOther.Bar<int>("Bond", 2, new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal(2, ((TOther.Bar<int>)source).Level);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.WithName("James");
            Assert.Same(source, target);
            Assert.IsAssignableFrom<IOther.IBar<int, string>>(target);
            Assert.Equal("James", target.Name); Assert.Equal("James", source.Name);
            Assert.Equal(2, ((TOther.Bar<int>)target).Level); Assert.Equal(2, ((TOther.Bar<int>)source).Level);

            target = source.WithMyPair(null);
            Assert.Same(source, target);
            Assert.IsAssignableFrom<IOther.IBar<int, string>>(target);
            Assert.Null(target.MyPair); Assert.Null(source.MyPair);
            Assert.Equal(2, ((TOther.Bar<int>)target).Level); Assert.Equal(2, ((TOther.Bar<int>)source).Level);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo_On_Bar()
        {
            IOther.IFoo<string> source = new TOther.Bar<int>("Bond", 2, new("Age", 50));
            Assert.Equal("Bond", source.Name);
            var bar = (IOther.IBar<int, string>)source;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);

            var target = source.WithName("James");
            Assert.Same(source, target);
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.Equal("James", target.Name); Assert.Equal("James", source.Name);
            Assert.Equal(2, ((TOther.Bar<int>)target).Level); Assert.Equal(2, ((TOther.Bar<int>)source).Level);

            bar = (IOther.IBar<int, string>)target;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);

            bar = (IOther.IBar<int, string>)source;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);
        }
    }

    // ====================================================
    namespace IBuilderThis
    {
        public partial interface IOther
        {
            // --------------------------------------------
            public partial interface IFoo<T>
            {
                [WithGenerator]
                string Name { get; }
            }

            // --------------------------------------------
            [WithGenerator]
            public partial interface IBar<K, T> : IFoo<string>
            {
                [WithGenerator]
                TPair? MyPair { get; }
            }
        }
    }

    // ====================================================
    namespace TBuilderThis
    {
        public partial class TOther
        {
            // --------------------------------------------
            [WithGenerator("this")]
            public partial class Foo : IOther.IFoo<string>
            {
                public Foo(string name, int level) { Name = name; Level = level; }
                protected Foo(Foo source) { Name = source.Name; Level = source.Level; }
                public override string ToString() => $"{Name}(Level)";

                [WithGenerator]
                public int Level;
                public string Name { get; set; }
            }

            // --------------------------------------------
            [WithGenerator]
            public partial class Bar<K> : Foo, IOther.IBar<K, string>
            {
                public Bar(string name, int level, TPair? myPair) : base(name, level) => MyPair = myPair;
                protected Bar(Bar<K> source) : base(source) => MyPair = source.MyPair;
                public override string ToString() =>
                    (base.ToString()) + (MyPair.HasValue
                    ? ($", {MyPair.Value.Key} = {MyPair.Value.Value}")
                    : string.Empty);

                public TPair? MyPair { get; set; }
            }
        }
    }
}