namespace Yotei.Generators.Tests.MemberWith
{
    using IEmptyConstructor;
    using EmptyConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_EmptyConstructor
    {
        //[Enforced]
        [Fact]
        public static void Test()
        {
            var source = new Other.Persona() { FirstName = "James", LastName = "Bond", Age = 50 };

            var target = source.WithFirstName("Other");

            Assert.NotSame(source, target);
            Assert.Equal("Other", target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);

            target = source.WithLastName("Other");

            Assert.NotSame(source, target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(source.Age, target.Age);

            target = source.WithAge(55);

            Assert.NotSame(source, target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(55, target.Age);
        }
    }

    // ====================================================
    namespace IEmptyConstructor
    {
        public partial interface IOther
        {
            public partial interface IPersona
            {
                [MemberWith] string FirstName { get; }
                [MemberWith] string? LastName { get; }
            }
        }
    }

    // ====================================================
    namespace EmptyConstructor
    {
        public partial class Other
        {
            public partial class Persona : IOther.IPersona
            {
                public Persona() { }

                public override string ToString() => $"{FirstName} {LastName}";

                [MemberWith] public string FirstName { get; init; } = default!;
                [MemberWith] public string? LastName { get; init; } = default!;

                [MemberWith] public int Age = 0;
            }
        }
    }
}