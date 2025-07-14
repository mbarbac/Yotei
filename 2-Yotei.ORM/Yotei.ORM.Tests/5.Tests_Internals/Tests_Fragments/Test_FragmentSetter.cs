namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_FragmentSetter
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_String()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " any = other ");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("any", entry.Target.ToString());
        Assert.IsType<DbTokenLiteral>(entry.Value); Assert.Equal("other", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_String_Rounded()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " ( any = other ) ");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("any", entry.Target.ToString());
        Assert.IsType<DbTokenLiteral>(entry.Value); Assert.Equal("other", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invoke()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" ( any = other ) "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("any", entry.Target.ToString());
        Assert.IsType<DbTokenLiteral>(entry.Value); Assert.Equal("other", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " any = {0} ", null);
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("any", entry.Target.ToString());
        Assert.IsType<DbTokenCommandInfo>(entry.Value); Assert.Equal("#0", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("(any = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => " any = {0} ", "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("any", entry.Target.ToString());
        Assert.IsType<DbTokenCommandInfo>(entry.Value); Assert.Equal("#0", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("(any = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalids()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => ""); Assert.Fail(); } // Empty literal...
        catch (ArgumentException) { }

        try { master.Capture(x => x("target=value", "other")); Assert.Fail(); } // Too many arguments...
        catch (ArgumentException) { }

        try { master.Capture(x => "any=value {0} {1}", "any"); Assert.Fail(); } // Parameters' mismatch...
        catch (ArgumentException) { }

        try { master.Capture(x => "any"); Assert.Fail(); } // Invalid 'target=value' format...
        catch (ArgumentException) { }

        try { master.Capture(x => "target="); Assert.Fail(); } // No value part...
        catch (EmptyException) { }

        try { master.Capture(x => "=value"); Assert.Fail(); } // No target part...
        catch (EmptyException) { }

        try { master.Capture(x => "{0}=value", "any"); Assert.Fail(); } // No parameters in target...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "First=James");
        master.Capture(x => " ( Last  =  Bond ) ");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("First", entry.Target.ToString());
        Assert.IsType<DbTokenLiteral>(entry.Value); Assert.Equal("James", entry.Value.ToString());
        entry = Assert.IsType<FragmentSetter.Entry>(master[1]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("Last", entry.Target.ToString());
        Assert.IsType<DbTokenLiteral>(entry.Value); Assert.Equal("Bond", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("((First = James), (Last = Bond))", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(First, Last)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(James, Bond)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "First={0}", "James");
        master.Capture(x => " Last = {0} ", "Bond");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("First", entry.Target.ToString());
        Assert.IsType<DbTokenCommandInfo>(entry.Value); Assert.Equal("#0", entry.Value.ToString());
        entry = Assert.IsType<FragmentSetter.Entry>(master[1]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("Last", entry.Target.ToString());
        Assert.IsType<DbTokenCommandInfo>(entry.Value); Assert.Equal("#0", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("((First = #0), (Last = #1))", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);

        builder = master.VisitNames();
        Assert.Equal("(First, Last)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0, #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "First={0}", null);
        master.Capture(x => " Last = {0} ", "Bond");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("First", entry.Target.ToString());
        Assert.IsType<DbTokenCommandInfo>(entry.Value); Assert.Equal("#0", entry.Value.ToString());
        entry = Assert.IsType<FragmentSetter.Entry>(master[1]);
        Assert.IsType<DbTokenLiteral>(entry.Target); Assert.Equal("Last", entry.Target.ToString());
        Assert.IsType<DbTokenCommandInfo>(entry.Value); Assert.Equal("#0", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("((First = NULL), (Last = #0))", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("Bond", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("(First, Last)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL, #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("Bond", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Null_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null!);
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenIdentifier>(entry.Target); Assert.Equal("x.[Id]", entry.Target.ToString());
        Assert.IsType<DbTokenValue>(entry.Value); Assert.Equal("NULL", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("([Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = "007");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenIdentifier>(entry.Target); Assert.Equal("x.[Id]", entry.Target.ToString());
        Assert.IsType<DbTokenValue>(entry.Value); Assert.Equal("'007'", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("([Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => 7); Assert.Fail(); } // Resolves to raw value...
        catch (ArgumentException) { }

        try { master.Capture(x => x.Id); Assert.Fail(); } // No target=value format...
        catch (ArgumentException) { }

        try { master.Capture(x => x.Id == 7); Assert.Fail(); } // Comparison is not setter...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.IsType<DbTokenIdentifier>(entry.Target); Assert.Equal("x.[First]", entry.Target.ToString());
        Assert.IsType<DbTokenValue>(entry.Value); Assert.Equal("'James'", entry.Value.ToString());
        entry = Assert.IsType<FragmentSetter.Entry>(master[1]);
        Assert.IsType<DbTokenIdentifier>(entry.Target); Assert.Equal("x.[Last]", entry.Target.ToString());
        Assert.IsType<DbTokenValue>(entry.Value); Assert.Equal("'Bond'", entry.Value.ToString());

        builder = master.Visit();
        Assert.Equal("(([First] = #0), ([Last] = #1))", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);

        builder = master.VisitNames();
        Assert.Equal("([First], [Last])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0, #1)", builder.Text);
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
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal("(([First] = #0), ([Last] = #1))", builder.Text);
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
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}