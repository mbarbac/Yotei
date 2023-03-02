namespace Yotei.Generators.Tests.MemberWith
{
    using IRegularConstructor;
    using RegularConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_RegularConstructor
    {
        [Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", "MI6") { Age = 50 };

            var target = source.WithFirstName("Other");
            Assert.NotSame(target, source);
            Assert.IsAssignableFrom<IOther.IManager>(target);
            Assert.Equal("Other", target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Branch, target.Branch);

            target = source.WithBranch("Other");
            Assert.NotSame(target, source);
            Assert.IsAssignableFrom<IOther.IManager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal("Other", target.Branch);
        }

        [Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager("James", "Bond", "MI6") { Age = 50 };

            var target = source.WithFirstName("Other");
            Assert.NotSame(target, source);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal("Other", target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Branch, target.Branch);

            target = source.WithBranch("Other");
            Assert.NotSame(target, source);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal("Other", target.Branch);

            var other = source.WithAge(55);
            Assert.NotSame(other, source);
            Assert.IsType<Other.Persona>(other);
            Assert.Equal(source.FirstName, other.FirstName);
            Assert.Equal(source.LastName, other.LastName);
            Assert.Equal(55, other.Age);
        }
    }

    // ====================================================
    namespace IRegularConstructor
    {
        public partial interface IOther
        {
            public partial interface IPersona
            {
                [MemberWith] string FirstName { get; }
                [MemberWith] string LastName { get; }
            }

            public partial interface IManager : IPersona
            {
                [MemberWith] new string FirstName { get; }
                [MemberWith] new string LastName { get; }
                [MemberWith] string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace RegularConstructor
    {
        public partial class Other
        {
            public partial class Persona : IOther.IPersona
            {
                public Persona(string firstName, string lastName)
                {
                    FirstName = firstName;
                    LastName = lastName;
                }
                public override string ToString() => $"{FirstName} {LastName} {Age}";

                [MemberWith] public string FirstName { get; init; } = default!;
                [MemberWith] public string LastName { get; init; } = default!;

                [MemberWith] public int Age = 0;
            }

            public partial class Manager : Persona, IOther.IManager
            {
                public Manager(string firstName, string lastName, string branch)
                    : base(firstName, lastName)
                {
                    Branch = branch;
                }
                public override string ToString() => $"{base.ToString()} {Branch}";

                [MemberWith] public new string FirstName { get => base.FirstName; init => base.FirstName = value; }
                [MemberWith] public new string LastName { get => base.LastName; init => base.LastName = value; }
                [MemberWith] public string Branch { get; init; } = default!;
            }
        }
    }
}