/*using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Abstracts
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
        public abstract partial class AType01
        {
            public AType01() { }
            protected AType01(AType01 _) { }
        }
        
        /// <summary>
        /// Plain decoration. Adding ICloneable. Adding PreventVirtual: because it is an abstract
        /// one, it must get an 'override' modifier instead of a 'new' one.
        /// </summary>
        [Cloneable(AddICloneable = true, PreventVirtual = true)]
        public abstract partial class AType02 : AType01
        {
            public AType02() : base() { }
            protected AType02(AType02 source) : base(source) { }
        }

        /// <summary>
        /// ERROR: Cannot use ReturnInterface because Type03 inherits from Type01, whose method's
        /// return type is NOT an interface.
        /// </summary>
        //[Cloneable(ReturnInterface = true)]
        //public abstract partial class Type03 : Type01
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
            var type = typeof(TOther.AType01);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            type = typeof(TOther.AType02);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
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
    public abstract partial class AType1A : IFace1
    {
        public AType1A() : base() { }
        protected AType1A(AType1A _) { }
    }

    /// <summary>
    /// Plain decoration. Adds ICloneable.
    /// </summary>
    [Cloneable(AddICloneable = true)]
    public partial class CType1A : AType1A
    {
        public CType1A() : base() { }
        protected CType1A(CType1A source) : base(source) { }
    }

    /// <summary>
    /// ERROR: Cannot use ReturnInterface because Type03 inherits from Type01, whose method's
    /// return type is NOT an interface.
    /// </summary>
    //[Cloneable(ReturnInterface = true)]
    //public partial class CType1B : AType1A
    //{
    //    public CType1B() : base() { }
    //    protected CType1B(CType1B source) : base(source) { }
    //}

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type1As()
        {
            var type = typeof(AType1A);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            type = typeof(CType1A);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Plain decoration. Adding ReturnInterface.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public abstract partial class AType1B : IFace1
    {
        public AType1B() : base() { }
        protected AType1B(AType1B _) { }
    }

    /// <summary>
    /// Adding ReturnInterface: because no directly implemented interface, return type is the
    /// concrete one itself.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class CType1B : AType1B
    {
        public CType1B() : base() { }
        protected CType1B(CType1B _) { }
    }

    /// <summary>
    /// Adding ReturnInterface: using the direcly implemented interface.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class CType1C : AType1B, IFace1
    {
        public CType1C() : base() { }
        protected CType1C(CType1C source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type1Bs()
        {
            var type = typeof(AType1B);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            type = typeof(CType1B);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            type = typeof(CType1C);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig", method.Attributes.ToString());
        }
    }
}*/