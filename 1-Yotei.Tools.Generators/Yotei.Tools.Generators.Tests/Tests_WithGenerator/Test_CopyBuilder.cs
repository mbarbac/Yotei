// File-level elements...
#pragma whatever
using System;

namespace Yotei.Tools.Generators.Tests.WithGenerator
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
    namespace CopyBuilder
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