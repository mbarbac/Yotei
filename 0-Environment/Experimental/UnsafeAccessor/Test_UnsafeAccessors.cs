 #pragma warning disable CS0414
namespace Experimental.UnsafeAccessors;

// ========================================================
public static class Test_UnsafeAccessors
{
    // Test type.
    public class Persona
    {
        public override string ToString() => $"Name:{Name}, Age:{Age}";

        public Persona() { Name = "-"; Score = -1; Age = -2; }
        private Persona(string name, int score, int age) { Name = name; Score = score; Age = age; }

        public int Score;

        public int AgeGetter() => Age;
        private readonly int Age;
        
        [SuppressMessage("", "IDE0044")]
        private static int Level = 9;
        
        private static readonly int Casualties = 100;

        public string Name { get; init; }
        private string Branch { get; set; } = "Any";
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern Persona NewPersona();

    [Fact]
    public static void Test_Public_Constructor()
    {
        var item = NewPersona();
        Assert.Equal("-", item.Name);
        Assert.Equal(-1, item.Score);
        Assert.Equal(-2, item.AgeGetter());
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern Persona NewPersona(string name, int score, int age);

    [Fact]
    public static void Test_Private_Constructor()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal("James", item.Name);
        Assert.Equal(7, item.Score);
        Assert.Equal(50, item.AgeGetter());
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "Score")]
    private static extern ref int GetScore(Persona persona);

    [Fact]
    public static void Test_Public_Field()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal(7, GetScore(item));

        GetScore(item) = 109;
        Assert.Equal(109, item.Score);
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "Age")]
    private static extern ref int GetAge(Persona persona);

    [Fact]
    public static void Test_Private_Field()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal(50, GetAge(item));

        GetAge(item) = 60;
        Assert.Equal(60, item.AgeGetter());
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "Level")]
    private static extern ref int GetLevel(Persona persona);

    [Fact]
    public static void Test_Static_Field()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal(9, GetLevel(item));

        GetLevel(item) = 10;
        Assert.Equal(10, GetLevel(item));

        // Recovering...
        GetLevel(item) = 9;
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "Casualties")]
    private static extern ref int GetCasualties(Persona persona);

    [Fact]
    public static void Test_Static_Readonly_Field()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal(100, GetCasualties(item));

        // We can modify a readonly field!
        GetCasualties(item) = 200;
        Assert.Equal(200, GetCasualties(item));

        // Recovering...
        GetCasualties(item) = 100;
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Method/*, Name = "get_Name"*/)]
    private static extern string get_Name(Persona persona);

    [UnsafeAccessor(UnsafeAccessorKind.Method/*, Name = "set_Name"*/)]
    private static extern void set_Name(Persona persona, string value);

    [Fact]
    public static void Test_Public_Property()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal("James", item.Name);
        Assert.Equal("James", get_Name(item));

        set_Name(item, "other");
        Assert.Equal("other", item.Name);
        Assert.Equal("other", get_Name(item));
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Branch")]
    private static extern string GetBranch(Persona persona);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Branch")]
    private static extern void SetBranch(Persona persona, string value);

    [Fact]
    public static void Test_Private_Property()
    {
        var item = NewPersona("James", 007, 50);
        Assert.Equal("Any", GetBranch(item));

        // If there is no setter, then we will obtain a 'MissingMethodException'...
        SetBranch(item, "other");
        Assert.Equal("other", GetBranch(item));
    }
}