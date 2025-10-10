#pragma warning disable IDE0079

namespace Experimental.Tests.Interfaces;

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

        [SuppressMessage("", "CA1822")]
        public void SetName(string v) { Level = v; }
    }

    public class TManager : IPerson
    {
        public static string Level { get; set; } = "manager";

        [SuppressMessage("", "CA1822")]
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
    }
}