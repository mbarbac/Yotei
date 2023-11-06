// File-level elements...
#pragma whatever
using System;

namespace Yotei.Tools.WithGenerator.Tests
{
    // Namespace-level elements...
    using IBaseBuilder;
    using BaseBuilder;

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
                public virtual string LastName
                {
                    get => _LastName;
                    set
                    {
                        _LastName = value;
                        _Info = "Persona.LastName";
                    }
                }
                string _LastName = default!;

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
            [WithGenerator("base")] // <== To use base methods by default!
            public partial class Manager : Persona, IOther.IManager
            {
                [WithGenerator("copy")] // <== No base method for 'Branch'...
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

                public override string LastName
                {
                    get => base.LastName;
                    set
                    {
                        base.LastName = value;
                        _Info = "Manager.LastName";
                    }
                }
            }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Test_BaseBuilder
    {
        //[Enforced]
        [Fact]
        public static void Test_Interface()
        {
            IOther.IManager source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.WithLastName(source.LastName);
            Assert.Same(source, target);

            try { source.WithLastName("Other"); Assert.Fail(); }
            catch (InvalidCastException) { }
        }

        //[Enforced]
        [Fact]
        public static void Test_Concrete()
        {
            var source = new Other.Manager("James", "Bond", 50, "UK");

            var target = source.WithLastName(source.LastName);
            Assert.Same(source, target);

            try { source.WithLastName("Other"); Assert.Fail(); }
            catch (InvalidCastException) { }

            target = source.WithAge(source.Age);
            Assert.Same(source, target);

            try { source.WithAge(60); Assert.Fail(); }
            catch (InvalidCastException) { }
        }
    }
}