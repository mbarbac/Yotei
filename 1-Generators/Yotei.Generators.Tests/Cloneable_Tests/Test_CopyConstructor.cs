namespace Yotei.Generators.Tests.Cloneable
{
    using CopyConstructor;
    using ICopyConstructor;

    // ====================================================
    [Enforced]
    public static class Test_CopyConstructor
    {
        [Fact]
        public static void Test()
        {
            var source = new Other.Persona("James", "Bond", 50, "High");
            var target = (Other.Persona)source.Clone();

            Assert.NotSame(source, target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.NotEqual(source.Level, target.Level); // Not equal expected...
            Assert.Equal(source.Another, target.Another); // Both cero as not set...
        }
    }

    // ====================================================
    namespace ICopyConstructor
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
    namespace CopyConstructor
    {
        public partial class Other
        {
            [CloneableType]
            public partial class Persona : BasePersona, IOther.IPersona
            {
                public Persona(string first, string last, int age, string level)
                    : base(first, last)
                {
                    Age = age;
                    Level = level;
                }

                public Persona(IOther.IPersona source)
                    : base(source.FirstName, source.LastName)
                {
                    Age = source.Age;
                }

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

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public readonly int Another;
        }
    }
}