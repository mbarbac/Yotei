using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.HostInterface
{
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

    // ----------------------------------------------------

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
    
    // Adding and returning ICloneable...
    [Cloneable<ICloneable>]
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
            Assert.Equal(typeof(ICloneable), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

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
}