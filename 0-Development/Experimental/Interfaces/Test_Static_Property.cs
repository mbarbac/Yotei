#pragma warning disable IDE0079

namespace Experimental.Interfaces;

// ========================================================
//[Enforced]
public static class Test_Static_Property
{
    public interface IPerson
    {
        public static abstract string Level { get; }
    }

    public class TPerson : IPerson
    {
        public static string Level { get; set; } = "individual";

        [SuppressMessage(
            "Performance", "CA1822:Mark members as static",
            Justification = "Will be called below from a concrete type, hence cannot be static.")]
        public void SetName(string v) { Level = v; }
    }

    public class TManager : IPerson
    {
        public static string Level { get; set; } = "manager";

        [SuppressMessage(
            "Performance", "CA1822:Mark members as static",
            Justification = "Will be called below from a concrete type, hence cannot be static.")]
        public void SetName(string v) { Level = v; }
    }

    //[Enforced]
    [Fact]
    public static void Test()
    {
        Assert.Equal("individual", TPerson.Level);
        Assert.Equal("manager", TManager.Level);

        var item = new TPerson();
        item.SetName("hola");
        Assert.Equal("hola", TPerson.Level);
        Assert.Equal("manager", TManager.Level);

        TPerson.Level = "persona";
        Assert.Equal("persona", TPerson.Level);

        // CS0176: Static member 'member' cannot be accessed with an instance reference; qualify
        // it with a type name instead. Only a class name can be used to qualify a static variable;
        // an instance name cannot be a qualifier.
        // item.Level
    }
}