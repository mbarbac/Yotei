using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.CloneGenerator.Tests
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
    }
}