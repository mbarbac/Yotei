namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_FragmentWhere
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_String()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " Id = null "); // Accepting '=' for SQL-alike convenience...
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = null)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = null)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => " Id == '007' "); // Accepting '==' for matching C#...
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = '007')", builder.Text);
        Assert.Empty(builder.Parameters);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Literal_String_Rounded()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " ( Id = null ) "); // Accepting '=' for SQL-alike convenience...
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = null)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = null)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => " ( Id == '007' ) "); // Accepting '==' for matching C#...
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = '007')", builder.Text);
        Assert.Empty(builder.Parameters);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Literal_String_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " And Id = '007' ");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = '007')", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invoke()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" Id = null "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = null)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = null)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" Id == '007' "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id = '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = '007')", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " Id = {0} ", null);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id = #0)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => " ( Id == {0} ) ", null);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id = #0)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " Id = {0} ", "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id = #0)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        master = new(command);
        master.Capture(x => " ( Id == {0} ) ", "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id = #0)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " or ( Id = {0} ) ", "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("or", entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id = #0)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Id = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalids()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => "any=value {0} {1}", "any"); Assert.Fail(); } // Parameters' mismatch...
        catch (ArgumentException) { }

        try { master.Capture(x => "And"); Assert.Fail(); } // Connector-only...
        catch (EmptyException) { }

        try { master.Capture(x => " Or "); Assert.Fail(); } // Connector-only...
        catch (EmptyException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Name = null");
        master.Capture(x => "Id != '007'");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Name = null)", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id <> '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Name = null)(Id <> '007')", builder.Text); // Just concatenated...
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Or Name = null");
        master.Capture(x => "And Id != '007'");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Name = null)", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("(Id <> '007')", entry.Body.ToString());

        // Note that 1st entry does not use its connector...
        builder = master.Visit();
        Assert.Equal("(Name = null) And (Id <> '007')", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Or Name = {0}", null);
        master.Capture(x => "   And ( Id != {0} ) ", "007");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Name = #0)", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("(Id <> #0)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("(Name = NULL) And (Id <> #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Null_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == null);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal NULL)", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Setter_To_Comparison()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.And(x.Id == "007"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        master = new(command);
        master.Capture(x => x.Or().Id == "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal '007')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => 7); Assert.Fail(); } // Resolves to raw value...
        catch (ArgumentException) { }

        try { master.Capture(x => x.And()); Assert.Fail(); } // Connector-only...
        catch (ArgumentException) { }

        try { master.Capture(x => x.And(x.Id = 1).y); Assert.Fail(); } // Remaining not empty...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.Last == "Bond");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[First] Equal 'James')", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Last] Equal 'Bond')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([First] = #0)([Last] = #1)", builder.Text); // Just concatenated ^^!
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_With_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[First] Equal 'James')", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Last] Equal 'Bond')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([First] = #0) And ([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_Parametrized()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == null);
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[First] Equal NULL)", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Last] Equal 'Bond')", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("([First] IS NULL) And ([Last] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("Bond", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == null);
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("([First] IS NULL) And ([Last] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("Bond", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == null);
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}