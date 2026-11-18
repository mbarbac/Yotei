namespace Experimental.ExtendedProperties;

// ========================================================
public static class Test_ExtendedAge
{
    [Fact]
    public static void Test_Default_Value()
    {
        var person = new Person("Bond");
        var age = person.Age;
        Assert.Equal(1, age);
    }

    [Fact]
    public static void Test_Setting_Value()
    {
        var person = new Person("Bond") { Age = 50 };
        Assert.Equal(50, person.Age);
    }

    [Fact]
    public static void Test_Collect()
    {
        GC.Collect(); GC.WaitForPendingFinalizers();
        GC.Collect(); GC.WaitForPendingFinalizers();
        var old = PersonExtensions.Size;
        
        Test_Collect_Helper();
        GC.Collect(); GC.WaitForPendingFinalizers();
        GC.Collect(); GC.WaitForPendingFinalizers();

        var num = PersonExtensions.Size;
        Assert.Equal(old, num);
    }
    static void Test_Collect_Helper()
    {
        var bond = new Person("James Bond") { Age = 50 };
        var mpenny = new Person("Miss Moneypenny") { Age = 45 };
        var leiter = new Person("Felix Leiter") { Age = 55 };
    }

    [Fact]
    public static void Test_Clear()
    {
        var bond = new Person("James Bond") { Age = 50 };
        var mpenny = new Person("Miss Moneypenny") { Age = 45 };
        var leiter = new Person("Felix Leiter") { Age = 55 };

        GC.Collect(); GC.WaitForPendingFinalizers();
        GC.Collect(); GC.WaitForPendingFinalizers();

        PersonExtensions.Clear();
        var num = PersonExtensions.Size;
        Assert.Equal(0, num);
    }
}