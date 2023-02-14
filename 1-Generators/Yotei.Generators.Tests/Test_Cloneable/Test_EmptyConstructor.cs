namespace Yotei.Generators.Tests.Cloneable
{
    using EmptyConstructor;
    using IEmptyConstructor;

    // ====================================================
    //[Enforced]
    public static class Test_EmptyConstructor
    {
        [Fact]
        public static void Test()
        {
            var source = new Other.Persona()
            {
                FirstName = "James",
                LastName = "Bond",
                Age = 50,
                Level = "High"
            };
            var target = (Other.Persona)source.Clone();

            Assert.NotSame(source, target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Level, target.Level);
            Assert.Equal(source.Another, target.Another); // Equal expected as we aren't setting...
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
                int Age { get; }
            }
        }
    }

    // ====================================================
    namespace EmptyConstructor
    {
        public partial class Other
        {
            [CloneableType]
            public partial class Persona : BasePersona, IOther.IPersona
            {
                public override string ToString()
                    => $"{base.ToString()}, Age:{Age}, Level:{Level}";

                public int Age { get; set; }

                public string Level = null!;
            }
        }

        // ------------------------------------------------
        public class BasePersona
        {
            public BasePersona Clone() => new()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
            };

            public override string ToString()
                => $"First:{FirstName}, Last:{LastName}, Another:{Another}";

            public string FirstName { get; set; } = default!;

            public string LastName { get; set; } = default!;

            public readonly int Another;
        }
    }
}