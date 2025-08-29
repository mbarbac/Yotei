#define ENABLED

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.HostConcrete
{
    // ----------------------------------------------------
#if ENABLED
    // Nested elements...
    public static partial class TOther
    {
        public partial class CType00
        {
            public CType00(string name, int age) { Name = name; Age = age; }
            protected CType00(CType00 source) { Name = source.Name; Age = source.Age; }
            public override string ToString() => $"{Name}, {Age}";
            [With] public string Name { get; set; }
            [With] public int Age;
        }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType00()
        {
            var type = typeof(TOther.CType00);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // VirtualMethod...
    public partial class CType01
    {
        public CType01(string name, int age) { Name = name; Age = age; }
        protected CType01(CType01 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name}, {Age}";
        [With(VirtualMethod = false)] public string Name { get; set; }
        [With(VirtualMethod = false)] public int Age;
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType01()
        {
            var type = typeof(CType01);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // ReturnType...
    public partial interface IFace02
    {
        [With] public string Name { get; set; }
    }
    public partial class CType02 : IFace02
    {
        public CType02(string name, int age) { Name = name; Age = age; }
        protected CType02(CType02 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name}, {Age}";
        [With<IFace02>] public string Name { get; set; }
        [With(ReturnType = typeof(IFace02))] public int Age;
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType02()
        {
            var type = typeof(CType02);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace02), method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace02), method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Changing member type...
    public partial interface IFace03
    {
        [With] public int Age { get; set; }
    }
    [InheritWiths]
    public abstract partial class CType03 : IFace03
    {
        public CType03(int age) { Age = age; }
        protected CType03(CType03 source) { Age = source.Age; }
        public override string ToString() => $"{Age}";

        [With] public long Age { get; set; }
        int IFace03.Age { get => (int)Age; set => Age = value; }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType03()
        {
            var type = typeof(CType03);

            var method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(long), method.GetParameters()[0].ParameterType);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Inheritance...
    public partial class CType04A
    {
        public CType04A(string name, int age) { Name = name; Age = age; }
        protected CType04A(CType04A source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name}, {Age}";
        [With] public string Name { get; set; }
        [With] public int Age;
    }
    [InheritWiths]
    public partial class CType04B : CType04A
    {
        public CType04B(string name, int age) : base(name, age) { }
        protected CType04B(CType04B source) : base(source) { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType04()
        {
            var type = typeof(CType04B);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
    // Changing type
    public partial interface IFace06
    {
        [With] string Name { get; set; }
    }
    public partial class CType06A : IFace06
    {
        public CType06A(string name, int age) { Name = name; Age = age; }
        protected CType06A(CType06A source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name}, {Age}";
        [With] public string Name { get; set; }
        [With] public int Age;
    }
    [InheritWiths<CType06A>]
    public partial class CType06B : CType06A
    {
        public CType06B(string name, int age) : base(name, age) { }
        protected CType06B(CType06B source) : base(source) { }
    }
    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType06()
        {
            var type = typeof(CType06B);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(CType06A), method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(CType06A), method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);

            var source = new CType06B("James", 50);
            var target = source.WithName("Bond");
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);

            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }
#endif

    // ----------------------------------------------------
#if ENABLED
#endif
}