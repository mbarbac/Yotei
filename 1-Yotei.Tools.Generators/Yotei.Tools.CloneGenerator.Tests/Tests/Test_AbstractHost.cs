/*

#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Abstracts
{
    using IFaces;
    using TTypes;

    // ====================================================
    namespace IFaces
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
            public partial interface IBar<K, T> : IFoo<T>
            {
                string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace TTypes
    {
        public partial class TOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial class ConcreteFoo<T> : IOther.IFoo<T>
            {
                public ConcreteFoo(string name) => Name = name;
                protected ConcreteFoo(ConcreteFoo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Test_Interface
    {
        //[Enforced]
        [Fact]
        public static void Test() { }
    }
}

*/