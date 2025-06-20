using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentJoin
{
    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_NotSupported()
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
    public static void Test_ToLiteral_Single()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Customers AS Cust ON Orders.CustomerId = Cust.Id");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("JOIN Customers AS Cust ON (Orders.CustomerId = Cust.Id)", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Left Join Customers AS Cust ON Orders.CustomerId = Cust.Id");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("LEFT JOIN Customers AS Cust ON (Orders.CustomerId = Cust.Id)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Many()
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
    public static void Test_Expression_NotSupported()
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

    //[Enforced]
    [Fact]
    public static void Test_Expression_Single()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("JOIN [Customers] AS [Cust] ON (([Orders].[CustomerId] = [Cust].[Id]))", builder.Text);
        Assert.Empty(builder.Parameters);
    }

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
            "JOIN [Customers] AS [Cust] ON (([Orders].[CustomerId] = [Cust].[Id])) " +
            "LEFT JOIN [Employees] AS [Emp] ON (([Orders].[EmployeeId] = [Emp].[Id]))",
            builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentJoin.Master source = new(command);
        source.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        source.Capture(x => x.Employees.As(x.Emp).On(x.Orders.EmployeeId == x.Emp.Id));
        Assert.Equal(2, source.Count);

        var target = source.Clone();
        Assert.Equal(2, target.Count);
        Assert.Same(target, target[0].Master);
        Assert.Same(target, target[1].Master);

        var builder = target.Visit();
        Assert.Equal(
            "JOIN [Customers] AS [Cust] ON (([Orders].[CustomerId] = [Cust].[Id])) " +
            "JOIN [Employees] AS [Emp] ON (([Orders].[EmployeeId] = [Emp].[Id]))",
            builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentJoin.Master master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Orders.CustomerId == x.Cust.Id));
        master.Capture(x => x.Employees.As(x.Emp).On(x.Orders.EmployeeId == x.Emp.Id));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}