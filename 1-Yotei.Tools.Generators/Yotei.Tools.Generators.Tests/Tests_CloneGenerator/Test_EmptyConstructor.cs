// File-level usings...
using System;

namespace Yotei.Tools.CloneGenerator.Tests
{
    // Namespace-level usings...
    using IEmptyConstructor;
    using EmptyConstructor;

    // ====================================================
    namespace IEmptyConstructor
    {
        public partial interface IOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial interface IPersona
            {
                string FirstName { get; }
                string? LastName { get; }
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
    namespace EmptyConstructor
    {
        public partial class Other
        {
            // --------------------------------------------
            [Cloneable("()-info")]
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

                public string FirstName { get; set; } = default!;

                public virtual string? LastName { get; set; } = null;

                public int Age = 0;
            }

            // --------------------------------------------
            [Cloneable("()-info")]
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