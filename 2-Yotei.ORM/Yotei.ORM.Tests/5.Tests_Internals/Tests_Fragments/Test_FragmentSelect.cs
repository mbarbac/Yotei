namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_FragmentSelect
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_String()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Any");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_String_AllColumns()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Any.*");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any.*", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_String_Alias()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Any as Other");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Equal("Other", entry.Alias);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any as Other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_String_AllColumns_Alias()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Any.* As Other");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Equal("Other", entry.Alias);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any.* As Other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invoke()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("Any.* As Other"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Equal("Other", entry.Alias);
        Assert.Equal("Any", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Any.* As Other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "SUM(...[Id]={0}) AS [Total]", null);
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Total]", entry.Alias);
        Assert.Equal("SUM(...[Id]=#0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);

        builder = master.Visit();
        Assert.Equal("SUM(...[Id]=NULL) AS [Total]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "SUM(...[Id]={0}).* AS [Total]", "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Equal("[Total]", entry.Alias);
        Assert.Equal("SUM(...[Id]=#0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);

        builder = master.Visit();
        Assert.Equal("SUM(...[Id]=#0).* AS [Total]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalids()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

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
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "[Employees].*");
        master.Capture(x => "[Ctry].[Id] As [Id]");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("[Employees]", entry.Body.ToString()); Assert.IsType<DbTokenLiteral>(entry.Body);
        entry = Assert.IsType<FragmentSelect.Entry>(master[1]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Id]", entry.Alias);
        Assert.Equal("[Ctry].[Id]", entry.Body.ToString()); Assert.IsType<DbTokenLiteral>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Employees].*, [Ctry].[Id] As [Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "SUM(...[Id]={0})", null);
        master.Capture(x => "(..WHERE [Name] <= {0}).* AS [Emps]", "James");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("SUM(...[Id]=#0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);
        entry = Assert.IsType<FragmentSelect.Entry>(master[1]);
        Assert.True(entry.AllColumns);
        Assert.Equal("[Emps]", entry.Alias);
        Assert.Equal("(..WHERE [Name] <= #0)", entry.Body.ToString()); Assert.IsType<DbTokenCommandInfo>(entry.Body);

        builder = master.Visit();
        Assert.Equal("SUM(...[Id]=NULL), (..WHERE [Name] <= #0).* AS [Emps]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("James", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Any);
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("x.[Any]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Any]", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Any.As(x.Other));
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Other]", entry.Alias);
        Assert.Equal("x.[Any]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Any] As [Other]", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Any.All());
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("x.[Any]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Any].*", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Any.As(x.Other).All());
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Equal("[Other]", entry.Alias);
        Assert.Equal("x.[Any]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Any].* As [Other]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Value_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Average(x("Where ").Sum(x.Id == null)));
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.IsType<DbTokenMethod>(entry.Body);
        Assert.Equal("x.Average(x(Where ).Sum((x.[Id] Equal NULL)))", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Average(Where Sum(([Id] IS NULL)))", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Value_Other()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Average(x("Where ").Sum(x.Id == "007")));
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.IsType<DbTokenMethod>(entry.Body);
        Assert.Equal("x.Average(x(Where ).Sum((x.[Id] Equal '007')))", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Average(Where Sum(([Id] = #0)))", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => 7); Assert.Fail(); } // Resolves to raw value...
        catch (ArgumentException) { }

        try { master.Capture(x => x.As()); Assert.Fail(); } // Empty alias...
        catch (ArgumentException) { }

        try { master.Capture(x => x.As(x.Any)); Assert.Fail(); } // No main part...
        catch (ArgumentException) { }

        try { master.Capture(x => x.All(x.Any)); Assert.Fail(); } // No arguments in all...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Ctry.Id.As(x.Any));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("x.[Employees]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);
        entry = Assert.IsType<FragmentSelect.Entry>(master[1]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Any]", entry.Alias);
        Assert.Equal("x.[Ctry].[Id]", entry.Body.ToString()); Assert.IsType<DbTokenIdentifier>(entry.Body);

        builder = master.Visit();
        Assert.Equal("[Employees].*, [Ctry].[Id] As [Any]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Sum(x.Id = null).As(x.Total));
        master.Capture(x => x.Where(x.Name >= "James").All());
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Total]", entry.Alias);
        Assert.Equal("x.Sum((x.[Id] = NULL))", entry.Body.ToString()); Assert.IsType<DbTokenMethod>(entry.Body);
        entry = Assert.IsType<FragmentSelect.Entry>(master[1]);
        Assert.True(entry.AllColumns);
        Assert.Null(entry.Alias);
        Assert.Equal("x.Where((x.[Name] GreaterThanOrEqual 'James'))", entry.Body.ToString()); Assert.IsType<DbTokenMethod>(entry.Body);

        builder = master.Visit();
        Assert.Equal("Sum(([Id] = NULL)) As [Total], Where(([Name] >= #0)).*", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("James", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Sum(x.Id = null).As(x.Total));
        master.Capture(x => x.Where(x.Name >= "James").All());
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("Sum(([Id] = NULL)) As [Total], Where(([Name] >= #0)).*", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("James", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Sum(x.Id = null).As(x.Total));
        master.Capture(x => x.Where(x.Name >= "James").All());
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}