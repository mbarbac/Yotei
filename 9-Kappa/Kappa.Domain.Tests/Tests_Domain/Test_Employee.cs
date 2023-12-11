namespace Kappa.Domain.Tests;

// ========================================================
//[Enforced]
public static class Test_Employee
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new Employee("007", "James", "Bond");
        Assert.Equal("007", item.Id);
        Assert.Equal("James", item.Name.First);
        Assert.Equal("Bond", item.Name.Last);
        Assert.Null(item.Active);
        Assert.Null(item.Country);
        Assert.Null(item.Manager);
        Assert.Empty(item.Employees);
        Assert.Empty(item.Talents);
        Assert.Null(item.BirthDate);
        Assert.Null(item.Photo);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Manage_Talents()
    {
        var emp1 = new Employee("E1");
        var tnt1 = new Talent("T1");
        var tnt2 = new Talent("T2");

        emp1.Talents.Add(tnt1);
        Assert.Single(emp1.Talents);
        Assert.Contains(tnt1, emp1.Talents);
        Assert.Contains(emp1, tnt1.Employees);

        emp1.Talents.Add(tnt2);
        Assert.Equal(2, emp1.Talents.Count);
        Assert.Contains(tnt2, emp1.Talents);
        Assert.Contains(emp1, tnt2.Employees);

        emp1.Talents.Remove(tnt1);
        Assert.Single(emp1.Talents);
        Assert.DoesNotContain(tnt1, emp1.Talents);
        Assert.DoesNotContain(emp1, tnt1.Employees);

        tnt2.Employees.Remove(emp1);
        Assert.Empty(tnt2.Employees);
        Assert.Empty(emp1.Talents);
    }
}