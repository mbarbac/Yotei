using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.Concretes
{
    /// <summary>
    /// Plain decoration.
    /// </summary>
    public partial class Type00
    {
        public Type00(string name, int age) { Name = name; Age = age; }
        protected Type00(Type00 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name ?? "-"}, {Age}";

        [With] public string Name { get; set; }
        [With] public int Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type00()
        {
            var type = typeof(Type00);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type00("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            Assert.Equal("ABC", target.Name);
            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Plain inheritance.
    /// </summary>
    [InheritWiths]
    public partial class Type01 : Type00
    {
        public Type01(string name, int age) : base(name, age) { }
        protected Type01(Type01 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type01()
        {
            var type = typeof(Type01);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type01("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            Assert.Equal("ABC", target.Name);
            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Plain interface.
    /// </summary>
    public partial interface IFace1 { [With] string Name { get; } }

    // <summary>
    /// Inherits interface with property 'Name'. Note that field 'Age' cannot appear in 'IFace1'
    /// because it is a field, and fields are not supported in interfaces.
    /// </summary>
    [InheritWiths]
    public partial class Type10 : IFace1
    {
        public Type10(string name, int age) { Name = name; Age = age; }
        protected Type10(Type10 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name ?? "-"}, {Age}";

        [With] public string Name { get; set; }
        [With] public int Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type10()
        {
            var type = typeof(Type10);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type10("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// In this case, as we are implementing the interface using 'With' attributes, 'InheritWiths'
    /// is redundant. In addition, note that 'ReturnInterface' is ignored on 'Age' because it is
    /// a field, and fields cannot appear on interfaces.
    /// </summary>
    public partial class Type20 : IFace1
    {
        public Type20(string name, int age) { Name = name; Age = age; }
        protected Type20(Type20 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name ?? "-"}, {Age}";

        [With(ReturnInterface = true)] public virtual string Name { get; set; }
        [With(ReturnInterface = true)] public int Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type20()
        {
            var type = typeof(Type20);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type20("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            //target = source.WithAge(100);      - target is of type 'IFace1', so it has not a
            //Assert.NotSame(source, target);   - member named 'Age'. So, we need to use another
            //Assert.Equal(100, target.Age);    - variable to prevent cast errors.

            var concrete = source.WithAge(100);
            Assert.NotSame(source, concrete);
            Assert.Equal(100, concrete.Age);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Inherit withs but host does not directly implement interface means that return types
    /// will be the host itself.
    /// </summary>
    [InheritWiths]
    public partial class Type21 : Type20
    {
        public Type21(string name, int age) : base(name, age) { }
        protected Type21(Type21 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type21()
        {
            var type = typeof(Type21);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type21("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Inherit with the host directly implementing the interface permits the methods to use that
    /// interface as the return type.
    /// </summary>
    [InheritWiths]
    public partial class Type22 : Type20, IFace1
    {
        public Type22(string name, int age) : base(name, age) { }
        protected Type22(Type22 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type22()
        {
            var type = typeof(Type22);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type22("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            //target = source.WithAge(100);
            //Assert.NotSame(source, target);
            //Assert.Equal(100, target.Age);
            var concrete = source.WithAge(100);
            Assert.NotSame(source, concrete);
            Assert.Equal(100, concrete.Age);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reimplementing the members to modify their with settings. Note that this only works
    /// with 'Name' because only properties can be made virtual.
    /// </summary>
    [InheritWiths]
    public partial class Type23 : Type22, IFace1
    {
        public Type23(string name, int age) : base(name, age) { }
        protected Type23(Type23 source) : base(source) { }

        [With(PreventVirtual = true)]
        public override string Name
        {
            get => base.Name;
            set => base.Name = value;
        }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Type23()
        {
            var type = typeof(Type23);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.False(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, HideBySig", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask", method.Attributes.ToString());

            var source = new Type23("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }
}