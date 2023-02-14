namespace Yotei.Generators.Tests.Cloneable
{
    using ManyConstructor;
    using IManyConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_ManyConstructor
    {
        [Fact]
        public static void Test()
        {
            var source = new Other.Persona("James", "Bond") { Age = 50, Level = "High" };
            var target = (Other.Persona)source.Clone();

            Assert.NotSame(source, target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Level, target.Level);
            Assert.Equal(source.Another, target.Another); // Both cero as not set...
        }
    }

    // ====================================================
    namespace IManyConstructor
    {
        public partial interface IOther
        {
            [CloneableType]
            public partial interface IPersona
            {
                string FirstName { get; }
                string LastName { get; }
                int Age { get; }
            }
        }
    }

    // ====================================================
    namespace ManyConstructor
    {
        public partial class Other
        {
            [CloneableType]
            public partial class Persona : BasePersona, IOther.IPersona
            {
                public Persona(string firstName, string lastName) : base(firstName, lastName) { }

                public override string ToString()
                    => $"{base.ToString()}, Age:{Age}, Level:{Level}";

                public int Age { get; set; }

                public string Level = null!;
            }
        }

        // ------------------------------------------------
        public class BasePersona
        {
            public BasePersona(string firstName, string lastName)
                : this(firstName, lastName, 0) { }

            public BasePersona(string firstName, string lastName, int another)
            {
                FirstName = firstName;
                LastName = lastName;
                Another = another;
            }

            public BasePersona Clone() => new(FirstName, LastName, Another);

            public override string ToString()
                => $"First:{FirstName}, Last:{LastName}, Another:{Another}";

            [CloneableMember(Deep = true)]
            public string FirstName { get; set; }

            public string LastName { get; init; }

            public readonly int Another;
        }
    }
}