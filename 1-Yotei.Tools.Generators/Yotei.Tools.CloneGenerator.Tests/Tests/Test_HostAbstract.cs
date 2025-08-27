using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.HostAbstract
{
    public abstract class Core
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
        public abstract partial class AType00 : Core
        {
            public AType00(string name) : base(name) { }
            protected AType00(AType00 source) : base(source) { }
        }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType00()
        {
            var type = typeof(TOther.AType00);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

    // Adding ICloneable...
    [Cloneable]
    public abstract partial class AType01A : Core, ICloneable
    {
        public AType01A(string name) : base(name) { }
        protected AType01A(AType01A source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType01A()
        {
            var type = typeof(AType01A);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }

    // Adding and returning ICloneable (object!)...
    [Cloneable<object>]
    public abstract partial class AType01B : Core, ICloneable
    {
        public AType01B(string name) : base(name) { }
        protected AType01B(AType01B source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType01B()
        {
            var type = typeof(AType01B);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(typeof(object), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

    // VirtualMethod has no effect on abstract classes...
    [Cloneable(VirtualMethod = false)]
    public abstract partial class AType02 : Core
    {
        public AType02(string name) : base(name) { }
        protected AType02(AType02 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType02()
        {
            var type = typeof(AType02);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

    // Inheritance...
    [Cloneable]
    public abstract partial class AType03 : Core
    {
        public AType03(string name) : base(name) { }
        protected AType03(AType03 source) : base(source) { }
    }

    [Cloneable(ReturnType = typeof(IsNullable<AType03>))]
    public abstract partial class AType04 : AType03
    {
        public AType04(string name) : base(name) { }
        protected AType04(AType04 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType0304()
        {
            var type = typeof(AType03);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            type = typeof(AType04);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(typeof(AType03), method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

    // Re-abstract...
    [Cloneable]
    public partial class Type05 : Core
    {
        public Type05(string name) : base(name) { }
        protected Type05(Type05 source) : base(source) { }
    }

    [Cloneable]
    public abstract partial class AType05 : Type05
    {
        public AType05(string name) : base(name) { }
        protected AType05(AType05 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType05()
        {
            var type = typeof(AType05);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }
}