using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Abstracts
{
    /// <summary>
    /// Plain decoration.
    /// </summary>
    [Cloneable]
    public abstract partial class AType00
    {
        public AType00() { }
        protected AType00(AType00 _) { }
    }

    /// <summary>
    /// AddICloneable. PreventVirtual: method DOES NOT get a 'new' modifier because the base
    /// one it overrides is an abstract one, and C# insists in using 'override'
    /// </summary>
    [Cloneable(AddICloneable = true, PreventVirtual = true)]
    public partial class CType00 : AType00
    {
        public CType00() : base() { }
        protected CType00(CType00 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type00()
        {
            var type = typeof(AType00);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            type = typeof(CType00);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Plain decoration.
    /// </summary>
    [Cloneable]
    public partial interface IFace1 { }

    /// <summary>
    /// Cloneable and inherits interface.
    /// </summary>
    [Cloneable]
    public abstract partial class AType10 : IFace1
    {
        public AType10() { }
        protected AType10(AType10 _) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type10()
        {
            var type = typeof(AType10);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    /// <summary>
    /// ERROR: AType11 requests return interface, but base type returns concrete type.
    /// </summary>
    //[Cloneable(ReturnInterface = true)]
    //public partial class AType11 : AType10
    //{
    //    public AType11() : base() { }
    //    protected AType11(AType11 source) : base(source) { }
    //}

    // ----------------------------------------------------

    /// <summary>
    /// ReturnInterface requested at base type.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public abstract partial class AType12 : IFace1
    {
        public AType12() { }
        protected AType12(AType12 _) { }
    }

    /// <summary>
    /// ReturnInterface requested at base type.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public abstract partial class AType121 : AType12, IFace1
    {
        public AType121() : base() { }
        protected AType121(AType12 source) : base(source) { }
    }

    /// <summary>
    /// ReturnInterface requested at base type.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class CType121 : AType121, IFace1
    {
        public CType121() : base() { }
        protected CType121(CType121 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type12x()
        {
            var type = typeof(AType12);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            type = typeof(AType121);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, Abstract", method.Attributes.ToString());

            type = typeof(CType121);
            method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig", method.Attributes.ToString());
        }
    }


    // ----------------------------------------------------

    /// <summary>
    /// ReturnInterface requested at derived type, but not 1st-level interface
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public abstract partial class AType13 : AType12
    {
        public AType13() : base() { }
        protected AType13(AType13 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType13()
        {
            var type = typeof(AType13);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Plain decoration, inherited.
    /// </summary>
    [Cloneable]
    public partial interface IFace2 : IFace1 { }

    /// <summary>
    /// ReturnInterface requested at derived type, with valid 1st-level interface. Note that
    /// because IFace1 appears before IFace2, then it is selected even if it is less derived
    /// than the second one.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public abstract partial class AType14 : AType12, IFace1, IFace2
    {
        public AType14() : base() { }
        protected AType14(AType14 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type14()
        {
            var type = typeof(AType14);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// ReturnInterface requested at derived type, with valid 1st-level interface. In this case
    /// IFace2 is the selected one as it appears first.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public abstract partial class AType15 : AType12, IFace2, IFace1
    {
        public AType15() : base() { }
        protected AType15(AType15 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType15()
        {
            var type = typeof(AType15);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace2), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }
}