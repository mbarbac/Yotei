namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_IFace_StaticProperty
{
    // An abstract static property declared in an interface gets a different implementation in
    // each type that implements the interface, they being independent from each other.
    public interface ILevelProvider
    {
        // If we use 'public static abstract', then trying to use 'ILevelProvider.Level'
        // gets a 'CS8926: a static virtual or abstract interface member can be accessed only
        // on a type parameter'. But all other type-based accessors work.

        public static string Level { get; } = null!;
    }

    public class TPerson : ILevelProvider
    {
        public static string Level { get; set; } = "Person";
        [SuppressMessage("", "CA1822")] public void SetLevel(string msg) => Level = msg;
    }

    public class TManager : ILevelProvider
    {
        public static string Level { get; set; } = "Manager";
        [SuppressMessage("", "CA1822")] public void SetLevel(string msg) => Level = msg;
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Independent_StaticProperty_DeclaredInInterface()
    {
        Assert.Null(ILevelProvider.Level);
        Assert.Equal("Person", TPerson.Level);
        Assert.Equal("Manager", TManager.Level);

        TPerson.Level = "other";
        Assert.Null(ILevelProvider.Level);
        Assert.Equal("other", TPerson.Level);
        Assert.Equal("Manager", TManager.Level);

        var item = new TPerson();
        item.SetLevel("any");
        Assert.Null(ILevelProvider.Level);
        Assert.Equal("any", TPerson.Level);
        Assert.Equal("Manager", TManager.Level);
    }
}