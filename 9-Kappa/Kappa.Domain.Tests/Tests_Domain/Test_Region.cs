namespace Kappa.Domain.Tests;

// ========================================================
//[Enforced]
public static class Test_Region
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new Region("000");
        Assert.Equal("000", item.Id);
        Assert.Null(item.Parent);
        Assert.Empty(item.Regions);
        Assert.Empty(item.Countries);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Manage_Parent_Region()
    {
        var emea = new Region("EMEA");
        var south = new Region("SOUTH") { Parent = emea };
        Assert.Same(emea, south.Parent);
        Assert.Contains(south, emea.Regions);

        var apac = new Region("APAC");
        south.Parent = apac;
        Assert.Same(apac, south.Parent);
        Assert.Contains(south, apac.Regions);
        Assert.DoesNotContain(south, emea.Regions);

        south.Parent = null;
        Assert.Null(south.Parent);
        Assert.DoesNotContain(south, emea.Regions);
    }

    //[Enforced]
    [Fact]
    public static void Test_Manage_Child_Regions()
    {
        var emea = new Region("EMEA");
        var south = new Region("SOUTH");

        emea.Regions.Add(south);
        Assert.Same(emea, south.Parent);
        Assert.Contains(south, emea.Regions);

        var apac = new Region("APAC");
        apac.Regions.Add(south);
        Assert.Same(apac, south.Parent);
        Assert.Contains(south, apac.Regions);
        Assert.DoesNotContain(south, emea.Regions);

        apac.Regions.Remove(south);
        Assert.DoesNotContain(south, apac.Regions);
        Assert.Null(south.Parent);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Manage_Child_Countries()
    {
        var south = new Region("SOUTH");
        var spain = new Country("SP");

        south.Countries.Add(spain);
        Assert.Same(south, spain.Region);
        Assert.Contains(spain, south.Countries);

        var western = new Region("WEST");
        western.Countries.Add(spain);
        Assert.Contains(spain, western.Countries);
        Assert.DoesNotContain(spain, south.Countries);
    }
}