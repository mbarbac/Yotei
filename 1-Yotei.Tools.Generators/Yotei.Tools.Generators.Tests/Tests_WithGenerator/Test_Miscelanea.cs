using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.Generators.Tests.WithGenerator
{
    using IMiscelanea;
    using TMiscelanea;

    // ====================================================
    [Enforced]
    public static partial class Test_Miscelanea
    {
        //[Enforced]
        [Fact]
        public static void Test_Concrete_Foo()
        {
            var source = new TOther.Foo("James");
            Assert.Equal("James", source.Name);

            var target = source.WithName("Bond");
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal("Bond", target.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo()
        {
            IOther.IFoo<string> source = new TOther.Foo("James");
            Assert.Equal("James", source.Name);

            var target = source.WithName("Bond");
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal("Bond", target.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_Bar()
        {
            var source = new TOther.Bar<int>("Bond", new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.WithName("James");
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Equal("James", target.Name);

            target = source.WithMyPair(null);
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Null(target.MyPair);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IBar_On_Bar()
        {
            IOther.IBar<int, string> source = new TOther.Bar<int>("Bond", new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.WithName("James");
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IBar<int, string>>(target);
            Assert.Equal("James", target.Name);

            target = source.WithMyPair(null);
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IBar<int, string>>(target);
            Assert.Null(target.MyPair);
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

            var target = source.WithName("James");
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.Equal("James", target.Name);
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
    namespace TMiscelanea
    {
        public partial class TOther
        {
            // --------------------------------------------
            [WithGenerator]
            public partial class Foo : IOther.IFoo<string>
            {
                public Foo(string name) => Name = name;
                protected Foo(Foo source) => Name = source.Name;
                public override string ToString() => Name ?? string.Empty;

                public string Name { get; set; }
            }

            // --------------------------------------------
            [WithGenerator]
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