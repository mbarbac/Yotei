namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_CommandInfo_Builder
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);

        Assert.Equal("", source.ToString());
        Assert.True(source.IsEmpty);
        Assert.True(source.IsConsistent);
        Assert.False(source.IsValid);

        source = new CommandInfo(engine, null);
        Assert.True(source.IsEmpty);
        Assert.True(source.IsConsistent);
        Assert.False(source.IsValid);

        source = new CommandInfo(engine, "", []);
        Assert.True(source.IsEmpty);
        Assert.True(source.IsConsistent);
        Assert.False(source.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_Only()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "any");

        Assert.Equal("any", source.ToString());
        Assert.False(source.IsEmpty);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        Assert.Equal("any #0 #1", source.Text);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#0", source.Parameters[0].Name); Assert.Equal("James", source.Parameters[0].Value);
        Assert.Equal("#1", source.Parameters[1].Name); Assert.Equal("Bond", source.Parameters[1].Value);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);

        try { source = new CommandInfo(engine, "any {0} {1}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source = new CommandInfo(engine, "any {0}", "James", "Bond"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source = new CommandInfo(engine, "any {9}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_Parameters()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");

        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);
        Assert.Equal("any #First #Last", source.Text);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#First", source.Parameters[0].Name); Assert.Equal("James", source.Parameters[0].Value);
        Assert.Equal("#Last", source.Parameters[1].Name); Assert.Equal("Bond", source.Parameters[1].Value);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);

        source = new CommandInfo(engine, "any {0} {1}", xfirst, xlast);
        Assert.Equal("any #First #Last", source.Text);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#First", source.Parameters[0].Name); Assert.Equal("James", source.Parameters[0].Value);
        Assert.Equal("#Last", source.Parameters[1].Name); Assert.Equal("Bond", source.Parameters[1].Value);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_Anonymous()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };

        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);
        Assert.Equal("any #First #Last", source.Text);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#First", source.Parameters[0].Name); Assert.Equal("James", source.Parameters[0].Value);
        Assert.Equal("#Last", source.Parameters[1].Name); Assert.Equal("Bond", source.Parameters[1].Value);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);

        source = new CommandInfo(engine, "any {0} {1}", xfirst, xlast);
        Assert.Equal("any #First #Last", source.Text);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#First", source.Parameters[0].Name); Assert.Equal("James", source.Parameters[0].Value);
        Assert.Equal("#Last", source.Parameters[1].Name); Assert.Equal("Bond", source.Parameters[1].Value);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var target = source.Add((string?)null); Assert.Same(source, target);
        target = source.Add(null, []); Assert.Same(source, target);
        target = source.Add("", []); Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Only()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var target = source.Add(" other");
        Assert.NotSame(source, target);
        Assert.Equal("any #First #Last other", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Only_Inconsistent()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var target = source.Add(" other #0");
        Assert.NotSame(source, target);
        Assert.Equal("any #First #Last other #0", target.Text);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);

        target = source.Add(" other #2");
        Assert.NotSame(source, target);
        Assert.Equal("any #First #Last other #2", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);

        target = source.Add(" other {Last}");
        Assert.NotSame(source, target);
        Assert.Equal("any #First #Last other {Last}", target.Text);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Values_Only_Inconsistent()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var target = source.Add(null, 50, "MI6");
        Assert.NotSame(source, target);
        Assert.Equal("any #First #Last", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.Equal("#3", target.Parameters[3].Name); Assert.Equal("MI6", target.Parameters[3].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Values()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var target = source.Add(" {0} {1}", 50, "MI6");
        Assert.NotSame(source, target);
        Assert.Equal("any #First #Last #2 #3", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.Equal("#3", target.Parameters[3].Name); Assert.Equal("MI6", target.Parameters[3].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Parameters()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var xage = new Parameter("#Age", 50);
        var xorg = new Parameter("Org", "MI6");
        var target = source.Add(" {Age} {#Org}", xage, xorg);
        Assert.Equal("any #First #Last #Age #Org", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Age", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.Equal("#Org", target.Parameters[3].Name); Assert.Equal("MI6", target.Parameters[3].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);

        target = source.Add(" {0} {1}", xage, xorg);
        Assert.Equal("any #First #Last #Age #Org", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Age", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.Equal("#Org", target.Parameters[3].Name); Assert.Equal("MI6", target.Parameters[3].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_Anonymous()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var source = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);

        var xage = new { Age = 50 };
        var xorg = new { Org = "MI6" };
        var target = source.Add(" {Age} {#Org}", xage, xorg);
        Assert.Equal("any #First #Last #Age #Org", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Age", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.Equal("#Org", target.Parameters[3].Name); Assert.Equal("MI6", target.Parameters[3].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);

        target = source.Add(" {0} {1}", xage, xorg);
        Assert.Equal("any #First #Last #Age #Org", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Age", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.Equal("#Org", target.Parameters[3].Name); Assert.Equal("MI6", target.Parameters[3].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);

        var xother = new { Other = 1, Another = 2 };
        try { source.Add(" {0}", xother); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Text_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine);
        var target = source.ReplaceText(null);
        Assert.Same(source, target);

        target = source.ReplaceText("");
        Assert.Same(source, target);

        source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        target = source.ReplaceText(null);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Text()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var target = source.ReplaceText("other #0 #1");
        Assert.NotSame(source, target);
        Assert.Equal("other #0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);
        Assert.True(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Text_Inconsistent()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var target = source.ReplaceText("other #3 #1");
        Assert.NotSame(source, target);
        Assert.Equal("other #3 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
        
        target = source.ReplaceText("other {1}");
        Assert.NotSame(source, target);
        Assert.Equal("other {1}", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine);
        var target = source.ReplaceValues();
        Assert.Same(source, target);

        target = source.ReplaceValues([]);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var target = source.ReplaceValues([50]);
        Assert.NotSame(source, target);
        Assert.Equal("any #0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);

        target = source.ReplaceValues([50, 60, 70]);
        Assert.NotSame(source, target);
        Assert.Equal("any #0 #1", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal(60, target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(70, target.Parameters[2].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Parameters()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var xage = new Parameter("Age", 50);
        var target = source.ReplaceValues([xage]);
        Assert.NotSame(source, target);
        Assert.Equal("any #0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#Age", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Command()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "any {0} {1}", "James", "Bond");

        var source = new CommandInfo(command.GetCommandInfo());
        Assert.Equal("any #0 #1", source.Text);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#0", source.Parameters[0].Name); Assert.Equal("James", source.Parameters[0].Value);
        Assert.Equal("#1", source.Parameters[1].Name); Assert.Equal("Bond", source.Parameters[1].Value);
        Assert.True(source.IsConsistent);
        Assert.True(source.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_CommandInfo()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0}}", "James");
        source = (CommandInfo)source.ReplaceText("other #0 #1");
        Assert.False(source.IsValid);

        var target = new CommandInfo(source);
        Assert.Equal("other #0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_CommandInfo()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        source = (CommandInfo)source.ReplaceText("other #0");
        Assert.False(source.IsValid);

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, " another {0}", 50);

        var target = source.Add(command.GetCommandInfo());
        Assert.NotSame(source, target);
        Assert.Equal("other #0 another #2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
        Assert.False(target.IsConsistent);
        Assert.False(target.IsValid);
    }
}