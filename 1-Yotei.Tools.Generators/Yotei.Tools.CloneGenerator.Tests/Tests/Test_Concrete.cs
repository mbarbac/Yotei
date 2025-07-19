#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Concrete
{
    using IFaces;
    using TTypes;

    // ====================================================
    namespace IFaces
    {
        public partial interface IOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial interface IFaceA<T> { string Name { get; } }

            // --------------------------------------------
            [Cloneable(AddICloneable = true)]
            public partial interface IFaceB<T> : IFaceA<T> { }
        }
    }

    // ====================================================
    namespace TTypes
    {
        public partial class TOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial class TypeA1<T> : IOther.IFaceA<T>
            {
                public TypeA1(string name) => Name = name;
                protected TypeA1(TypeA1<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            [Cloneable(AddICloneable = true)]
            public partial class TypeB1<T> : TypeA1<T>, IOther.IFaceB<T>
            {
                public TypeB1(string name) : base(name) { }
                protected TypeB1(TypeB1<T> source) : base(source.Name) { }
            }

            [Cloneable(PreventVirtual = true)]
            public partial class TypeC1<T> : TypeB1<T>
            {
                public TypeC1(string name) : base(name) { }
                protected TypeC1(TypeC1<T> source) : base(source.Name) { }
            }

            // --------------------------------------------
            [Cloneable(ReturnInterface = true)]
            public partial class TypeA2<T> : IOther.IFaceA<T>
            {
                public TypeA2(string name) => Name = name;
                protected TypeA2(TypeA2<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            [Cloneable(ReturnInterface = true, AddICloneable = true)]
            public partial class TypeB2<T> : TypeA2<T>, IOther.IFaceB<T>
            {
                public TypeB2(string name) : base(name) { }
                protected TypeB2(TypeB2<T> source) : base(source.Name) { }
            }
        }
    }


    // ====================================================
    //[Enforced]
    public static class Test
    {
        //[Enforced]
        [Fact]
        public static void Test_TypeA1()
        {
            var type = typeof(TOther.TypeA1<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
            var done = type.GetInterfaces().Contains(typeof(ICloneable)); Assert.False(done);
        }

        //[Enforced]
        [Fact]
        public static void Test_TypeB1()
        {
            var type = typeof(TOther.TypeB1<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
            var done = type.GetInterfaces().Contains(typeof(ICloneable)); Assert.True(done);
        }

        //[Enforced]
        [Fact]
        public static void Test_TypeC1()
        {
            var type = typeof(TOther.TypeC1<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.Attributes.HasFlag(MethodAttributes.NewSlot));
            var done = type.GetInterfaces().Contains(typeof(ICloneable)); Assert.True(done);
        }

        //[Enforced]
        [Fact]
        public static void Test_TypeA2()
        {
            var type = typeof(TOther.TypeA2<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IOther.IFaceA<int>), method.ReturnType);
            Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
            var done = type.GetInterfaces().Contains(typeof(ICloneable)); Assert.False(done);
        }

        //[Enforced]
        [Fact]
        public static void Test_TypeB2()
        {
            var type = typeof(TOther.TypeB2<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IOther.IFaceB<int>), method.ReturnType);
            Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
            var done = type.GetInterfaces().Contains(typeof(ICloneable)); Assert.True(done);
        }
    }
}