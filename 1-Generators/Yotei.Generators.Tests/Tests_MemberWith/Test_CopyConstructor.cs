namespace Yotei.Generators.Tests.MemberWith
{
    using ICopyConstructor;
    using CopyConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_CopyConstructor
    {
        //[Enforced]
        [Fact]
        public static void Test()
        {
            var source = new Other.Persona() { FirstName = "James", LastName = "Bond" };
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
    namespace ICopyConstructor
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
    namespace CopyConstructor
    {
        public partial class Other
        {
            public partial class Persona : IOther.IPersona
            {
                public Persona(IOther.IPersona other)
                {
                    FirstName = other.FirstName;
                    LastName = other.LastName;
                }
                public Persona() { }

                public override string ToString() => $"{FirstName} {LastName}";

                [MemberWith] public string FirstName { get; init; } = default!;
                [MemberWith] public string? LastName { get; init; } = default!;
            }
        }
    }
}