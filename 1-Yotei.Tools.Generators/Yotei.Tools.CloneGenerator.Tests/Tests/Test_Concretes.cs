using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Concretes
{
    /// <summary>
    /// For testing nested elements.
    /// </summary>
    public partial class TOther
    {
        /// <summary>
        /// Plain decoration.
        /// </summary>
        [Cloneable]
        public partial class Type01
        {
            public Type01() { }
            protected Type01(Type01 _) { }
        }

        /// <summary>
        /// Adding ICloneable. Adding PreventVirtual: method gets a 'new' modifier because it is
        /// not a virtual one, but overrides the previous Type01 one.
        /// </summary>
        [Cloneable(AddICloneable = true, PreventVirtual = true)]
        public partial class Type02 : Type01
        {
            public Type02() : base() { }
            protected Type02(Type02 source) : base(source) { }
        }

        ///// <summary>
        ///// ERROR: Cannot use ReturnInterface because Type03 inherits from Type01, whose method's
        //// return type is NOT an interface.
        ///// </summary>
        //[Cloneable(ReturnInterface = true)]
        //public partial class Type03 : Type01
        //{
        //    public Type03() : base() { }
        //    protected Type03(Type03 source) : base(source) { }
        //}
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type0s()
        {
            var type = typeof(TOther.Type01);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            type = typeof(TOther.Type02);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, HideBySig", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Plain decoration.
    /// </summary>
    [Cloneable]
    public partial interface IFace1 { }

    /// <summary>
    /// Plain decoration.
    /// </summary>
    [Cloneable]
    public partial class Type1A : IFace1
    {
        public Type1A() : base() { }
        protected Type1A(Type1A _) { }
    }

    /// <summary>
    /// Plain decoration. Adds ICloneable. Adds ReturnInterface.
    /// </summary>
    [Cloneable(AddICloneable = true, ReturnInterface = true)]
    public partial class Type1B : IFace1
    {
        public Type1B() : base() { }
        protected Type1B(Type1B _) { }
    }

    /// <summary>
    /// Inherits from Type1B. But not using ReturnInterface means return type is Type1C, not the
    /// inherited interface. Adds PreventVirtual: method gets a 'new' modifier.
    /// </summary>
    [Cloneable(PreventVirtual = true)]
    public partial class Type1C : Type1B
    {
        public Type1C() : base() { }
        protected Type1C(Type1C source) : base(source) { }
    }

    /// <summary>
    /// Inherits from Type1B. Even if using ReturnInterface, because there is no interface that
    /// Type1D directly implements, return type is the concrete one (note that ICloneable does
    /// not qualify).
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class Type1D : Type1B, ICloneable
    {
        public Type1D() : base() { }
        protected Type1D(Type1D source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type1s()
        {
            var type = typeof(Type1A);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            type = typeof(Type1B);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            type = typeof(Type1C);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, HideBySig", method.Attributes.ToString());

            type = typeof(Type1D);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }
}