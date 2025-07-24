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

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType00()
        {
            var type = typeof(AType00);
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
        public static void Test_CType00()
        {
            var type = typeof(CType00);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }
}
/*

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
    public partial class Type10 : IFace1
    {
        public Type10() { }
        protected Type10(Type10 _) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type10()
        {
            var type = typeof(Type10);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// ERROR: Type11 requests return interface, but base type returns concrete type.
    /// </summary>
    //[Cloneable(ReturnInterface = true)]
    //public partial class Type11 : Type10
    //{
    //    public Type11() : base() { }
    //    protected Type11(Type11 source) : base(source) { }
    //}

    // ----------------------------------------------------

    /// <summary>
    /// ReturnInterface requested at base type.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class Type12 : IFace1
    {
        public Type12() { }
        protected Type12(Type12 _) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type12()
        {
            var type = typeof(Type12);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// ReturnInterface requested at derived type, but not 1st-level interface
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class Type13 : Type12
    {
        public Type13() : base() { }
        protected Type13(Type13 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type13()
        {
            var type = typeof(Type13);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
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
    public partial class Type14 : Type12, IFace1, IFace2
    {
        public Type14() : base() { }
        protected Type14(Type14 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type14()
        {
            var type = typeof(Type14);
            var method = type.GetMethod("Clone");
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
    /// ReturnInterface requested at derived type, with valid 1st-level interface. In this case
    /// IFace2 is the selected one as it appears first.
    /// </summary>
    [Cloneable(ReturnInterface = true)]
    public partial class Type15 : Type12, IFace2, IFace1
    {
        public Type15() : base() { }
        protected Type15(Type15 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type15()
        {
            var type = typeof(Type15);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace2), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }
    }
 */