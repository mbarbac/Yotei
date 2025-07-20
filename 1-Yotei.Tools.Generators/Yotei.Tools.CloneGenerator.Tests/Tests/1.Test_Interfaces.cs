#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests
{
    // ========================================================
    namespace IFaces
    {
        // Plain cloneable interface...
        public partial interface IOther
        {
            [Cloneable]
            public partial interface IFaceA { string Name { get; } }
        }

        // Cloneable inheritance, and adding ICloneable...
        public partial interface IOther
        {
            [Cloneable(AddICloneable = true)]
            public partial interface IFaceB : IFaceA { }
        }

        // PreventVirtual, ReturnInterface, are ignored on interfaces...
        public partial interface IOther
        {
            [Cloneable(PreventVirtual = true, ReturnInterface = true)]
            public partial interface IFaceC : IFaceA { }
        }
    }
}

namespace Yotei.Tools.CloneGenerator.Tests
{
    using IFaces;

    // ====================================================
    //[Enforced]
    public static class Test_Cloneable_Interfaces
    {
        //[Enforced]
        [Fact]
        public static void Test_IFaceA()
        {
            var type = typeof(IOther.IFaceA);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_IFaceB()
        {
            var type = typeof(IOther.IFaceB);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_IFaceC()
        {
            var type = typeof(IOther.IFaceC);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }
}