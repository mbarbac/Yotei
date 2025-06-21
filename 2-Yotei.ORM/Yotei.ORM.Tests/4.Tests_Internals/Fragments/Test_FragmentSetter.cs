using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using System.Runtime.Intrinsics.Arm;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSetter
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => "any"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "any=other");
        Assert.Single(master);
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
    public static void Test_Literal_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "First=James");
        master.Capture(x => "Last=Bond");
        Assert.Equal(2, master.Count);
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => x.One + 7); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null);
        Assert.Single(master);
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
    public static void Test_Expression_Simple_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = "007");
        Assert.Single(master);
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
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-")(x.Id = null));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
        builder = master.VisitNames(); Assert.Equal("([Id])", builder.Text);
        builder = master.VisitValues(); Assert.Equal("(NULL)", builder.Text);

        try { master.Capture(x => x("-pre-")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => (x.Id = null)("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);
        builder = master.VisitNames(); Assert.Equal("([Id])", builder.Text);
        builder = master.VisitValues(); Assert.Equal("(NULL)", builder.Text);

        master = new(command);
        master.Capture(x => (x.Id = null!).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);
        builder = master.VisitNames(); Assert.Equal("([Id])", builder.Text);
        builder = master.VisitValues(); Assert.Equal("(NULL)", builder.Text);

        try { master.Capture(x => x("-post-")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHeadAndTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-")(x.Id = null).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);
        builder = master.VisitNames(); Assert.Equal("([Id])", builder.Text);
        builder = master.VisitValues(); Assert.Equal("(NULL)", builder.Text);

        try { master.Capture(x => x("-pre-").x("-post-")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    /*

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHeadTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-")(x.Id = null).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] = NULL)-post", builder.Text);
        Assert.Empty(builder.Parameters);
        builder = master.VisitNames(); Assert.Equal("([Id])", builder.Text);
        builder = master.VisitValues(); Assert.Equal("(NULL)", builder.Text);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentTerminal.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Other.x("-post-"));
        master.Capture(x => x.Another);
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("-pre-[Other]-post-[Another]", builder.Text);
        Assert.Empty(builder.Parameters);
    }
    
    
    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Other.x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Other]-post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Other.x("-post-"));
        master.Capture(x => x.Another);
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("-pre-[Other]-post-[Another]", builder.Text);
        Assert.Empty(builder.Parameters);
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
        master.Capture(x => x("-pre-").Other.x("-post-"));
        master.Capture(x => x.Another);
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal("-pre-[Other]-post-[Another]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Other.x("-post-"));
        master.Capture(x => x.Another);
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
    */
}