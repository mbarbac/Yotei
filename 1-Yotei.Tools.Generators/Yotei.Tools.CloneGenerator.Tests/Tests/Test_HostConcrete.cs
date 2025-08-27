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

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    // Using VirtualMethod...
    [Cloneable(VirtualMethod = false)]
    public partial class CType03 : Core
    {
        public CType03(string name) : base(name) { }
        protected CType03(CType03 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType03()
        {
            var type = typeof(CType03);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            var source = new CType03("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<CType03>(target);
            Assert.Equal(source.Name, target.Name);
        }
    }

    [Cloneable(VirtualMethod = false)]
    public partial class CType04 : CType03
    {
        public CType04(string name) : base(name) { }
        protected CType04(CType04 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType04()
        {
            var type = typeof(CType04);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            var source = new CType04("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<CType04>(target);
            Assert.Equal(source.Name, target.Name);
        }
    }
}