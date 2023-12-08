// File-level elements...
#pragma whatever
using System;

namespace Yotei.Tools.Generators.Tests
{
    // Namespace-level elements...
    using IBaseBuilder;
    using BaseBuilder;

    // ====================================================
    //[Enforced]
    public static class Test_BaseBuilder
    {
        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.WithLastName(source.LastName); // Because no changes...
            Assert.Same(source, target);

            // Should not cast Persona to Manager...
            try { source.WithLastName("Other"); Assert.Fail(); }
            catch (InvalidCastException) { }
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.WithLastName(source.LastName); // Because no changes...
            Assert.Same(source, target);

            target = source.WithAge(source.Age); // Because no changes...
            Assert.Same(source, target);

            target = source.WithBranch("Other");
            Assert.NotSame(source, target);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal("Other", target.Branch);
            Assert.Equal("Manager.Copy", target._Info);

            // Should not cast Persona to Manager...
            try { source.WithLastName("Other"); Assert.Fail(); }
            catch (InvalidCastException) { }

            // Should not cast Persona to Manager...
            try { source.WithAge(60); Assert.Fail(); }
            catch (InvalidCastException) { }
        }
    }

    // ====================================================
    namespace IBaseBuilder
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
    namespace BaseBuilder
    {
        public partial class Other
        {
            // --------------------------------------------
            [WithGenerator]
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
            // Using 'base' with all methods by default...
            [WithGenerator("base")]
            public partial class Manager : Persona, IOther.IManager
            {
                // Not using 'base' method for 'Branch'...
                [WithGenerator("copy")]
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