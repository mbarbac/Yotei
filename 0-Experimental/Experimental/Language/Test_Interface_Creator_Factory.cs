namespace Experimental.Language;

// =========================================================
//[Enforced]
public static class Test_Interface_Creator_Factory
{
    public interface IPersona
    {
        public static T Create<T>() where T : IPersona
        {
            return (T)T.CreateInstance();
        }
        static abstract IPersona CreateInstance();
        public string Country { get; }
    }

    public class Spanish : IPersona
    {
        public Spanish() => Country = "Spain";
        public string Country { get; }
        public static IPersona CreateInstance() => new Spanish();
    }

    public class Italian : IPersona
    {
        public Italian() => Country = "Italy";
        public string Country { get; }
        public static IPersona CreateInstance() => new Italian();
    }

    //[Enforced]
    [Fact]
    public static void Test_Factory()
    {
        var spain = IPersona.Create<Spanish>();
        Assert.Equal("Spain", spain.Country);

        var italy = IPersona.Create<Italian>();
        Assert.Equal("Italy", italy.Country);
    }

    // ----------------------------------------------------

    public interface IManager : IPersona
    {
        public static new T Create<T>() where T : IManager
        {
            return (T)T.CreateInstance();
        }
        static abstract new IManager CreateInstance();
        public int Reports { get; }
    }

    public class SpanishManager : Spanish, IManager
    {
        public SpanishManager() : base() { }
        public int Reports => 100;
        static IManager IManager.CreateInstance() => new SpanishManager();

    }

    //[Enforced]
    [Fact]
    public static void Test_Factory_Derived()
    {
        var spain = IManager.Create<SpanishManager>();
        Assert.Equal("Spain", spain.Country);
        Assert.Equal(100, spain.Reports);
    }

    // ----------------------------------------------------

    public abstract class Executive : IPersona
    {
        public abstract string Country { get; }
        public static IPersona CreateInstance() => throw new TargetException();
    }

    public class SpainExecutive : Executive, IPersona // Need 'IPersona' to resolve CreateInstance
    {
        public SpainExecutive() => Country = "Spain";
        public override string Country { get; }
        public static new IPersona CreateInstance() => new SpainExecutive();
    }

    //[Enforced]
    [Fact]
    public static void Test_Factory_Over_Abstract()
    {
        var spain = IPersona.Create<SpainExecutive>();
        Assert.Equal("Spain", spain.Country);

        try { IPersona.Create<Executive>(); Assert.Fail(); }
        catch (TargetException) { }
    }
}