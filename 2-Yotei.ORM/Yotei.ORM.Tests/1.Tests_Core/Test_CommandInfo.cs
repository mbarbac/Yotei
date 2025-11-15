using IBuilder = Yotei.ORM.ICommandInfo.IBuilder;
using Builder = Yotei.ORM.Code.CommandInfo.Builder;

namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_CommandIndo
{
    //[Enforced]
    [Fact]
    public void Test_Create_Empty()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection);
        Assert.True(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("", info.ToString());

        builder = new Builder(connection, null);
        Assert.True(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("", info.ToString());

        builder = new Builder(connection, null, []);
        Assert.True(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("", info.ToString());

        builder = new Builder(connection, "");
        Assert.True(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("", info.ToString());

        builder = new Builder(connection, "", []);
        Assert.True(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("", info.ToString());
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Only_Text()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, "any");
        Assert.False(builder.IsEmpty);
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("any", info.ToString());

        try { _ = new Builder(connection, "{0}"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Builder(connection, "#0"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Builder(connection, "any {0}"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Builder(connection, "any #0"); Assert.Fail(); }
        catch (ArgumentException) { }

        builder = new Builder(connection, "any#other");
        Assert.False(builder.IsEmpty);
        Assert.Equal("any#other", builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("any#other", info.ToString());
    }
    
    //[Enforced]
    [Fact]
    public void Test_Create_Only_Values()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, null, null);
        Assert.False(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        builder = new Builder(connection, null, "any", new Parameter("First", "James"), new { Last = "Bond" });
        Assert.False(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);
        Assert.Equal("#First", builder.Parameters[1].Name); Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("#Last", builder.Parameters[2].Name); Assert.Equal("Bond", builder.Parameters[2].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Text_And_Values()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, "{0}", null);
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 -- [#0='NULL']", info.ToString());

        builder = new Builder(connection, "{0} {1}", "James", "Bond");
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0 #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='James', #1='Bond']", info.ToString());

        try { builder = new Builder(connection, "{0}", "James", "Bond"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Text_And_Parameters()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("First", "James");
        var plast = new Parameter("#Last", "Bond");

        builder = new Builder(connection, "{0} {1}", pfirst, plast);
        Assert.False(builder.IsEmpty);
        Assert.Equal("#First #Last", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#First #Last -- [#First='James', #Last='Bond']", info.ToString());

        // ISSUE: # shall not be identified because **is protected** in a {} bracket...

        builder = new Builder(connection, "{#First} {Last}", pfirst, plast);
        Assert.False(builder.IsEmpty);
        Assert.Equal("#First #Last", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#First #Last -- [#First='James', #Last='Bond']", info.ToString());

        try { builder = new Builder(connection, "{0}", pfirst, plast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}