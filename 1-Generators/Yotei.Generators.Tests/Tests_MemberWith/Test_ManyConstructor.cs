namespace Yotei.Generators.Tests.MemberWith
{
    using IManyConstructor;
    using ManyConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_ManyConstructor
    {
        [Enforced]
        [Fact]
        public static void Test()
        {
            var source = new Other.Persona("James") { LastName = "Bond" };
            var target = source.WithFirstName("Other");

            Assert.NotSame(source, target);
            Assert.Equal("Other", target.FirstName);
            Assert.Equal(source.LastName, target.LastName);

            target = source.WithLastName("Other");

            Assert.NotSame(source, target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
        }
    }

    // ====================================================
    namespace IManyConstructor
    {
        public partial interface IOther
        {
            public partial interface IPersona
            {
                [MemberWith] string FirstName { get; }
                [MemberWith] string LastName { get; }
            }
        }
    }

    // ====================================================
    namespace ManyConstructor
    {
        public partial class Other
        {
            public partial class Persona : IOther.IPersona
            {
                public Persona(string firstName)
                {
                    FirstName = firstName;
                }

                public override string ToString() => $"{FirstName} {LastName}";

                [MemberWith] public string FirstName { get; init; } = default!;
                [MemberWith] public string LastName { get; init; } = default!;
            }
        }
    }
}