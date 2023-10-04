// File-level usings...
using System;

namespace Yotei.Tools.WithGenerator.Tests
{
    // Namespace-level usings...
    using ICopyBuilder;
    using CopyBuilder;

    // ====================================================
    //[Enforced]
    public static class Test_CopyBuilder
    {
        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", 50, "MI6");

            var target = source.WithLastName("Other");
            Assert.NotSame(source, target);
            Assert.IsAssignableFrom<IOther.IManager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(((Other.Manager)source).Age, ((Other.Manager)target).Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Copy", ((Other.Manager)target).Info);
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager("James", "Bond", 50, "MI6");

            var target = source.WithLastName("Other");
            Assert.NotSame(source, target);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal("Other", target.LastName);
            Assert.Equal(source.Age, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Copy", target.Info);

            target = source.WithAge(25);
            Assert.NotSame(source, target);
            Assert.IsType<Other.Manager>(target);
            Assert.Equal(source.FirstName, target.FirstName);
            Assert.Equal(source.LastName, target.LastName);
            Assert.Equal(25, target.Age);
            Assert.Equal(source.Branch, target.Branch);
            Assert.Equal("Manager.Copy", target.Info);
        }
    }

    // ====================================================
    namespace ICopyBuilder
    {
        public partial interface IOther
        {
            // --------------------------------------------
            public partial interface IPersona
            {
                [WithGenerator] string FirstName { get; }
                [WithGenerator] string? LastName { get; }
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
    namespace CopyBuilder
    {
        public partial class Other
        {
            // --------------------------------------------
            [WithGenerator("(source)+*-info")]
            public partial class Persona : IOther.IPersona
            {
                public string Info = string.Empty;

                public Persona() => Info = "Persona.Empty";

                public Persona(IOther.IPersona source)
                {
                    Info = "Persona.Copy";
                    FirstName = source.FirstName;
                    LastName = source.LastName;
                    Age = source is Persona temp ? temp.Age : 0;
                }

                public Persona(string firstName, string? lastName, int age)
                {
                    Info = "Persona.Regular";
                    FirstName = firstName;
                    LastName = lastName;
                    Age = age;
                }

                public Persona(int age, string firstName, string? lastName)
                {
                    Info = "Persona.Inverse";
                    FirstName = firstName;
                    LastName = lastName;
                    Age = age;
                }

                public virtual Persona Creator()
                {
                    var temp = new Persona(this); temp.Info = "Persona.Creator";
                    return temp;
                }

                public string FirstName { get; init; } = default!;

                public virtual string? LastName { get; set; } = null;

                [WithGenerator]
                public int Age = 0;
            }

            // --------------------------------------------
            [WithGenerator("(source)+*-info")]
            public partial class Manager : Persona, IOther.IManager
            {
                public Manager() => Info = "Manager.Empty";

                public Manager(IOther.IManager source) : base(source)
                {
                    Info = "Manager.Copy";
                    Branch = source.Branch;
                }

                public Manager(string firstName, string? lastName, int age, string branch)
                    : base(firstName, lastName, age)
                {
                    Info = "Manager.Regular";
                    Branch = branch;
                }

                public Manager(int age, string firstName, string? lastName, string branch)
                    : base(age, firstName, lastName)
                {
                    Info = "Manager.Inverse";
                    Branch = branch;
                }

                public override Manager Creator()
                {
                    var temp = new Manager(this); temp.Info = "Manager.Creator";
                    return temp;
                }

                public override string? LastName
                {
                    get => base.LastName;
                    set => base.LastName = value;
                }

                public string Branch { get; set; } = default!;
            }
        }
    }
}