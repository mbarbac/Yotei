#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests
{
    using IFaces;

    // ========================================================
    namespace TTypes
    {
        // Plain class with no iface inheritance...
        public partial class TOther
        {
            [Cloneable]
            public partial class Type0
            {
                public Type0(string name) => Name = name;
                protected Type0(Type0 source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // Plain class from plain interface...
            [Cloneable]
            public partial class TypeA<T> : IOther.IFaceA
            {
                public TypeA(string name) => Name = name;
                protected TypeA(TypeA<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // Cannot use 'ReturnInterface' because we are returning an interface overriding a
            // concrete return type, which is a C# syntax error...
            //[Cloneable(ReturnInterface = true)]
            //public partial class TypeA1<T> : TypeA<T>, IOther.IFaceB
            //{
            //    public TypeA1(string name) : base(name) { }
            //    protected TypeA1(TypeA1<T> source) : this(source.Name) { }
            //}

            // Plain class from plain interface, with 'ReturnInterface'...
            [Cloneable(ReturnInterface = true)]
            public partial class TypeB<T> : IOther.IFaceA
            {
                public TypeB(string name) => Name = name;
                protected TypeB(TypeB<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // The return type is the concrete host because not directly interface listed.
            [Cloneable(ReturnInterface = true)]
            public partial class TypeB2<T> : TypeB<T>
            {
                public TypeB2(string name) : base(name) { }
                protected TypeB2(TypeB2<T> source) : this(source.Name) { }
            }

            // ReturnInterface works because overriding return type being an interface.
            // The return type is IFaceA because is the first one listed.
            [Cloneable(ReturnInterface = true)]
            public partial class TypeB3<T> : TypeB<T>, IOther.IFaceA, IOther.IFaceB
            {
                public TypeB3(string name) : base(name) { }
                protected TypeB3(TypeB3<T> source) : this(source.Name) { }
            }

            // ReturnInterface works because overriding return type being an interface.
            // The return type is IFaceB.
            [Cloneable(ReturnInterface = true)]
            public partial class TypeB4<T> : TypeB<T>, IOther.IFaceB
            {
                public TypeB4(string name) : base(name) { }
                protected TypeB4(TypeB4<T> source) : this(source.Name) { }
            }

            // Prevent virtual provoke a 'new' modifier...
            [Cloneable(PreventVirtual = true)]
            public partial class TypeC<T> : TypeB<T>
            {
                public TypeC(string name) : base(name) { }
                protected TypeC(TypeC<T> source) : this(source.Name) { }
            }
        }
    }
}

namespace Yotei.Tools.CloneGenerator.Tests
{
    using IFaces;
    using TTypes;

    // ====================================================
    //[Enforced]
    public static partial class Test_Cloneable_Concretes
    {
        //[Enforced]
        [Fact]
        public static void Test_Concrete_Type0()
        {
            var type = typeof(TOther.Type0);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_TypeA()
        {
            var type = typeof(TOther.TypeA<int>);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_TypeB()
        {
            var type = typeof(TOther.TypeB<int>);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IOther.IFaceA), method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_TypeB2()
        {
            var type = typeof(TOther.TypeB2<int>);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_TypeB3()
        {
            var type = typeof(TOther.TypeB3<int>);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IOther.IFaceA), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));

            // No NewSlot (VtableLayoutMask): IFaceA is less concrete than IFaceB...
            Assert.Equal("Public, Virtual, HideBySig", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_TypeB4()
        {
            var type = typeof(TOther.TypeB4<int>);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IOther.IFaceB), method.ReturnType);
            Assert.NotNull(type.GetInterface("ICloneable"));
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete_TypeC()
        {
            var type = typeof(TOther.TypeC<int>);
            var method = type.GetMethod("Clone");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Null(type.GetInterface("ICloneable"));
            Assert.Equal("Public, HideBySig", method.Attributes.ToString());
        }
    }
}