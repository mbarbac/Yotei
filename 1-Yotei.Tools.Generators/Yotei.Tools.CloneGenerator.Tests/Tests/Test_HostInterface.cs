#define ENABLED

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.HostInterface
{
    // ----------------------------------------------------
#if ENABLED
    // Nested elements...
    public partial interface IOther
    {
        [Cloneable]
        public partial interface IFace00 { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace00()
        {
            var type = typeof(IOther.IFace00);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Adding ICloneable...
    [Cloneable]
    public partial interface IFace01A : ICloneable { }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace01A()
        {
            var type = typeof(IFace01A);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }

    // Adding and returning ICloneable (object!)...
    [Cloneable<object>]
    public partial interface IFace01B : ICloneable { }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace01B()
        {
            var type = typeof(IFace01B);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(object), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // VirtualMethod has no effect on interfaces...
    [Cloneable(VirtualMethod = false)]
    public partial interface IFace02 { }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace02()
        {
            var type = typeof(IFace02);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Simple inheritance...
    [Cloneable] public partial interface IFace03 { }
    [Cloneable] public partial interface IFace04 : IFace03 { }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace0304()
        {
            var type = typeof(IFace03);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));

            type = typeof(IFace04);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Complex inheritance...
    [Cloneable]
    public partial interface IFace05 : ICloneable { }

    [Cloneable(ReturnType = typeof(IsNullable<IFace05>))]
    public partial interface IFace06 : IFace05 { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace0506()
        {
            var type = typeof(IFace05);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));

            type = typeof(IFace06);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace05), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
#endif
}