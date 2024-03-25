namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);
        Assert.True(info.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "whatever");

        Assert.False(info.IsEmpty);
        Assert.Equal("whatever", info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Id]={Id}, [First]={#First}, [Last]={1}",
            new { Id = "007" },
            "Bond",
            new Parameter("First", "James"));

        Assert.False(info.IsEmpty);
        Assert.Equal("[Id]=#Id, [First]=#First, [Last]=#2", info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#Id", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#First", info.Parameters[1].Name); Assert.Equal("James", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal("Bond", info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}, [First]={1}", "007", "James");
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("James", target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddText()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");

        var target = source.AddText(null);
        Assert.Same(source, target);

        target = source.AddText("xxx");
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0xxx", target.Text);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddParameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");

        var target = source.AddParameters();
        Assert.Same(source, target);

        target = source.AddParameters(new Parameter("#1", "James"), new Parameter("#2", "Bond"));
        Assert.NotSame(source, target);
        Assert.Equal(source.Text, target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("James", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("Bond", target.Parameters[2].Value);

        try { source.AddParameters(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddParameters([null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddParameters(new Parameter("#0", "any")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Arguments()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");

        var target = source.Add((string?)null);
        Assert.Same(source, target);

        target = source.Add(", [First]={First}, [Last]={Last}");
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]={First}, [Last]={Last}", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("007", target.Parameters[0].Value);

        target = source.Add(", [First]={0}, [Last]={Last}", "James");
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#1, [Last]={Last}", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("James", target.Parameters[1].Value);

        target = source.Add(", [First]={First}, [Last]={0}", "Bond", new { First = "James" });
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#First, [Last]=#2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("James", target.Parameters[1].Value);
        Assert.Equal("Bond", target.Parameters[2].Value);

        target = source.Add(", [First]={1}, [Last]={Last}", new Parameter("#Last", "Bond"), "James");
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#1, [Last]=#Last", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("James", target.Parameters[1].Value);
        Assert.Equal("Bond", target.Parameters[2].Value);

        try { source.Add(" ", "any"); Assert.Fail(); } // Argument not used...
        catch (ArgumentException) { }

        try { source.Add(", [First]={-1}", "James"); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.Add(", [First]={1}", "James"); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Source()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");

        var other = new CommandInfo(engine);
        var target = source.Add(other);
        Assert.Same(source, target);

        target = source.Add("", other);
        Assert.Same(source, target);

        target = source.Add("x", other);
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0x", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("007", target.Parameters[0].Value);

        other = new CommandInfo(engine, "[First]={0}", "James");
        target = source.Add(", ", other);
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("James", target.Parameters[1].Value);
    }
}