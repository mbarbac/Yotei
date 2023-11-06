// File-level elements...
#pragma whatever
using System;

namespace Yotei.Tools.CloneGenerator.Tests
{
    // Namespace-level elements...
    using ICopyBuilder;
    using CopyBuilder;

    // ====================================================
    namespace ICopyBuilder
    {
        public partial interface IOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial interface IPersona : ICloneable
            {
                string FirstName { get; }
                string LastName { get; }
            }

            // --------------------------------------------
            [Cloneable]
            public partial interface IManager : IPersona
            {
                string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace CopyBuilder
    {
        public partial class Other
        {
            // --------------------------------------------
            [Cloneable]
            public partial class Persona : IOther.IPersona
            {
                public string _Info = string.Empty;

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
            [Cloneable]
            public partial class Manager : Persona, IOther.IManager
            {
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

    // ====================================================
    //[Enforced]
    public static class Test_CopyBuilder
    {
        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IManager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(((Other.Manager)source).Age, ((Other.Manager)target).Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Copy", ((Other.Manager)target)._Info);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Copy", target._Info);
        }
    }
}