using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.Interfaces
{
    /// <summary>
    /// For testing nested elements.
    /// </summary>
    public partial interface IOther
    {
        /// <summary>
        /// Plain decoration.
        /// </summary>
        public partial interface IFace0
        {
            [With] string Name { get; }
        }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace0()
        {
            var type = typeof(IOther.IFace0);
            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// PreventVirtual and ReturnInterface have no effect on interfaces.
    /// </summary>
    public partial interface IFace1
    {
        [With(PreventVirtual = true, ReturnInterface = true)]
        string Name { get; }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace1()
        {
            var type = typeof(IFace1);
            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Interface inheritance.
    /// </summary>
    [InheritWiths]
    public partial interface IFace2 : IFace1 { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace2()
        {
            var type = typeof(IFace2);
            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }
}