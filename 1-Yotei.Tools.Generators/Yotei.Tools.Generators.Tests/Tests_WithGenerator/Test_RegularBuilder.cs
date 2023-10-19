//Regular Builder scenario:
// For the base class (Persona) we will use the 'Direct' builders. For the derived one (Manager)
// we will use the 'Reverse' ones. This way we'll test the resolution mechanism.

// File-level usings...
using System;

namespace Yotei.Tools.WithGenerator.Tests
{
    // Namespace-level usings...
    using IRegularBuilder;
    using RegularBuilder;

    // ====================================================
    namespace IRegularBuilder
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
    namespace RegularBuilder
    {
        public partial class Other
        {
            // --------------------------------------------
            [WithGenerator(Specs = "(firstname,lastname,age)")]
            public partial class Persona : IOther.IPersona
            {
                public string _Info = string.Empty;
                public string FirstName { get; set; } = default!;
                public string LastName { get; set; } = default!;
                
                [WithGenerator]
                public int Age = 0;

                public Persona() => _Info = "Persona.Empty";
                public Persona(IOther.IPersona source)
                {
                    _Info = "Persona.Copy";
                    FirstName = source.FirstName;
                    LastName = source.LastName;
                    Age = source is Persona temp ? temp.Age : 0;
                }
                public Persona(string firstName, string lastName, int age)
                {
                    _Info = "Persona.Direct";
                    FirstName = firstName;
                    LastName = lastName;
                    Age = age;
                }
                public Persona(int age, string firstName, string lastName)
                {
                    _Info = "Persona.Reverse";
                    FirstName = firstName;
                    LastName = lastName;
                    Age = age;
                }
                public virtual Persona Creator()
                {
                    var temp = new Persona(this); temp._Info = "Persona.Creator";
                    return temp;
                }
            }

            // --------------------------------------------
            [WithGenerator(Specs = "(age,firstname,lastname,branch)")]
            public partial class Manager : Persona, IOther.IManager
            {
                public string Branch { get; set; } = default!;

                public Manager() => _Info = "Manager.Empty";
                public Manager(IOther.IManager source) : base(source)
                {
                    _Info = "Manager.Copy";
                    Branch = source.Branch;
                }
                public Manager(string firstName, string lastName, int age, string branch)
                    : base(firstName, lastName, age)
                {
                    _Info = "Manager.Direct";
                    Branch = branch;
                }
                public Manager(int age, string firstName, string lastName, string branch)
                    : base(age, firstName, lastName)
                {
                    _Info = "Manager.Reverse";
                    Branch = branch;
                }
                public override Manager Creator()
                {
                    var temp = new Manager(this); temp._Info = "Manager.Creator";
                    return temp;
                }
            }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Test_RegularBuilder
    {
        //[Enforced]
        [Fact]
        public static void Test_Persona_Interface()
        {
            IOther.IPersona source = new Other.Persona("James", "Bond", 50);

            var target = source.WithLastName("Other");
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IPersona>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(((Other.Persona)source).Age, ((Other.Persona)target).Age);
            Assert.Equal("Persona.Direct", ((Other.Persona)target)._Info);
        }

        //[Enforced]
        [Fact]
        public static void Test_Persona_Concrete()
        {
            var source = new Other.Persona("James", "Bond", 50);

            var target = source.WithLastName("Other");
            Assert.NotSame(source, target);
            Assert.IsType<Other.Persona>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal("Persona.Direct", target._Info);

            target = source.WithAge(25);
            Assert.NotSame(source, target);
            Assert.IsType<Other.Persona>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(25, target.Age);
            Assert.Equal("Persona.Direct", target._Info);
        }

        // ------------------------------------------------

        //[Enforced]
        [Fact]
        public static void Test_Manager_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", 50, "MI6");

            var target = source.WithLastName("Other");
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IManager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(((Other.Manager)source).Age, ((Other.Manager)target).Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Reverse", ((Other.Manager)target)._Info);
        }

        //[Enforced]
        [Fact]
        public static void Test_Manager_Concrete()
        {
            var source = new Other.Manager("James", "Bond", 50, "MI6");

            var target = source.WithLastName("Other");
            Assert.NotSame(source, target);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Reverse", target._Info);

            target = source.WithAge(25);
            Assert.NotSame(source, target);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(25, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Reverse", target._Info);
        }
    }
}