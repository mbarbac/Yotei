namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_FragmentGroupBy
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_String()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Any");
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invoke()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("Any"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "EXTRACT(YEAR FROM {0})", null);
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("EXTRACT(YEAR FROM #0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);

        builder = master.Visit();
        Assert.Equal("EXTRACT(YEAR FROM NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "EXTRACT(YEAR FROM {0})", "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("EXTRACT(YEAR FROM #0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);

        builder = master.Visit();
        Assert.Equal("EXTRACT(YEAR FROM #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalids()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master = new(command);

        try { master.Capture(x => ""); Assert.Fail(); } // Empty literal...
        catch (ArgumentException) { }

        try { master.Capture(x => "any=value {0} {1}", "any"); Assert.Fail(); } // Parameters' mismatch...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        master.Capture(x => "Ctry.Id");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("Emp.Id", entry.Body.ToString()); Assert.IsType<DbTokenLiteral>(entry.Body);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[1]);
        Assert.Equal("Ctry.Id", entry.Body.ToString()); Assert.IsType<DbTokenLiteral>(entry.Body);

        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "EXTRACT(YEAR FROM {0})", null);
        master.Capture(x => "(..WHERE [Name] <= {0})", "James");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("EXTRACT(YEAR FROM #0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[1]);
        Assert.Equal("(..WHERE [Name] <= #0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);

        builder = master.Visit();
        Assert.Equal("EXTRACT(YEAR FROM NULL), (..WHERE [Name] <= #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("James", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Any);
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("x.[Any]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Any]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Value_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Extract(x("YEAR FROM ").x(null)));
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.IsType<DbTokenMethod>(entry.Body);
        Assert.Equal("x.Extract(x(YEAR FROM )(NULL))", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Extract(YEAR FROM NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Value_Other()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Extract(x("YEAR FROM ").x("007"))); // '007' not 1st-level escaped...
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.IsType<DbTokenMethod>(entry.Body);
        Assert.Equal("x.Extract(x(YEAR FROM )('007'))", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Extract(YEAR FROM #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        master = new(command);
        master.Capture(x => x.Extract(x("YEAR FROM ").x(x("007")))); // '007' 1st-level escaped...
        Assert.Single(master);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.IsType<DbTokenMethod>(entry.Body);
        Assert.Equal("x.Extract(x(YEAR FROM )(x(007)))", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Extract(YEAR FROM 007)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => 7); Assert.Fail(); } // Resolves to raw value...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees);
        master.Capture(x => x.Ctry.Id);
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("x.[Employees]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[1]);
        Assert.Equal("x.[Ctry].[Id]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Employees], [Ctry].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        FragmentGroupBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Sum(x.Id = null));
        master.Capture(x => x.Extract(x.Name >= "James"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[0]);
        Assert.Equal("x.Sum((x.[Id] = NULL))", entry.Body.ToString()); Assert.IsType<DbTokenMethod>(entry.Body);
        entry = Assert.IsType<FragmentGroupBy.Entry>(master[1]);
        Assert.Equal("x.Extract((x.[Name] GreaterThanOrEqual 'James'))", entry.Body.ToString()); Assert.IsType<DbTokenMethod>(entry.Body);

        builder = master.Visit();
        Assert.Equal("Sum(([Id] = NULL)), Extract(([Name] >= #0))", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("James", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Sum(x.Id = null));
        master.Capture(x => x.Extract(x.Name >= "James"));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("Sum(([Id] = NULL)), Extract(([Name] >= #0))", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("James", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Sum(x.Id = null));
        master.Capture(x => x.Extract(x.Name >= "James"));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}