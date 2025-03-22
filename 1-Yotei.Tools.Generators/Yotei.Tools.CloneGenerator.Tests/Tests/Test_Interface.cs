#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Interfaces
{
    using IFaces;

    // ====================================================
    namespace IFaces
    {
        public partial interface IOther
        {
            // --------------------------------------------
            [Cloneable(AddICloneable = true)]
            public partial interface IA { }

            // --------------------------------------------
            [Cloneable(PreventVirtual = true)]
            public partial interface IB : IA { }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Test
    {
        //[Enforced]
        [Fact]
        public static void Test_IA_Methods()
        {
            var type = typeof(IOther.IA);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
        }

        //[Enforced]
        [Fact]
        public static void Test_IB_Methods()
        {
            var type = typeof(IOther.IB);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual); // PreventVirtual has no effect on interfaces
        }
    }
}