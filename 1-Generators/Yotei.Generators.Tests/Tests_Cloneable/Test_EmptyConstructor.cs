namespace Yotei.Generators.Tests.Cloneable
{
    using IEmptyConstructor;
    using EmptyConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_EmptyConstructor
    {
        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager() { FirstName = "James", LastName = "Bond", Age = 50, Branch = "MI6" };

            var target = source.Clone();
            Assert.NotSame(target, source);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Branch, target.Branch);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager() { FirstName = "James", LastName = "Bond", Age = 50, Branch = "MI6" };

            var target = source.Clone();
            Assert.NotSame(target, source);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Branch, target.Branch);
        }
    }

    // ====================================================
    namespace IEmptyConstructor
    {
        public partial interface IOther
        {
            [CloneableType]
            public partial interface IPersona
            {
                string FirstName { get; }
                string LastName { get; }
            }

            [CloneableType]
            public partial interface IManager : IPersona
            {
                new string FirstName { get; }
                new string LastName { get; }
                string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace EmptyConstructor
    {
        public partial class Other
        {
            [CloneableType]
            public partial class Persona : IOther.IPersona
            {
                public Persona() { }
                public override string ToString() => $"{FirstName} {LastName} {Age}";

                public string FirstName { get; init; } = default!;
                public string LastName { get; init; } = default!;

                public int Age = 0;
            }

            [CloneableType]
            public partial class Manager : Persona, IOther.IManager
            {
                public Manager() : base() { }
                public override string ToString() => $"{base.ToString()} {Branch}";

                public new string FirstName { get => base.FirstName; init => base.FirstName = value; }
                public new string LastName { get => base.LastName; init => base.LastName = value; }
                public string Branch { get; init; } = default!;
            }
        }
    }
}