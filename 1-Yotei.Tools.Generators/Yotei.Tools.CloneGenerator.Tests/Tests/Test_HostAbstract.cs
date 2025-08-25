#pragma warning disable IDE0079
#pragma warning disable Cloneable01 // No interface found for type

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.HostAbstract
{
    public abstract class Core
    {
        public Core(string name) => Name = name;
        protected Core(Core source) => Name = source.Name;
        public override string ToString() => Name ?? "-";
        public string Name { get; set; }
    }
    
    // ----------------------------------------------------
    /*
    // Nested elements...
    public static partial class TOther
    {
        [Cloneable]
        public abstract partial class AType00 : Core
        {
            public AType00(string name) : base(name) { }
            protected AType00(AType00 source) : base(source) { }
        }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType00()
        {
            var type = typeof(TOther.AType00);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }
    */
    // ----------------------------------------------------
    /*
    // Adding ICloneable explicitly...
    [Cloneable]
    public abstract partial class AType01 : Core, ICloneable
    {
        public AType01(string name) : base(name) { }
        protected AType01(AType01 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType01()
        {
            var type = typeof(AType01);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }
    */
    // ----------------------------------------------------
    /*
    // Adding ICloneable through decoration...
    [Cloneable(AddICloneable = true)]
    public abstract partial class AType02 : Core
    {
        public AType02(string name) : base(name) { }
        protected AType02(AType02 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType02()
        {
            var type = typeof(AType02);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.NotNull(type.GetInterface("ICloneable"));
        }
    }
    */
    // ----------------------------------------------------

    // Returns interface by default...
    [Cloneable]
    public partial interface IFace03 { }
    /*
    [Cloneable]
    public abstract partial class AType03 : Core, IFace03
    {
        public AType03(string name) : base(name) { }
        protected AType03(AType03 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType03()
        {
            var type = typeof(AType03);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace03), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }*/

    // ----------------------------------------------------

    // Unless explicitly prevented...
    [Cloneable]
    public partial interface IFace04 : IFace03{ }

    [Cloneable(ReturnsDecorated = true)]
    public abstract partial class AType04 : Core, IFace04
    {
        public AType04(string name) : base(name) { }
        protected AType04(AType04 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType04()
        {
            var type = typeof(AType04);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }

    /*
     // ----------------------------------------------------

    // Shall not compile because IFace04 is not decorated when IFace03 is...
    //public partial interface IFace04 : IFace03 { }
    //[Cloneable]
    //public abstract partial class AType04 : IFace04 { }

    // ----------------------------------------------------

    // Prevent virtual has no effect on abstract types...
    [Cloneable(PreventVirtual = true)]
    public abstract partial class AType05
    {
        public AType05(string name) => Name = name;
        protected AType05(AType05 source) => Name = source.Name;
        public override string ToString() => Name ?? "-";
        public string Name { get; set; }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType05()
        {
            var type = typeof(AType05);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Null(type.GetInterface("ICloneable"));
        }
    }
     */
}