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
            [Cloneable(AddICloneable = true)]
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

            // --------------------------------------------
            [Cloneable]
            public abstract partial class AbstractFromConcrete<K, T> : ConcreteFoo<T>, IOther.IBar<K, T>
            {
                public AbstractFromConcrete(string name, string branch) : base(name) => Branch = branch;
                protected AbstractFromConcrete(AbstractFromConcrete<K, T> source) : this(source.Name, source.Branch) { }
                public override string ToString() => $"{(Name ?? "-")}, {(Branch ?? "-")}";
                public string Branch { get; set; }
            }

            // --------------------------------------------
            //[Cloneable]
            //public abstract partial class AbstractFoo<T> : IOther.IFoo<T>
            //{
            //    public AbstractFoo(string name) => Name = name;
            //    protected AbstractFoo(AbstractFoo<T> source) : this(source.Name) { }
            //    public override string ToString() => Name ?? "-";
            //    public string Name { get; set; }
            //}

            // --------------------------------------------
            //[Cloneable]
            //public abstract partial class AbstractFromAbstract<K, T> : ConcreteFoo<T>, IOther.IBar<K, T>
            //{
            //    public AbstractFromAbstract(string name, string branch) : base(name) => Branch = branch;
            //    protected AbstractFromAbstract(AbstractFromAbstract<K, T> source) : this(source.Name, source.Branch) { }
            //    public override string ToString() => $"{(Name ?? "-")}, {(Branch ?? "-")}";
            //    public string Branch { get; set; }
            //}
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