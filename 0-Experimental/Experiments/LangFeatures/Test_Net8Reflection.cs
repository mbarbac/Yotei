namespace Experiments.LangFeatures;

// ========================================================
//[Enforced]
public static class Test_Net8reflection
{
    public class Persona
    {
        [SuppressMessage("", "IDE0290")]
        public Persona(string name, int age) { Name = name; _Age = age; }
        public override string ToString() => $"Name:{Name}, Age:{_Age}";
        public int AgeGetter() => _Age;

        [SuppressMessage("", "IDE0051")]
        private const int _Level = 9;
        private readonly int _Age;
        public string Name { get; init; }
        public string Branch { get; } = "Any";
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern Persona NewPersona(string name, int age);

    //[Enforced]
    [Fact]
    public static void Test_Constructor()
    {
        var persona = NewPersona("James", 50);
        Assert.Equal("James", persona.Name);
        Assert.Equal(50, persona.AgeGetter());
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_Age")]
    private static extern ref int GetAge(Persona persona);

    //[Enforced]
    [Fact]
    public static void Test_Private_Field()
    {
        var persona = NewPersona("James", 50);
        Assert.Equal(50, GetAge(persona));

        GetAge(persona) = 25;
        Assert.Equal(25, persona.AgeGetter());
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_Level")]
    private static extern ref int GetLevel(Persona persona);

    //[Enforced]
    [Fact]
    public static void Test_Readonly_Field()
    {
        var persona = NewPersona("James", 50);

        try { Assert.Equal(9, GetLevel(persona)); Assert.Fail(); }
        catch (MissingFieldException) { }
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Name")]
    private static extern string GetName(Persona persona);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Name")]
    private static extern void SetName(Persona persona, string value);

    //[Enforced]
    [Fact]
    public static void Test_Property()
    {
        var persona = NewPersona("James", 50);
        Assert.Equal("James", persona.Name);
        Assert.Equal("James", GetName(persona));

        SetName(persona, "Other");
        Assert.Equal("Other", persona.Name);
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Branch")]
    private static extern string GetBranch(Persona persona);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Branch")]
    private static extern void SetBranch(Persona persona, string value);

    //[Enforced]
    [Fact]
    public static void Test_Readonly_Property()
    {
        var persona = NewPersona("James", 50);
        Assert.Equal("James", persona.Name);
        Assert.Equal("Any", GetBranch(persona));

        try { SetBranch(persona, "Other"); Assert.Fail(); }
        catch (MissingMethodException) { }
    }
}