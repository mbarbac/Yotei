namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_IFace_CreatorFactory
{
    public interface IPersona
    {
        static abstract IPersona Create();
        public string Country { get; }

        public static IPersona Creator<T>() where T : IPersona => T.Create();
    }

    public class Spanish : IPersona
    {
        public static IPersona Create() => new Spanish();
        public string Country { get; protected set; }
        public Spanish() => Country = "Spain";
    }

    public class Italian : IPersona
    {
        public static IPersona Create() => new Italian();
        public string Country { get; protected set; }
        public Italian() => Country = "Italy";
    }

    //[Enforced]
    [Fact]
    public static void Test_Using_Create()
    {
        var item = Spanish.Create(); Assert.Equal("Spain", item.Country);
        item = Italian.Create(); Assert.Equal("Italy", item.Country);
    }

    // ----------------------------------------------------

    // This is an implementation of a factory pattern.
    // For instance, the 'Create' method invoked may be a protected virtual or abstract one.

    //[Enforced]
    [Fact]
    public static void Test_Using_Creator()
    {
        var item = IPersona.Creator<Spanish>(); Assert.Equal("Spain", item.Country);
        item = IPersona.Creator<Italian>(); Assert.Equal("Italy", item.Country);

        // item = IPersona.Creator<Unknown>(); -- compiler intercepts!
    }

    public class Unknown { }

    // ----------------------------------------------------

    // This can be implemented in a helper class or whereever. It may be useful when a priori
    // we don't know what 'T' class to create, maybe we are obtained it dynamically or from
    // context.

    static IPersona Resolver<T>() where T : IPersona => T.Create();

    //[Enforced]
    [Fact]
    public static void Test_Using_Resolver()
    {
        var item = Resolver<Spanish>(); Assert.Equal("Spain", item.Country);
        item = Resolver<Italian>(); Assert.Equal("Italy", item.Country);
    }
}