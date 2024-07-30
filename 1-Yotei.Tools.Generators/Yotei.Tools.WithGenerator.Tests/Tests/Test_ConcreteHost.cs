#pragma warning disable IDE0065

namespace Yotei.Tools.WithGenerator.Tests.ConcreteHost
{
    using IFaces;
    using TTypes;

    // ====================================================
    namespace IFaces
    {
        public partial interface IOther
        {
            // ---------------------------------------------
            [InheritWiths]
            public partial interface IFoo<T>
            {
                [With] string Name { get; }
            }

            // ---------------------------------------------
            [InheritWiths]
            public partial interface IBar<K, T> : IFoo<T>
            {
                [With] string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace TTypes
    {
        public partial class TOther
        {
            // ---------------------------------------------
            [InheritWiths]
            public partial class Foo<T> : IOther.IFoo<T>
            {
                public Foo(string name, int age) { Name = name; Age = age; }
                protected Foo(Foo<T> source) : this(source.Name, source.Age) { }
                public override string ToString() => $"{Name ?? "-"} ({Age})";

                [With] public int Age;
                public string Name { get; set; }
            }

            // ---------------------------------------------
            [InheritWiths]
            public partial class Bar<K, T> : Foo<T>, IOther.IBar<K, T>
            {
                public Bar(string name, int age, string branch) : base(name, age) => Branch = branch;
                protected Bar(Bar<K, T> source) : base(source) => Branch = source.Branch;
                public override string ToString() => $"{base.ToString()}:{Branch ?? "-"}";
                public string Branch { get; set; }
            }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Foo()
        {
            var source = new TOther.Foo<int>("Bond", 50);
            var target = source.WithName("James");
            Assert.NotSame(source, target);
            Assert.Equal("James", target.Name);

            target = source.WithAge(55);
            Assert.NotSame(source, target);
            Assert.Equal(55, target.Age);
        }
    }
}
/*
        

        //[Enforced]
        [Fact]
        public static void Test_Bar()
        {
            var source = new TOther.Bar<int, string>("Bond", 50, "MI5");
            var target = source.WithName("James");
            Assert.NotSame(source, target);
            Assert.Equal("James", target.Name);

            target = source.WithAge(55);
            Assert.NotSame(source, target);
            Assert.Equal(55, target.Age);

            target = source.WithBranch("ARMY");
            Assert.NotSame(source, target);
            Assert.Equal("ARMY", target.Branch);
        }
 */