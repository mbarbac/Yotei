using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentJoin
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master = new(command);

        try { master.Capture(x => "Customers AS"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => "Customers ON"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Customers AS Cust ON Orders.CustomerId = Cust.Id");
        Assert.Single(master);
        Assert.Equal("Orders.CustomerId = Cust.Id", ((FragmentJoin.Entry)master[0]).Condition!.ToString());
        Assert.Equal("Cust", ((FragmentJoin.Entry)master[0]).Alias);
        builder = master.Visit();
        Assert.Equal("JOIN Customers AS Cust ON (Orders.CustomerId = Cust.Id)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Left Join Customers AS Cust ON Orders.CustomerId = Cust.Id");
        Assert.Single(master);
        Assert.Equal("Orders.CustomerId = Cust.Id", ((FragmentJoin.Entry)master[0]).Condition!.ToString());
        Assert.Equal("Cust", ((FragmentJoin.Entry)master[0]).Alias);
        builder = master.Visit();
        Assert.Equal("LEFT JOIN Customers AS Cust ON (Orders.CustomerId = Cust.Id)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Customers AS Cust ON Orders.CustomerId = Cust.Id");
        master.Capture(x => "Outer Join Employees AS Emp ON Orders.EmployeeId = Emp.Id");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal(
            "JOIN Customers AS Cust ON (Orders.CustomerId = Cust.Id) " +
            "OUTER JOIN Employees AS Emp ON (Orders.EmployeeId = Emp.Id)",
            builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master = new(command);

        try { master.Capture(x => x.Id.As()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.As(x.Emp)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.Id.On()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.On(x.Emp)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("JOIN [Customers] AS [Cust] ON ([Orders].[CustomerId] = [Cust].[Id])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-JOIN [Customers] AS [Cust] ON ([Orders].[CustomerId] = [Cust].[Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        try { master.Capture(x => x("-pre-")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("JOIN [Customers] AS [Cust] ON ([Orders].[CustomerId] = [Cust].[Id])-post-", builder.Text);
        Assert.Empty(builder.Parameters);

        try { master.Capture(x => x("-pre-")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithheadAndTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-JOIN [Customers] AS [Cust] ON ([Orders].[CustomerId] = [Cust].[Id])-post-", builder.Text);
        Assert.Empty(builder.Parameters);

        // Here "-pre-" is extracted as an invoke head and because "-post-" is the sole argument
        // of the remaining, then it is captured (as a literal, but it could be anything).
        master = new(command);
        master.Capture(x => x("-pre-").x("-post-"));
        Assert.Single(master);
        Assert.IsType<DbTokenInvoke>(master[0].Head);
        Assert.IsType<DbTokenLiteral>(master[0].Body);
        Assert.Null(master[0].Tail);
        builder = master.Visit();
        Assert.Equal("-pre-JOIN -post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        master.Capture(x => x.JoinType("LEFT JOIN").Employees.As(x.Emp).On(x.Orders.EmployeeId == x.Emp.Id));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal(
            "JOIN [Customers] AS [Cust] ON ([Orders].[CustomerId] = [Cust].[Id]) " +
            "LEFT JOIN [Employees] AS [Emp] ON ([Orders].[EmployeeId] = [Emp].[Id])",
            builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        master.Capture(x => x.JoinType("LEFT JOIN").Employees.As(x.Emp).On(x.Orders.EmployeeId == x.Emp.Id));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal(
            "JOIN [Customers] AS [Cust] ON ([Orders].[CustomerId] = [Cust].[Id]) " +
            "LEFT JOIN [Employees] AS [Emp] ON ([Orders].[EmployeeId] = [Emp].[Id])",
            builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        master.Capture(x => x.JoinType("LEFT JOIN").Employees.As(x.Emp).On(x.Orders.EmployeeId == x.Emp.Id));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}