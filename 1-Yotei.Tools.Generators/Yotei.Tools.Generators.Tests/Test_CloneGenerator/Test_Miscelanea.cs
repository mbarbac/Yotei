using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.Generators.Tests_CloneGenerator
{
    using IMiscelanea;
    using TMiscelanea;

    // ====================================================
    //[Enforced]
    public static partial class Test_Miscelanea
    {
        //[Enforced]
        [Fact]
        public static void Test_Concrete_Foo()
        {
            var source = new TOther.Foo("James");
            Assert.Equal("James", source.Name);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal(source.Name, target.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo()
        {
            IOther.IFoo<string> source = new TOther.Foo("James");
            Assert.Equal("James", source.Name);

            var target = source.Clone();
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal(source.Name, target.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_Bar()
        {
            var source = new TOther.Bar<int>("Bond", new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(source.MyPair, target.MyPair);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IBar_On_Bar()
        {
            IOther.IBar<int, string> source = new TOther.Bar<int>("Bond", new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IBar<int, string>>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(source.MyPair, target.MyPair);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo_On_Bar()
        {
            IOther.IFoo<string> source = new TOther.Bar<int>("Bond", new("Age", 50));
            Assert.Equal("Bond", source.Name);
            var bar = (IOther.IBar<int, string>)source;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.Equal(source.Name, target.Name);
            bar = (IOther.IBar<int, string>)target;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);
        }
    }

    // ====================================================
    namespace IMiscelanea
    {
        public partial interface IOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial interface IFoo<T>
            {
                string Name { get; }
            }

            // --------------------------------------------
            [Cloneable]
            public partial interface IBar<K, T> : IFoo<string>
            {
                TPair? MyPair { get; }
            }
        }
    }

    // ====================================================
    namespace TMiscelanea
    {
        public partial class TOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial class Foo : IOther.IFoo<string>
            {
                public Foo(string name) => Name = name;
                protected Foo(Foo source) => Name = source.Name;
                public override string ToString() => Name ?? string.Empty;

                public string Name { get; set; }
            }

            // --------------------------------------------
            [Cloneable]
            public partial class Bar<K> : Foo, IOther.IBar<K, string>
            {
                public Bar(string name, TPair? myPair) : base(name) => MyPair = myPair;
                protected Bar(Bar<K> source) : base(source) => MyPair = source.MyPair;
                public override string ToString() =>
                    (Name ?? "-") + (MyPair.HasValue
                    ? ($", {MyPair.Value.Key} = {MyPair.Value.Value}")
                    : string.Empty);

                public TPair? MyPair { get; set; }
            }
        }
    }
}