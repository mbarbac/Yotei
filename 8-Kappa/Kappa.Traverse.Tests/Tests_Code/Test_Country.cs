namespace Kappa.Traverse.Tests;

// ====================================================
//[Enforced]
public static class Test_Country
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new Country();
        Assert.Null(item.Region);
        Assert.Empty(item.Employees);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Set_Parent_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var spain = new Country() { Id = "Spain", Region = emea };
        Assert.Same(emea, spain.Region);
        Assert.Contains(spain, emea.Countries);

        spain.Region = null;
        Assert.Null(spain.Region);
        Assert.DoesNotContain(spain, emea.Countries);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Parent_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var spain = new Country() { Id = "Spain", Region = emea };
        Assert.Same(emea, spain.Region);
        Assert.Contains(spain, emea.Countries);

        var amer = new Region() { Id = "AMER" };
        spain.Region = amer;
        Assert.Same(amer, spain.Region);
        Assert.Contains(spain, amer.Countries);
        Assert.DoesNotContain(spain, emea.Countries);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Remove_Child_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var spain = new Country() { Id = "Spain" };

        emea.Countries.Add(spain);
        Assert.Same(emea, spain.Region);
        Assert.Contains(spain, emea.Countries);

        emea.Countries.Remove(spain);
        Assert.Null(spain.Region);
        Assert.DoesNotContain(spain, emea.Countries);
    }

    //[Enforced]
    [Fact]
    public static void Test_Move_Child_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var spain = new Country() { Id = "Spain" };

        emea.Countries.Add(spain);
        Assert.Same(emea, spain.Region);
        Assert.Contains(spain, emea.Countries);

        var amer = new Region() { Id = "AMER" };
        amer.Countries.Add(spain);
        Assert.Same(amer, spain.Region);
        Assert.Contains(spain, amer.Countries);
        Assert.DoesNotContain(spain, emea.Countries);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Remove_Child_Employee()
    {
        var spain = new Country() { Id = "Spain" };
        var cervantes = new Employee("01", "Miguel", "De Cervantes");

        spain.Employees.Add(cervantes);
        Assert.Same(spain, cervantes.Country);
        Assert.Contains(cervantes, spain.Employees);

        spain.Employees.Remove(cervantes);
        Assert.Null(cervantes.Country);
        Assert.DoesNotContain(cervantes, spain.Employees);
    }

    //[Enforced]
    [Fact]
    public static void Test_Move_Child_Employee()
    {
        var spain = new Country() { Id = "Spain" };
        var cervantes = new Employee("01", "Miguel", "De Cervantes");

        spain.Employees.Add(cervantes);
        Assert.Same(spain, cervantes.Country);
        Assert.Contains(cervantes, spain.Employees);

        var portugal = new Country() { Id = "Portugal" };
        portugal.Employees.Add(cervantes);
        Assert.Same(portugal, cervantes.Country);
        Assert.Contains(cervantes, portugal.Employees);
        Assert.DoesNotContain(cervantes, spain.Employees);
    }
}