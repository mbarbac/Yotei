namespace Experimental.ExtendedProperties.Tests;

// ========================================================
//[Enforced]
public static class Test_ExtendedProperty
{
    //[Enforced]
    [Fact]
    public static void Test_Extended_Property()
    {        
        Assert.Equal(0, PersonExtensions.Size);
        try
        {
            var person = new Person("James"); Assert.Null(person.Nickname);
            person.Nickname = "007"; Assert.Equal("007", person.Nickname);

            person = new Person("Paul"); Assert.Null(person.Nickname);
            person.Nickname = "008"; Assert.Equal("008", person.Nickname);

            Assert.Equal(2, PersonExtensions.Size);

            person = null;
            GC.Collect(); GC.WaitForPendingFinalizers();
            GC.Collect(); GC.WaitForPendingFinalizers();
        }
        finally { }
        Assert.Equal(2, PersonExtensions.Size);
    }
}