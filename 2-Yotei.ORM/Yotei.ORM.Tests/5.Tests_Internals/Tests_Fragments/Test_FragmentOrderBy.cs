namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_FragmentOrderBy
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_From_String()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " Id ");
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Id", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Id", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => " Id Desc ");
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Desc", entry.Order);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Id", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Id Desc", builder.Text);
        Assert.Empty(builder.Parameters);
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
        master.Capture(x => x(" Id "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Id", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Id", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" Id Desc "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Desc", entry.Order);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Id", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Id Desc", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Id {0}", "any"); // Just an example, no SQL sense...
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id #0)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Id #0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("any", builder.Parameters[0].Value);

        master = new(command);
        master.Capture(x => "Id {0} Asc", "any");
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Asc", entry.Order);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id #0)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Id #0 Asc", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("any", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalids()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master = new(command);

        try { master.Capture(x => "Asc"); Assert.Fail(); } // Resolves to raw ordering...
        catch (ArgumentException) { }

        try { master.Capture(x => "value {0} {1}", "any"); Assert.Fail(); } // Parameters' mismatch...
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
        master.Capture(x => "Emp.Id");
        master.Capture(x => "Ctry.Id Asc");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Emp.Id", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Equal("Asc", entry.Order);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Ctry.Id", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id Asc", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id {0}", "any");
        master.Capture(x => "Ctry.Id {0} Asc", "other");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Emp.Id #0)", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Equal("Asc", entry.Order);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Ctry.Id #0)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("Emp.Id #0, Ctry.Id #1 Asc", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("any", builder.Parameters[0].Value);
        Assert.Equal("other", builder.Parameters[1].Value);
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
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Order()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id.Descending());
        Assert.Single(master);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Descending", entry.Order);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Id] Descending", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => 7); Assert.Fail(); } // Resolves to raw value...
        catch (ArgumentException) { }

        try { master.Capture(x => x.Desc()); Assert.Fail(); } // Resolves to order only...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

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
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Emp].[Id]", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Null(entry.Order);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Ctry].[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_Order()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        FragmentOrderBy.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.Asc());
        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[0]);
        Assert.Equal("Asc", entry.Order);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Emp].[Id]", entry.Body.ToString());
        entry = Assert.IsType<FragmentOrderBy.Entry>(master[1]);
        Assert.Equal("Desc", entry.Order);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Ctry].[Id]", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("[Emp].[Id] Asc, [Ctry].[Id] Desc", builder.Text);
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
        master.Capture(x => x.Emp.Id.Asc());
        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("[Emp].[Id] Asc, [Ctry].[Id] Desc", builder.Text);
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
        master.Capture(x => x.Emp.Id.Asc());
        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}