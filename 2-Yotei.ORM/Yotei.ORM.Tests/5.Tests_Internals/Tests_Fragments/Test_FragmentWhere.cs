namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentWhere
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => null!);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenValue>(entry.Body);
        builder = master.Visit();
        Assert.Equal("NULL", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => 50);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenValue>(entry.Body);
        builder = master.Visit();
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);

        master = new(command);
        master.Capture(x => "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
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
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("any"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
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
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x());
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenInvoke>(entry.Body);
        builder = master.Visit();
        Assert.Equal("", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(""));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Invoke_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" And any"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("any", builder.Text); // first entry with contents doesn't use connector...
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" And "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("And ", builder.Text); // stand-alone injects a tail harmless space...
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Invoke_Comparison_Alike()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" any == other "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text); // with '==' spaces are removed
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" ( any == other ) "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text); // with '==' spaces are removed
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Invoke_Setter_Alike()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" any = other "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text); // with '=' spaces are removed
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" ( any = other ) "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text); // with '=' spaces are removed
        Assert.Empty(builder.Parameters);
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
        master.Capture(x => x("any"));
        master.Capture(x => x("other"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());        
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("other", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("anyother", builder.Text); // contents just joined together
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
        master.Capture(x => x("any"));
        master.Capture(x => x(" And "));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("any And ", builder.Text); // terminal connector add a harmless space
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" And "));
        master.Capture(x => x(" any "));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal(" any ", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("And  any ", builder.Text); // body carries several spaces
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("any"));
        master.Capture(x => x(" And other"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("other", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("any And other", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("any"));
        master.Capture(x => x(" And "));
        master.Capture(x => x("other"));
        Assert.Equal(3, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        entry = Assert.IsType<FragmentWhere.Entry>(master[2]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("other", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("any And other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => null!);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenValue>(entry.Body); Assert.Equal("NULL", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("NULL", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenValue>(entry.Body); Assert.Equal("'007'", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Comparison()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == null!);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal NULL)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

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
    public static void Test_Dynamic_Setter()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null!); // Setter instead of comparison for convenience...
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal NULL)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Id = "007"); // Setter instead of comparison for convenience...
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
    public static void Test_Dynamic_Single_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.And(x.Id == null!));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal NULL)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text); // first entry doesn't use connector
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.And().Id == null!);
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Id] Equal NULL)", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text); // first entry doesn't use connector
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.And());
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("And ", builder.Text); // stand alone injects a harmless tail space
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.And<string>(x.Id = 1)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.And(x.Id = 1).y); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many_No_Connector()
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
        Assert.Equal("([First] = #0)([Last] = #1)", builder.Text); // contents just joined together
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

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And().Last == "Bond");
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

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And());
        master.Capture(x => x.Last == "Bond");
        Assert.Equal(3, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[First] Equal 'James')", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("And", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        entry = Assert.IsType<FragmentWhere.Entry>(master[2]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenBinary>(entry.Body); Assert.Equal("(x.[Last] Equal 'Bond')", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("([First] = #0) And ([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
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
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("([First] = #0) And ([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
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
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}