namespace Kappa.Traverse.Tests;

// ====================================================
//[Enforced]
public static class Test_Employee
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new Employee();
        Assert.Null(item.Country);
        Assert.Null(item.Manager);
        Assert.Empty(item.Employees);
        Assert.Empty(item.Talents);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Set_Parent_Country()
    {
        var spain = new Country() { Id = "Spain" };
        var emp = new Employee() { Country = spain };
        Assert.Same(emp.Country, spain);
        Assert.Contains(emp, spain.Employees);

        emp.Country = null;
        Assert.Null(emp.Country);
        Assert.DoesNotContain(emp, spain.Employees);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Country()
    {
        var spain = new Country() { Id = "Spain" };
        var emp = new Employee() { Country = spain };
        Assert.Same(emp.Country, spain);
        Assert.Contains(emp, spain.Employees);

        var portugal = new Country() { Id = "Portugal" };
        emp.Country = portugal;
        Assert.Same(portugal, emp.Country);
        Assert.Contains(emp, portugal.Employees);
        Assert.DoesNotContain(emp, spain.Employees);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Remove_Child_Employee()
    {
        var mgr = new Employee() { Id = "Manager" };
        var emp = new Employee() { Id = "Employee" };

        mgr.Employees.Add(emp);
        Assert.Same(mgr, emp.Manager);
        Assert.Contains(emp, mgr.Employees);

        mgr.Employees.Remove(emp);
        Assert.Null(emp.Manager);
        Assert.DoesNotContain(emp, mgr.Employees);
    }

    //[Enforced]
    [Fact]
    public static void Test_Move_Child_Employee()
    {
        var mgr = new Employee() { Id = "Manager" };
        var emp = new Employee() { Id = "Employee" };

        mgr.Employees.Add(emp);
        Assert.Same(mgr, emp.Manager);
        Assert.Contains(emp, mgr.Employees);

        var other = new Employee() { Id = "Other" };
        other.Employees.Add(emp);
        Assert.Same(other, emp.Manager);
        Assert.Contains(emp, other.Employees);
        Assert.DoesNotContain(emp, mgr.Employees);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Talent()
    {
        var emp = new Employee() { Id = "Employee" };
        var tnt = new Talent() { Id = "IT" };

        emp.Talents.Add(tnt);
        Assert.Contains(tnt, emp.Talents);
        Assert.Contains(emp, tnt.Employees);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Employee_Talent()
    {
        var emp = new Employee() { Id = "Employee" };
        var tnt = new Talent() { Id = "IT" };
        
        emp.Talents.Add(tnt);
        Assert.Contains(tnt, emp.Talents);
        Assert.Contains(emp, tnt.Employees);

        emp.Talents.Remove(tnt);
        Assert.DoesNotContain(tnt, emp.Talents);
        Assert.DoesNotContain(emp, tnt.Employees);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Talent_Employee()
    {
        var emp = new Employee() { Id = "Employee" };
        var tnt = new Talent() { Id = "IT" };

        emp.Talents.Add(tnt);
        Assert.Contains(tnt, emp.Talents);
        Assert.Contains(emp, tnt.Employees);

        tnt.Employees.Remove(emp);
        Assert.DoesNotContain(tnt, emp.Talents);
        Assert.DoesNotContain(emp, tnt.Employees);
    }
}