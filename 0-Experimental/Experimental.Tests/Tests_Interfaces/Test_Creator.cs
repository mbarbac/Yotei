namespace Experimental.Tests.Interfaces;

// ========================================================
//[Enforced]
public static class Test_Creator
{
    public interface IPersona
    {
        static abstract IPersona Create();
        public string Country { get; }
    }

    public class Spanish : IPersona
    {
        public Spanish() => Country = "Spain";
        public string Country { get; }

        public static IPersona Create() => new Spanish();
    }

    public class Italian : IPersona
    {
        public Italian() => Country = "Italy";
        public string Country { get; }

        public static IPersona Create() => new Italian();
    }

    //[Enforced]
    [Fact]
    public static void Test1()
    {
        var spanish = Spanish.Create();
        Assert.Equal("Spain", spanish.Country);

        var italian = Italian.Create();
        Assert.Equal("Italy", italian.Country);
    }

    // ----------------------------------------------------

    public class Galician : Spanish
    {
        public Galician() : base() { }
        public static new IPersona Create() => new Galician();
    }

    //[Enforced]
    [Fact]
    public static void Test2()
    {
        var galician = Galician.Create();
        Assert.Equal("Spain", galician.Country);
        Assert.IsType<Galician>(galician);
    }

    // ----------------------------------------------------

    static IPersona Resolver<T>() where T : IPersona
    {
        return T.Create();
    }

    //[Enforced]
    [Fact]
    public static void Test3()
    {
        var item = Resolver<Spanish>();
        Assert.Equal("Spain", item.Country);

        item = Resolver<Italian>();
        Assert.Equal("Italy", item.Country);
    }
}