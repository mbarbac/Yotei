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

        try { builder = new Builder(connection, "any #0"); Assert.Fail(); }
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

    //[Enforced]
    [Fact]
    public void Test_Create_Text_And_Anonymous()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine() { ParameterPrefix = "@" };
        var connection = new FakeConnection(engine);

        var xfirst = new { First = "James" };
        var xlast = new { @Last = "Bond" };

        builder = new Builder(connection, "{0} {1}", xfirst, xlast);
        Assert.False(builder.IsEmpty);
        Assert.Equal("@First @Last", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("@First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("@Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("@First @Last -- [@First='James', @Last='Bond']", info.ToString());

        builder = new Builder(connection, "{@First} {Last}", xfirst, xlast);
        Assert.False(builder.IsEmpty);
        Assert.Equal("@First @Last", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("@First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("@Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("@First @Last -- [@First='James', @Last='Bond']", info.ToString());

        try { builder = new Builder(connection, "{0}", xfirst, xlast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", xfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Values_As_Colection()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        // Because of the signature, arrays are the receiving type of variable values, so when
        // the first an unique one is itself an array it is taken as a collection of arguments...
        builder = new Builder(connection, "{0} {1}", [null, "any"]);
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0 #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("any", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='NULL', #1='any']", info.ToString());

        // A idiomatic workaround is to use a parameter as the argument...
        object?[] array = { null, "any" };
        builder = new Builder(connection, "{0}", new Parameter("#0", array));
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name);
        array = Assert.IsType<object?[]>(builder.Parameters[0].Value);
        Assert.Equal(2, array.Length);
        Assert.Null(array[0]);
        Assert.Equal("any", array[1]);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 -- [#0='[NULL, any]']", info.ToString());

        // When the collection is the 2nd argument, we have not such problem...
        builder = new Builder(connection, "{0} {1}", null, new object?[] { null, "any" });
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name);
        array = Assert.IsType<object?[]>(builder.Parameters[1].Value);
        Assert.Equal(2, array.Length);
        Assert.Null(array[0]);
        Assert.Equal("any", array[1]);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='NULL', #1='[NULL, any]']", info.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Duplicate_Value_Names()
    {
        IBuilder builder;
        var engine = new FakeEngine() { ParameterPrefix = "@" };
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("One", "any");
        var plast = new Parameter("@One", "other");
        try { builder = new Builder(connection, "{0} {1}", pfirst, plast); Assert.Fail(); }
        catch (DuplicateException) { }

        var xfirst = new { One = "any" };
        var xlast = new { @One = "other" };
        try { builder = new Builder(connection, "{0} {1}", xfirst, xlast); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clone()
    {
        IBuilder source, target;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        source = new Builder(connection);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Empty(target.Parameters);
        Assert.True(target.IsConsistent);

        source = new Builder(connection, "{0} {1}", "James", "Bond");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.True(target.IsConsistent);
        info = target.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='James', #1='Bond']", info.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public void Test_Add_Command()

    //[Enforced]
    //[Fact]
    //public void Test_Add_CommandInfo()

    //[Enforced]
    //[Fact]
    //public void Test_Add_CommandInfoBuilder()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public void Test_Add_Only_Text()

    //[Enforced]
    //[Fact]
    //public void Test_Add_Only_Values()

    //[Enforced]
    //[Fact]
    //public void Test_Add_Only_Parameters()

    //[Enforced]
    //[Fact]
    //public void Test_Add_Only_Anonymous()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public void Test_Replace_Text()

    //[Enforced]
    //[Fact]
    //public void Test_Replace_Values()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public void Test_Clear()
}