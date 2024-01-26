#pragma warning disable IDE0065

using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.Generators.Tests_WithGenerator
{
    using IBuilderBase;
    using TBuilderBase;

    // ====================================================
    //[Enforced]
    public static partial class Test_BuilderBase
    {
        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new TOther.Bar<int>("Bond", new("Age", 50));
            var target = source.WithName("James");
            Assert.Same(source, target);
            Assert.Equal("James", target.Name); Assert.Equal("James", source.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IBar<int, string> source = new TOther.Bar<int>("Bond", new("Age", 50));
            var target = source.WithName("James");
            Assert.Same(source, target);
            Assert.Equal("James", target.Name); Assert.Equal("James", source.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_No_Base_Method()
        {
            // This test must fail because in 'Bar' we are requesting, at type level, to use base
            // methods, and it happens that the 'WithBranch' base method creates, by design, a new
            // instance of 'Foo' that, obviously, cannot be casted to 'Bar'...

            var source = new TOther.Bar<int>("Bond", new("Age", 50));
            try { _ = source.WithBranch("Any"); Assert.Fail(); }
            catch (InvalidCastException) { }
        }
    }

    // ====================================================
    namespace IBuilderBase
    {
        public partial interface IOther
        {
            // --------------------------------------------
            public partial interface IFoo<T>
            {
                [WithGenerator]
                string Name { get; }

                [WithGenerator]
                string Branch { get; }
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
    namespace TBuilderBase
    {
        public partial class TOther
        {
            // --------------------------------------------
            [WithGenerator]
            public partial class Foo : IOther.IFoo<string>
            {
                public Foo(string name) => Name = name;
                protected Foo(Foo source) => Name = source.Name;
                public override string ToString() => $"{Name}";

                [WithGenerator("this")]
                public string Name { get; set; }

                public string Branch { get; set; }
            }

            // --------------------------------------------
            [WithGenerator("base")]
            public partial class Bar<K> : Foo, IOther.IBar<K, string>
            {
                public Bar(string name, TPair? myPair) : base(name) => MyPair = myPair;
                protected Bar(Bar<K> source) : base(source) => MyPair = source.MyPair;
                public override string ToString() =>
                    (base.ToString()) + (MyPair.HasValue
                    ? ($", {MyPair.Value.Key} = {MyPair.Value.Value}")
                    : string.Empty);

                [WithGenerator("copy")] // To prevent a "base" that doesn't exist...
                public TPair? MyPair { get; set; }
            }
        }
    }
}