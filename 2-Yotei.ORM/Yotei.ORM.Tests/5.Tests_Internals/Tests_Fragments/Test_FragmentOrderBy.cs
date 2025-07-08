namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentOrderBy
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => null!);
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]); Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenValue>(entry.Body);
        builder = master.Visit();
        Assert.Equal("NULL", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => 50);
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]); Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenValue>(entry.Body);
        builder = master.Visit();
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);

        master = new(command);
        master.Capture(x => "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]); Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenValue>(entry.Body);
        builder = master.Visit();
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Invoke()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("any"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]); Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Invoke_Empty()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x());
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]); Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenInvoke>(entry.Body);
        builder = master.Visit();
        Assert.Equal("", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(""));
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]); Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Invoke_Ordering()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" any Ascending"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Ascending", entry.Ordering);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("any Ascending", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" desc "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("desc", entry.Ordering);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("desc", builder.Text); // stand-alone, only for special cases...
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.Any.Desc(x.Other)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("Emp.Id"));
        master.Capture(x => x("Ctry.Id Asc"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Emp.Id", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Equal("Asc", entry.Ordering);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Ctry.Id", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id Asc", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id);
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Ordering()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id.Desc());
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Desc", entry.Ordering);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Id] Desc", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.Descending(x.Whatever)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id);
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Emp].[Id]", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Null(entry.Ordering);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Ctry].[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_Ordering()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.Desc());
        master.Capture(x => x.Ctry.Id.Asc());
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Desc", entry.Ordering);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Emp].[Id]", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Equal("Asc", entry.Ordering);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Ctry].[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Emp].[Id] Desc, [Ctry].[Id] Asc", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.Desc());
        master.Capture(x => x.Ctry.Id.Asc());
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("[Emp].[Id] Desc, [Ctry].[Id] Asc", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.Desc());
        master.Capture(x => x.Ctry.Id.Asc());
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}