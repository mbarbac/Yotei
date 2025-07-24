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
}

/*

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace0()
        {
            var type = typeof(IOther.IFace0);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adding ICloneable interface directly.
    /// </summary>
    [Cloneable]
    public partial interface IFace2 : ICloneable { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace1()
        {
            var type = typeof(IFace2);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adding ICloneable interface through decoration.
    /// </summary>
    [Cloneable(AddICloneable = true)]
    public partial interface IFace3 : IOther.IFace0 { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace3()
        {
            var type = typeof(IFace3);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// PreventVirtual and ReturnInterface have no effect on interfaces.
    /// </summary>
    [Cloneable(PreventVirtual = true, ReturnInterface = true)]
    public partial interface IFace4 : IFace3 { }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace4()
        {
            var type = typeof(IFace4);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }
}
 */