using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.Abstracts
{
    /// <summary>
    /// Plain decoration.
    /// </summary>
    public abstract partial class AType00
    {
        public AType00(string name, int age) { Name = name; Age = age; }
        protected AType00(AType00 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name ?? "-"}, {Age}";

        [With] public string Name { get; set; }
        [With] public int Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType00()
        {
            var type = typeof(AType00);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// Plain inheritance.
    /// </summary>
    [InheritWiths]
    public partial class CType01 : AType00
    {
        public CType01(string name, int age) : base(name, age) { }
        protected CType01(CType01 source) : base(source) { }
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

            var source = new CType01("XYZ", 50);
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
    public abstract partial class AType10 : IFace1
    {
        public AType10(string name, int age) { Name = name; Age = age; }
        protected AType10(AType10 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name ?? "-"}, {Age}";

        [With] public string Name { get; set; }
        [With] public int Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType10()
        {
            var type = typeof(AType10);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    // <summary>
    /// Inherits interface with property 'Name'. Note that field 'Age' cannot appear in 'IFace1'
    /// because it is a field, and fields are not supported in interfaces.
    /// </summary>
    [InheritWiths]
    public partial class CType10 : AType10
    {
        public CType10(string name, int age) : base(name, age) { }
        protected CType10(CType10 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType10()
        {
            var type = typeof(CType10);

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

            var source = new CType10("XYZ", 50);
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
    public abstract partial class AType20 : IFace1
    {
        public AType20(string name, int age) { Name = name; Age = age; }
        protected AType20(AType20 source) { Name = source.Name; Age = source.Age; }
        public override string ToString() => $"{Name ?? "-"}, {Age}";

        [With(ReturnInterface = true)] public virtual string Name { get; set; }
        [With(ReturnInterface = true)] public int Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType20()
        {
            var type = typeof(AType20);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// In this case, as we are implementing the interface using 'With' attributes, 'InheritWiths'
    /// is redundant. In addition, note that 'ReturnInterface' is ignored on 'Age' because it is
    /// a field, and fields cannot appear on interfaces.
    /// </summary>
    [InheritWiths]
    public partial class CType20 : AType20, IFace1
    {
        public CType20(string name, int age) : base(name, age) { }
        protected CType20(CType20 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType20()
        {
            var type = typeof(CType20);

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

            var source = new CType20("XYZ", 50);
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
    public partial class CType21 : AType20
    {
        public CType21(string name, int age) : base(name, age) { }
        protected CType21(CType21 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType21()
        {
            var type = typeof(CType21);

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

            var source = new CType21("XYZ", 50);
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
    public abstract partial class AType22 : AType20, IFace1
    {
        public AType22(string name, int age) : base(name, age) { }
        protected AType22(AType22 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType22()
        {
            var type = typeof(AType22);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace1), method.ReturnType);
            Assert.True(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, Abstract", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Inherit with the host directly implementing the interface permits the methods to use that
    /// interface as the return type.
    /// </summary>
    [InheritWiths]
    public  partial class CType22 : AType22, IFace1
    {
        public CType22(string name, int age) : base(name, age) { }
        protected CType22(CType22 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType22()
        {
            var type = typeof(CType22);

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

            var source = new CType22("XYZ", 50);
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
    public abstract partial class AType23 : AType22, IFace1
    {
        public AType23(string name, int age) : base(name, age) { }
        protected AType23(AType23 source) : base(source) { }

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
        public static void Test_AType23()
        {
            var type = typeof(AType23);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.False(method.ReturnType.IsInterface);
            Assert.Equal("Public, Virtual, HideBySig, VtableLayoutMask, Abstract", method.Attributes.ToString());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reimplementing the members to modify their with settings. Note that this only works
    /// with 'Name' because only properties can be made virtual.
    /// </summary>
    [InheritWiths]
    public partial class CType23 : AType23, IFace1
    {
        public CType23(string name, int age) : base(name, age) { }
        protected CType23(CType23 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_CType23()
        {
            var type = typeof(CType23);

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

            var source = new CType23("XYZ", 50);
            var target = source.WithName("ABC");
            Assert.NotSame(source, target);
            target = source.WithAge(100);
            Assert.NotSame(source, target);
            Assert.Equal(100, target.Age);
        }
    }
}