/*using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Concretes
{
    /// <summary>
    /// Plain decoration.
    /// </summary>
    [Cloneable]
    public partial class Type00
    {
        public Type00() { }
        protected Type00(Type00 _) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type00()
        {
            var type = typeof(Type00);
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
    /// AddICloneable. PreventVirtual: method gets a 'new' modifier because it is not a virtual
    /// one that overrides and existing base one.
    /// </summary>
    [Cloneable(AddICloneable = true, PreventVirtual = true)]
    public partial class Type01 : Type00
    {
        public Type01() : base() { }
        protected Type01(Type01 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type01()
        {
            var type = typeof(Type01);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
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
}*/