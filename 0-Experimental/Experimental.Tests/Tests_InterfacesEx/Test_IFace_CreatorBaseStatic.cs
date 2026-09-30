namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_IFace_CreatorBaseStatic
{
    public interface IPersona
    {
        static abstract IPersona Create();
        public string Country { get; }
    }

    public class Spanish : IPersona
    {
        public static IPersona Create() => new Spanish();
        public string Country { get; protected set; }
        public Spanish() => Country = "Spain";
    }

    public class French : IPersona
    {
        public static IPersona Create() => new French();
        public string Country { get; }
        public French() => Country = "France";
    }

    //[Enforced]
    [Fact]
    public static void Test_Using_EnforcedStaticMethod()
    {
        IPersona item;
        var spanish = Spanish.Create(); Assert.Equal("Spain", spanish.Country);
        var french = French.Create(); Assert.Equal("France", french.Country);

        item = Spanish.Create(); Assert.Equal("Spain", item.Country);
        item = French.Create(); Assert.Equal("France", item.Country);
    }

    // ====================================================

    public class Galician : Spanish
    {
        public static new IPersona Create() => new Galician();
        public Galician() => Country = base.Country + "-Galicia";
    }

    //[Enforced]
    [Fact]
    public static void Test_DerivedClass()
    {
        var galician = new Galician();
        Assert.Equal("Spain-Galicia", galician.Country);

        IPersona item = galician;
        Assert.Equal("Spain-Galicia", galician.Country);
    }
}