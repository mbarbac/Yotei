namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_Net8Reflection
{
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern Net8Reflection.Persona NewPersona(string name, int age);

    //[Enforced]
    [Fact]
    public static void Test_UnsafeAccessor_Constructor()
    {
        var persona = NewPersona("James", 50);
        Assert.Equal("James", persona.Name);
        Assert.Equal(50, persona.AgeGetter());
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_Age")]
    private static extern ref int GetPersonaAge(Net8Reflection.Persona persona);

    //[Enforced]
    [Fact]
    public static void Test_UnsafeAccessor_Private_Field()
    {
        var persona = new Net8Reflection.Persona("James", 50);
        var value = GetPersonaAge(persona);
        Assert.Equal(50, value);

        GetPersonaAge(persona) = 25;
        value = GetPersonaAge(persona);
        Assert.Equal(25, value);
    }

    // ----------------------------------------------------

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Name")]
    private static extern string GetPersonaName(Net8Reflection.Persona persona);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Name")]
    private static extern void SetPersonaName(Net8Reflection.Persona persona, string value);

    //[Enforced]
    [Fact]
    public static void Test_UnsafeAccessor_Property()
    {
        var persona = new Net8Reflection.Persona("James", 50);
        var value = GetPersonaName(persona);
        Assert.Equal("James", value);

        SetPersonaName(persona, "John");
        Assert.Equal("John", persona.Name);
    }
}