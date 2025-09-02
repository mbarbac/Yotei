#define ENABLEDX

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.HostConcrete
{
    public class Core
    {
        public Core(string name) => Name = name;
        protected Core(Core source) => Name = source.Name;
        public override string ToString() => Name ?? "-";
        public string Name { get; set; }
    }

    // ----------------------------------------------------
#if ENABLED
    // Nested elements...
    public static partial class TOther
    {
        [Cloneable]
        public partial class CType00 : Core
        {
            public CType00(string name) : base(name) { }
            protected CType00(CType00 source) : base(source) { }
        }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType00()
        {
            var type = typeof(TOther.CType00);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            var source = new TOther.CType00("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<TOther.CType00>(target);
            Assert.Equal(source.Name, target.Name);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Adding ICloneable...
    [Cloneable] public partial interface IFace01 : ICloneable { }
    [Cloneable]
    public partial class CType01 : Core, IFace01
    {
        public CType01(string name) : base(name) { }
        protected CType01(CType01 source) : base(source) { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType01()
        {
            var type = typeof(CType01);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));

            var source = new CType01("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<CType01>(target);
            Assert.Equal(source.Name, target.Name);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Using ReturnType with inheritance...
    [Cloneable<CType01>]
    public partial class CType02 : CType01
    {
        public CType02(string name) : base(name) { }
        protected CType02(CType02 source) : base(source) { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType02()
        {
            var type = typeof(CType02);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(CType01), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));

            var source = new CType02("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<CType02>(target); // Even if casted to CType01...
            Assert.Equal(source.Name, target.Name);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLEDX
    // Using VirtualMethod...
    [Cloneable(VirtualMethod = false)]
    public partial class CType03A : Core
    {
        public CType03A(string name) : base(name) { }
        protected CType03A(CType03A source) : base(source) { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType03A()
        {
            var type = typeof(CType03A);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            var source = new CType03A("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<CType03A>(target);
            Assert.Equal(source.Name, target.Name);
        }
    }

    [Cloneable(VirtualMethod = false)]
    public partial class CType03B : CType03A
    {
        public CType03B(string name) : base(name) { }
        protected CType03B(CType03B source) : base(source) { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType03B()
        {
            var type = typeof(CType03B);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            var source = new CType03B("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<CType03B>(target);
            Assert.Equal(source.Name, target.Name);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
#endif
}