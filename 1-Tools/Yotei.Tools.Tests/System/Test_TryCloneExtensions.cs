namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_TryCloneExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_Clone_Nulls()
    {
        string? source = null;
        string? target = source.TryClone();
        Assert.Null(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_Primitives()
    {
        var source = 7;
        var target = source.TryClone();
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_Value_Types()
    {
        var source = DateTime.Now;
        var target = source.TryClone();
        Assert.Equal(source, target);
    }

    // ----------------------------------------------------

    public class Persona
    {
        public Persona(string name) => Name = name;
        public string Name { get; set; }

        public Persona Clone() => new(Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Clone_Method()
    {
        var source = new Persona("Cervantes");
        var target = source.TryClone();

        Assert.NotNull(target);
        Assert.False(ReferenceEquals(source, target));
        Assert.Equal(source.Name, target!.Name);
    }

    // ----------------------------------------------------

    public class Other
    {
        public Other(string name) => Name = name;
        public string Name { get; set; }

        public Other Clone(string newname) => new(Name + newname);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Invalid_Clone_Method()
    {
        var source = new Other("Cervantes");
        var target = source.TryClone();

        Assert.NotNull(target);
        Assert.True(ReferenceEquals(source, target));
    }
}