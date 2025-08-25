/*using static Yotei.Tools.Diagnostics.ConsoleEx;
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

    // Adding ICloneable explicitly...
    [Cloneable]
    public partial interface IFace01 : ICloneable { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace01()
        {
            var type = typeof(IFace01);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

    // Adding ICloneable through decoration...
    [Cloneable(AddICloneable = true)]
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
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }

    // ----------------------------------------------------

    // Inheritance.
    // PreventVirtual no effect on interfaces...
    [Cloneable(PreventVirtual = true)]
    public partial interface IFace03 : IFace02 { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace03()
        {
            var type = typeof(IFace02);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }
}*/