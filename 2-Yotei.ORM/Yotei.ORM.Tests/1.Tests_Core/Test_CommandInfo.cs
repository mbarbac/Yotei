using IBuilder = Yotei.ORM.ICommandInfo.IBuilder;
using Builder = Yotei.ORM.Code.CommandInfo.Builder;

namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
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
    public static void Test_Create_Only_Text()
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
    public static void Test_Create_Only_Values()
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
    public static void Test_Create_Text_And_Values()
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
        Assert.Equal("#0 -- [#0=NULL]", info.ToString());

        builder = new Builder(connection, "{0} {1}", "James", "Bond");
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0 #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='James', #1='Bond']", info.ToString());

        try { builder.Add(" {1}", null); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0}", "James", "Bond"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "any #0"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
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

        try { builder.Add(" {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0}", pfirst, plast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Anonymous()
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
    public static void Test_Create_Values_As_Colection()
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
        Assert.Equal("#0 #1 -- [#0=NULL, #1='any']", info.ToString());

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
        Assert.Equal("#0 #1 -- [#0=NULL, #1='[NULL, any]']", info.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Duplicate_Value_Names()
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
    public static void Test_Clone()
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
    [Fact]
    public static void Test_Add_Command()
    {
        ICommand source;
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, "{0}", "one");
        source = new FakeCommand(connection);
        Assert.False(builder.Add(source, false));

        source = new FakeCommand(connection, " {0}", "two");
        Assert.True(builder.Add(source, false));
        Assert.Equal("#0 #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("two", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='one', #1='two']", info.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_CommandInfo()
    {
        ICommandInfo source, info;
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, "{0}", "one");
        source = new CommandInfo(connection);
        Assert.False(builder.Add(source));

        source = new CommandInfo(connection, " {0}", "two");
        Assert.True(builder.Add(source));
        Assert.Equal("#0 #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("two", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='one', #1='two']", info.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_CommandInfoBuilder()
    {
        IBuilder source;
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, "{0}", "one");
        source = new Builder(connection);
        Assert.False(builder.Add(source));

        source = new Builder(connection, " {0}", "two");
        Assert.True(builder.Add(source));
        Assert.Equal("#0 #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("two", builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='one', #1='two']", info.ToString());

        builder = new Builder(connection, "{0}", "one");
        source = new Builder(connection);
        source.ReplaceText(" {0}");
        Assert.True(builder.Add(source));
        Assert.Equal("#0 {0}", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Text()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection);
        Assert.False(builder.Add((string?)null)); Assert.True(builder.IsEmpty);
        Assert.False(builder.Add("")); Assert.True(builder.IsEmpty);

        Assert.True(builder.Add("any"));
        Assert.False(builder.IsEmpty);
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("any", info.ToString());

        try { builder.Add("{other}"); Assert.Fail(); } catch (ArgumentException) { }
        try { builder.Add("#other"); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Values()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection);
        Assert.False(builder.Add(null, []));
        Assert.True(builder.IsEmpty);

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.Add(null, null));
        Assert.False(builder.IsEmpty);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Null(builder.Parameters[1].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Parameters()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("First", "James");
        var plast = new Parameter("#Last", "Bond");

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.Add(null, pfirst, plast));
        Assert.False(builder.IsEmpty);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#First", builder.Parameters[1].Name); Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("#Last", builder.Parameters[2].Name); Assert.Equal("Bond", builder.Parameters[2].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Anonymous()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.Add(null, xfirst, xlast));
        Assert.False(builder.IsEmpty);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#First", builder.Parameters[1].Name); Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("#Last", builder.Parameters[2].Name); Assert.Equal("Bond", builder.Parameters[2].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Values()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.Add(" {0}", null));
        Assert.False(builder.IsEmpty);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Null(builder.Parameters[1].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #1 -- [#0='one', #1=NULL]", info.ToString());

        try { builder.Add(" {1}", null); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0}", "James", "Bond"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "any #0"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Parameters()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var pfirst = new Parameter("First", "James");
        var plast = new Parameter("#Last", "Bond");

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.Add(" {first} {1}", pfirst, plast));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0 #First #Last", builder.Text);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#First", builder.Parameters[1].Name); Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("#Last", builder.Parameters[2].Name); Assert.Equal("Bond", builder.Parameters[2].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #First #Last -- [#0='one', #First='James', #Last='Bond']", info.ToString());

        try { builder.Add(" {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0}", pfirst, plast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", pfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Anonymous()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.Add(" {0} {last}", xfirst, xlast));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0 #First #Last", builder.Text);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.Equal("#First", builder.Parameters[1].Name); Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("#Last", builder.Parameters[2].Name); Assert.Equal("Bond", builder.Parameters[2].Value);
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Equal("#0 #First #Last -- [#0='one', #First='James', #Last='Bond']", info.ToString());

        try { builder.Add(" {1}", xfirst); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0}", xfirst, xlast); Assert.Fail(); }
        catch (ArgumentException) { }

        try { builder = new Builder(connection, "{0} {1}", xfirst); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Text()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection);
        Assert.False(builder.ReplaceText(null));
        Assert.True(builder.IsEmpty);

        builder = new Builder(connection, "{0}", "one");
        Assert.False(builder.ReplaceText("#0"));

        Assert.True(builder.ReplaceText("{1}"));
        Assert.False(builder.IsEmpty);
        Assert.Equal("{1}", builder.Text);
        Assert.Equal(1, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        Assert.True(builder.ReplaceText("#2"));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#2", builder.Text);
        Assert.Equal(1, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        Assert.True(builder.ReplaceText(null));
        Assert.False(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Equal(1, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("one", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection);
        Assert.False(builder.ReplaceValues([]));
        Assert.True(builder.IsEmpty);

        Assert.True(builder.ReplaceValues(null));
        Assert.False(builder.IsEmpty);
        Assert.Empty(builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.ReplaceValues([]));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0", builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        builder = new Builder(connection, "{0}", "one");
        Assert.True(builder.ReplaceValues("two"));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#1", builder.Parameters[0].Name); Assert.Equal("two", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values_Parameters()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var pone = new Parameter("One", "v1");
        var ptwo = new Parameter("Two", "v2");
        builder = new Builder(connection, "{0}", pone);
        Assert.True(builder.ReplaceValues(ptwo));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#One", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#Two", builder.Parameters[0].Name); Assert.Equal("v2", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        ptwo = new Parameter("ONE", "v2");
        builder = new Builder(connection, "{0}", pone);
        Assert.True(builder.ReplaceValues(ptwo));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#One", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#1", builder.Parameters[0].Name); Assert.Equal("v2", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values_Anonymous()
    {
        IBuilder builder;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var xone = new { One = "v1" };
        var xtwo = new { Two = "v2" };
        builder = new Builder(connection, "{0}", xone);
        Assert.True(builder.ReplaceValues(xtwo));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#One", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#Two", builder.Parameters[0].Name); Assert.Equal("v2", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }

        var xthree = new { ONE = "v2" };
        builder = new Builder(connection, "{0}", xone);
        Assert.True(builder.ReplaceValues(xthree));
        Assert.False(builder.IsEmpty);
        Assert.Equal("#One", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#1", builder.Parameters[0].Name); Assert.Equal("v2", builder.Parameters[0].Value);
        Assert.False(builder.IsConsistent);
        try { builder.CreateInstance(); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        IBuilder builder;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        builder = new Builder(connection);
        Assert.False(builder.Clear());

        builder = new Builder(connection, "{0} {1}", "James", "Bond");
        Assert.True(builder.Clear());
        Assert.True(builder.IsConsistent);
        info = builder.CreateInstance();
        Assert.Empty(info.ToString()!);
    }
}