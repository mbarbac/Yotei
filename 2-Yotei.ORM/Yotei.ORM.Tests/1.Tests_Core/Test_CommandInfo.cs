namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public void Test_Create_Empty()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        info = new CommandInfo(connection);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, null);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, "");
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, null, []);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, "", []);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Only_Text()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        info = new CommandInfo(connection, "SELECT");
        Assert.False(info.IsEmpty);
        Assert.Equal("SELECT", info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsConsistent);

        try { _ = new CommandInfo(connection, "{any}"); Assert.Fail(); } // Dangling spec...
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Only_Values()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        info = new CommandInfo(connection, null, null);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.False(info.IsConsistent);

        info = new CommandInfo(connection, null, "any", new Parameter("First", "James"), new { Last = "Bond" });
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("any", info.Parameters[0].Value);
        Assert.Equal("#First", info.Parameters[1].Name); Assert.Equal("James", info.Parameters[1].Value);
        Assert.Equal("#Last", info.Parameters[2].Name); Assert.Equal("Bond", info.Parameters[2].Value);
        Assert.False(info.IsConsistent);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Text_And_Values()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        info = new CommandInfo(connection, "{0}", null);
        Assert.False(info.IsEmpty);
        Assert.Equal("#0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        Assert.False(info.IsEmpty);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.True(info.IsConsistent);

        try { _ = new CommandInfo(connection, "{0}", "James", "Bond"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new CommandInfo(connection, "{0} {1}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Text_And_Parameters()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("First", "James");
        var plast = new Parameter("#Last", "Bond");

        info = new CommandInfo(connection, "{0} {1}", pfirst, plast);
        Assert.False(info.IsEmpty);
        Assert.Equal("#First #Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, "{#First} {Last}", pfirst, plast);
        Assert.False(info.IsEmpty);
        Assert.Equal("#First #Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.True(info.IsConsistent);

        try { _ = new CommandInfo(connection, "{0}", pfirst, plast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new CommandInfo(connection, "{0} {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Text_And_Anonymous()
    {
        ICommandInfo info;
        var engine = new FakeEngine() { ParameterPrefix = "@" };
        var connection = new FakeConnection(engine);

        var xfirst = new { First = "James" };
        var xlast = new { @Last = "Bond" };

        info = new CommandInfo(connection, "{0} {1}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("@First @Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("@First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("@Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.True(info.IsConsistent);

        info = new CommandInfo(connection, "{@First} {Last}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("@First @Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("@First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("@Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.True(info.IsConsistent);

        try { _ = new CommandInfo(connection, "{0}", xfirst, xlast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new CommandInfo(connection, "{0} {1}", xfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Single_Value_As_Collection()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        info = new CommandInfo(connection, "{0} {1}", [null, "any"]);
        Assert.False(info.IsEmpty);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("any", info.Parameters[1].Value);
        Assert.True(info.IsConsistent);

        object?[] array = { null, "any" };
        info = new CommandInfo(connection, "{0}", new Parameter("#0", array));
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name);
        array = Assert.IsType<object?[]>(info.Parameters[0].Value);
        Assert.Equal(2, array.Length);
        Assert.Null(array[0]);
        Assert.Equal("any", array[1]);
        Assert.True(info.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Second_Value_As_Collection()
    {
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        info = new CommandInfo(connection, "{0} {1}", null, new object?[] { null, "any" });
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name);
        var array = Assert.IsType<object?[]>(info.Parameters[1].Value);
        Assert.Equal(2, array.Length);
        Assert.Null(array[0]);
        Assert.Equal("any", array[1]);
        Assert.True(info.IsConsistent);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Duplicated_Names()
    {
        ICommandInfo info;
        var engine = new FakeEngine() { ParameterPrefix = "@" };
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("One", "James");
        var plast = new Parameter("@One", "Bond");
        try { info = new CommandInfo(connection, "{0} {1}", pfirst, plast); Assert.Fail(); }
        catch (DuplicateException) { }

        var xfirst = new { One = "James" };
        var xlast = new { @One = "Bond" };
        try { info = new CommandInfo(connection, "{0} {1}", xfirst, xlast); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clone()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new CommandInfo(connection);
        target = source.Clone();
        Assert.True(target.IsEmpty);
        Assert.Empty(target.Text);
        Assert.Empty(target.Parameters);
        Assert.True(target.IsConsistent);

        source = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        target = source.Clone();
        Assert.False(target.IsEmpty);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public void Test_Add_Command()

    //[Enforced]
    //[Fact]
    //public void Test_Add_Command_Inconsistent()

    //[Enforced]
    //[Fact]
    //public void Test_Add_CommandInfo()

    //[Enforced]
    //[Fact]
    //public void Test_Add_CommandInfo_Inconsistent()

    //[Enforced]
    //[Fact]
    //public void Test_Add_CommandInfoBuilder()

    //[Enforced]
    //[Fact]
    //public void Test_Add_CommandInfoBuilder_Inconsistent()

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Add_Only_Text()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new CommandInfo(connection);
        target = source.Add((string?)null);
        Assert.Same(source, target);

        source = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        target = source.Add((string?)null);
        Assert.Same(source, target);

        target = source.Add(string.Empty);
        Assert.Same(source, target);

        target = source.Add(" any");
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1 any", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);

        try { target.Add("{any}"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { target.Add("#any"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Add_Only_Values()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new CommandInfo(connection);
        target = source.Add(null, []);
        Assert.Same(source, target);

        source = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        target = source.Add(null, "any");
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("any", target.Parameters[2].Value);
        Assert.False(target.IsConsistent);

        target = source.Add(null, new Parameter("#0", "three"));
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("three", target.Parameters[2].Value);
        Assert.False(target.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public void Test_Add_Text_And_Values()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new CommandInfo(connection);
        target = source.Add("{0} {1}", "James", "Bond");
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);

        source = target;
        target = source.Add(" {0} {1}", null, 50);
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1 #2 #3", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
        Assert.Equal("#3", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
        Assert.True(target.IsConsistent);

        try { source.Add("{0}", "James", "Bond"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("{0} {1}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Add_Text_And_Parameters()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("First", "James");
        var plast = new Parameter("#Last", "Bond");

        source = new CommandInfo(connection);
        target = source.Add("{0} {1}", pfirst, plast);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);

        var pbranch = new Parameter("Branch", null);
        var page = new Parameter("#Age", 50);

        source = target;
        target = source.Add(" {Branch} {Age}", pbranch, page);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last #Branch #Age", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Branch", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
        Assert.Equal("#Age", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
        Assert.True(target.IsConsistent);

        try { source.Add("{0}", pfirst, plast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("{0} {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Add_Text_And_Anonymous()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };

        source = new CommandInfo(connection);
        target = source.Add("{0} {1}", xfirst, xlast);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);

        var xbranch = new { Branch = (string?)null };
        var xage = new { Age = 50 };

        source = target;
        target = source.Add(" {Branch} {Age}", xbranch, xage);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last #Branch #Age", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Branch", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
        Assert.Equal("#Age", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
        Assert.True(target.IsConsistent);

        try { source.Add("{0}", xfirst, xlast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("{0} {1}", xfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Replace_Text()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new CommandInfo(connection);
        target = source.ReplaceText(string.Empty);
        Assert.Same(source, target);

        source = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        target = source.ReplaceText("");
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);

        target = source.ReplaceText("{3}");
        Assert.NotSame(source, target);
        Assert.Equal("{3}", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);

        target = source.ReplaceText("#3");
        Assert.NotSame(source, target);
        Assert.Equal("#3", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Replace_Values()
    {
        ICommandInfo source, target;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new CommandInfo(connection);
        target = source.ReplaceParameters([]);
        Assert.Same(source, target);

        source = new CommandInfo(connection, "{0}", "James");
        target = source.ReplaceParameters([]);
        Assert.NotSame(source, target);
        Assert.Equal("#0", target.Text);
        Assert.Empty(target.Parameters);
        Assert.False(target.IsConsistent);

        source = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        target = source.ReplaceParameters([]);
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Empty(target.Parameters);
        Assert.False(target.IsConsistent);

        target = source.ReplaceParameters(null);
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#2", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.False(target.IsConsistent);

        target = source.ReplaceParameters(new Parameter("#1", 50));
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#2", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
        Assert.False(target.IsConsistent);

        target = source.ReplaceParameters(new Parameter("Age", 50));
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#Age", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
        Assert.False(target.IsConsistent);

        target = source.ReplaceParameters(new { Age = 50 });
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#Age", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
        Assert.False(target.IsConsistent);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clear()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var source = new CommandInfo(connection);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new CommandInfo(connection, "{0} {1}", "James", "Bond");
        target = source.Clear();
        Assert.True(target.IsEmpty);
        Assert.Empty(target.Text);
        Assert.Empty(target.Parameters);
    }
}