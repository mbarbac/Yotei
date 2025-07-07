using System.Xml.Schema;

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
    public static void Test_Literal_From_Invoke_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        FragmentWhere.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x(" And any"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.NotNull(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("And any", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x(" And "));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.NotNull(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("And", builder.Text);
        Assert.Empty(builder.Parameters);
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
        master.Capture(x => x("any == other"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("(any == other)"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
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
        master.Capture(x => x("any = other"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("(any = other)"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]); Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
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
    public static void Test_Literal_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Arbitrary()
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
        Assert.Equal("anyother", builder.Text);
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
        master.Capture(x => x("Or"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("any Or", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("any"));
        master.Capture(x => x("Or"));
        master.Capture(x => x("other"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("other", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("any Or other", builder.Text);
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
        master.Capture(x => x("Or"));
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Empty(entry.Body.ToString()!);
        builder = master.Visit();
        Assert.Equal("any Or", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("any"));
        master.Capture(x => x("Or"));
        master.Capture(x => x("other")); // Consolidates with previous one.
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentWhere.Entry>(master[0]);
        Assert.Null(entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("any", entry.Body.ToString());
        entry = Assert.IsType<FragmentWhere.Entry>(master[1]);
        Assert.Equal("Or", entry.Connector);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("other", entry.Body.ToString());
        builder = master.Visit();
        Assert.Equal("any Or other", builder.Text);
        Assert.Empty(builder.Parameters);
    }
}