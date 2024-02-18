#pragma warning disable IDE0065

using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.CloneGenerator.Tests.Miscelanea
{
    using IMiscelanea;
    using TMiscelanea;

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
            public partial interface IBar<T, K> : IFoo<T>
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
            public partial class Foo : IOther.IFoo<string?>
            {
                public Foo(string name, int level) { Name = name; Level = level; }
                protected Foo(Foo source) { Name = source.Name; Level = source.Level; }
                public override string ToString() => $"{Name}({Level})";

                public string Name { get; set; }
                public int Level;
            }

            // --------------------------------------------
            [Cloneable]
            public partial class Bar<K> : Foo, IOther.IBar<K, string?>
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

    // ====================================================
    //[Enforced]
    public static partial class Test_Miscelanea
    {
        //[Enforced]
        [Fact]
        public static void Test_Concrete_Foo()
        {
            var source = new TOther.Foo("James", 1);
            Assert.Equal("James", source.Name);
            Assert.Equal(1, source.Level);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(source.Level, target.Level);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo()
        {
            IOther.IFoo<string?> source = new TOther.Foo("James", 1);
            Assert.Equal("James", source.Name);
            Assert.Equal(1, ((TOther.Foo)source).Level);

            var target = source.Clone();
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Foo>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(((TOther.Foo)source).Level, ((TOther.Foo)target).Level);
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

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<TOther.Bar<int>>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(source.Level, target.Level);
            Assert.Equal(source.MyPair, target.MyPair);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IBar_On_Bar()
        {
            IOther.IBar<int, string?> source = new TOther.Bar<int>("Bond", 2, new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal(2, ((TOther.Bar<int>)source).Level);
            Assert.Equal("Age", source.MyPair!.Value.Key);
            Assert.Equal(50, source.MyPair!.Value.Value);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IBar<int, string>>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(((TOther.Bar<int>)source).Level, ((TOther.Bar<int>)target).Level);
            Assert.Equal(source.MyPair, target.MyPair);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface_IFoo_On_Bar()
        {
            IOther.IFoo<string?> source = new TOther.Bar<int>("Bond", 2, new("Age", 50));
            Assert.Equal("Bond", source.Name);
            Assert.Equal(2, ((TOther.Bar<int>)source).Level);
            var bar = (IOther.IBar<int, string>)source;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IFoo<string>>(target);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(2, ((TOther.Bar<int>)target).Level);
            bar = (IOther.IBar<int, string>)target;
            Assert.Equal("Age", bar.MyPair!.Value.Key);
            Assert.Equal(50, bar.MyPair!.Value.Value);
        }
    }
}