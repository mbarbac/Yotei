namespace Kappa.Traverse.Tests;

// ====================================================
//[Enforced]
public static class Test_Region
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new Region();
        Assert.Null(item.Parent);
        Assert.Empty(item.Regions);
        Assert.Empty(item.Countries);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Set_Parent_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var south = new Region() { Id = "SOUTH", Parent = emea };
        Assert.Same(emea, south.Parent);
        Assert.Contains(south, emea.Regions);

        south.Parent = null;
        Assert.Null(south.Parent);
        Assert.DoesNotContain(south, emea.Regions);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Parent_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var south = new Region() { Id = "SOUTH", Parent = emea };
        Assert.Same(emea, south.Parent);
        Assert.Contains(south, emea.Regions);

        var amer = new Region() { Id = "AMER" };
        south.Parent = amer;
        Assert.Same(amer, south.Parent);
        Assert.Contains(south, amer.Regions);
        Assert.DoesNotContain(south, emea.Regions);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Remove_Child_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var south = new Region() { Id = "SOUTH" };

        emea.Regions.Add(south);
        Assert.Same(emea, south.Parent);
        Assert.Contains(south, emea.Regions);

        emea.Regions.Remove(south);
        Assert.Null(south.Parent);
        Assert.DoesNotContain(south, emea.Regions);
    }

    //[Enforced]
    [Fact]
    public static void Test_Move_Child_Region()
    {
        var emea = new Region() { Id = "EMEA" };
        var south = new Region() { Id = "SOUTH" };

        emea.Regions.Add(south);
        Assert.Same(emea, south.Parent);
        Assert.Contains(south, emea.Regions);

        var amer = new Region() { Id = "AMER" };
        amer.Regions.Add(south);
        Assert.Same(amer, south.Parent);
        Assert.Contains(south, amer.Regions);
        Assert.DoesNotContain(south, emea.Regions);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Remove_Child_Country()
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
    public static void Test_Move_Child_Country()
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
}