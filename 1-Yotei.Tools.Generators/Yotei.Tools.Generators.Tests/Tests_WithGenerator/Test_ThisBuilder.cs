// File-level elements...
#pragma whatever
using System;

namespace Yotei.Tools.Generators.Tests.WithGenerator
{
    // Namespace-level elements...
    using IThisBuilder;
    using ThisBuilder;

    // ====================================================
    //[Enforced]
    public static class Test_ThisBuilder
    {
        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.WithLastName(source.LastName);
            Assert.Same(source, target);

            target = source.WithLastName("Other");
            Assert.Same(source, target);

            Assert.IsAssignableFrom<IOther.IManager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(((Other.Manager)source).Age, ((Other.Manager)target).Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Regular", ((Other.Manager)target)._Info);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.WithLastName(source.LastName);
            Assert.Same(source, target);

            target = source.WithLastName("Other");
            Assert.Same(source, target);

            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Regular", target._Info);

            target = source.WithAge(source.Age);
            Assert.Same(source, target);

            target = source.WithAge(60);
            Assert.Same(source, target);

            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(60, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Regular", target._Info);
        }
    }

    // ====================================================
    namespace IThisBuilder
    {
        public partial interface IOther
        {
            // --------------------------------------------
            public partial interface IPersona
            {
                [WithGenerator] string FirstName { get; }
                [WithGenerator] string LastName { get; }
            }

            // --------------------------------------------
            [WithGenerator]
            public partial interface IManager : IPersona
            {
                [WithGenerator] string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace ThisBuilder
    {
        public partial class Other
        {
            // --------------------------------------------
            // Using 'this' at the root of the hierarchy...
            [WithGenerator("this")]
            public partial class Persona : IOther.IPersona
            {
                public string _Info = string.Empty;

                [WithGenerator]
                public int Age = 0;
                public string FirstName { get; set; } = default!;
                public string LastName { get; set; } = default!;

                public Persona(string first, string last, int age)
                {
                    _Info = "Persona.Regular";
                    FirstName = first;
                    LastName = last;
                    Age = age;
                }

                protected Persona(Persona source)
                {
                    _Info = "Persona.Copy";
                    FirstName = source.FirstName;
                    LastName = source.LastName;
                    Age = source.Age;
                }
            }

            // --------------------------------------------
            [WithGenerator]
            public partial class Manager : Persona, IOther.IManager
            {
                [WithGenerator]
                public string Branch { get; set; } = default!;

                public Manager(string first, string last, int age, string branch)
                    : base(first, last, age)
                {
                    _Info = "Manager.Regular";
                    Branch = branch;
                }

                protected Manager(Manager source) : base(source)
                {
                    _Info = "Manager.Copy";
                    Branch = source.Branch;
                }
            }
        }
    }
}