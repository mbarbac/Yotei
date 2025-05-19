using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Only_Text()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Only_Values()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Only_Parameters()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Only_Anonymous()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Values()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Any]={0} [Last]={1}", null, "Bond");
        Assert.Equal("[Any]=#0 [Last]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // Dangling specification...
        try { _ = new CommandInfo(engine, "{0} {1}", "one"); Assert.Fail(); }
        catch (ArgumentException) { }

        // Unused value...
        try { _ = new CommandInfo(engine, "{0}", "one", "two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Text_And_Parameters()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Text_And_Anonymous()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Clone()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Clear()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Text()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Values()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Parameters()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Anonymous()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Only_Text()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Only_Values()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Only_Parameters()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Only_Anonymous()
    //{
    //}

    // ----------------------------------------------------

    [Enforced]
    [Fact]
    public static void Test_Add_Info()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}", "James");

        var other = new CommandInfo(engine);
        var target = source.Add(other);
        Assert.Same(source, target);

        other = new CommandInfo(engine, " ANY");
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 ANY", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);

        other = new CommandInfo(engine, null, "Bond");
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        other = new CommandInfo(engine, " [Last]={0}", "Bond");
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Text_And_Values()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Text_And_Parameters()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Text_And_Anonymous()
    //{
    //}
}