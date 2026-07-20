using Builder = Yotei.ORM.Records.Code.CommandInfo.Builder;
using IBuilder = Yotei.ORM.Records.ICommandInfo.IBuilder;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CommandInfoBuilder
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine);
        Assert.Equal("", builder.ToString());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "", []);
        Assert.Equal("", builder.ToString());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        try { _ = new Builder(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine, "any");
        Assert.Equal("any", builder.ToString());
        Assert.Empty(builder.Parameters);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Arbitrary()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Parameters()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        var xfirst = new Parameter("#first", "James");
        var xlast = new Parameter("last", "Bond");
        builder = new Builder(engine, "any {first} {#last}", xfirst, xlast);
        Assert.Equal("any #first #last -- [#first='James', #last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#first", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "any {0} {1}", xfirst, xlast);
        Assert.Equal("any #first #last -- [#first='James', #last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#first", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Anonymous()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };
        builder = new Builder(engine, "any {first} {#last}", xfirst, xlast);
        Assert.Equal("any #First #Last -- [#First='James', #Last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "any {0} {1}", xfirst, xlast);
        Assert.Equal("any #First #Last -- [#First='James', #Last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Inconsistent()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine, "any #name");
        Assert.Equal("any #name", builder.ToString());
        Assert.Empty(builder.Parameters);
        Assert.False(builder.IsEmpty);
        Assert.False(builder.IsConsistent);
        try { _ = builder.ToInstance(); Assert.Fail(); } catch (InvalidOperationException) { }

        try { _ = new Builder(engine, "any {0}"); Assert.Fail(); }
        catch (ArgumentException) { }

        builder = new Builder(engine, "any", "James", "Bond");
        Assert.Equal("any -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.False(builder.IsConsistent);
        try { _ = builder.ToInstance(); Assert.Fail(); } catch (InvalidOperationException) { }
    }
}