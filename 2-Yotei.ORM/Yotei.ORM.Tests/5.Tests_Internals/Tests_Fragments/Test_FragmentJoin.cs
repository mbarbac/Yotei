namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_FragmentJoin
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_String_Reduced()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Customers On Items.CustomerId = Customers.Id");
        Assert.Single(master);
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.Null(entry.Alias);
        Assert.IsType<DbTokenLiteral>(entry.Condition);
        Assert.Equal("Items.CustomerId = Customers.Id", entry.Condition.ToString());
        Assert.Equal("Customers", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("JOIN Customers On Items.CustomerId = Customers.Id", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_String_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Left Join Customers As Cust On Items.CustomerId = Cust.Id");
        Assert.Single(master);
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("Left Join", entry.JoinType);
        Assert.Equal("Cust", entry.Alias);
        Assert.IsType<DbTokenLiteral>(entry.Condition);
        Assert.Equal("Items.CustomerId = Cust.Id", entry.Condition.ToString());
        Assert.Equal("Customers", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Left Join Customers As Cust On Items.CustomerId = Cust.Id", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invoke()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("Left Join Customers As Cust On Items.CustomerId = Cust.Id"));
        Assert.Single(master);
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("Left Join", entry.JoinType);
        Assert.Equal("Cust", entry.Alias);
        Assert.IsType<DbTokenLiteral>(entry.Condition);
        Assert.Equal("Items.CustomerId = Cust.Id", entry.Condition.ToString());
        Assert.Equal("Customers", entry.Body.ToString());

        builder = master.Visit();
        Assert.Equal("Left Join Customers As Cust On Items.CustomerId = Cust.Id", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;
        DbTokenCommandInfo info;

        master = new(command);
        master.Capture(x => "Customers {0} AS Cust ON Items.CustId = Cust.Id AND Name = {1}", "X", null);
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.Equal("Cust", entry.Alias);
        info = Assert.IsType<DbTokenCommandInfo>(entry.Condition);
        Assert.Equal("Items.CustId = Cust.Id AND Name = #1", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Null(info.CommandInfo.Parameters[0].Value);
        info = Assert.IsType<DbTokenCommandInfo>(entry.Body);
        Assert.Equal("Customers #0", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("X", info.CommandInfo.Parameters[0].Value);

        builder = master.Visit();
        Assert.Equal("JOIN Customers #0 AS Cust ON Items.CustId = Cust.Id AND Name = NULL", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("X", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Info_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;
        DbTokenCommandInfo info;

        master = new(command);
        master.Capture(x => "Customers {0} AS Cust ON Items.CustId = Cust.Id AND Name = {1}", "X", "007");
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.Equal("Cust", entry.Alias);
        info = Assert.IsType<DbTokenCommandInfo>(entry.Condition);
        Assert.Equal("Items.CustId = Cust.Id AND Name = #1", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("007", info.CommandInfo.Parameters[0].Value);
        info = Assert.IsType<DbTokenCommandInfo>(entry.Body);
        Assert.Equal("Customers #0", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("X", info.CommandInfo.Parameters[0].Value);

        builder = master.Visit();
        Assert.Equal("JOIN Customers #0 AS Cust ON Items.CustId = Cust.Id AND Name = #1", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("X", builder.Parameters[0].Value);
        Assert.Equal("007", builder.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Invalids()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master = new(command);

        try { master.Capture(x => ""); Assert.Fail(); } // Empty literal...
        catch (ArgumentException) { }

        try { master.Capture(x => "any=value {0} {1}", "any"); Assert.Fail(); } // Parameters' mismatch...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Customers as Cust ON Items.CustId = Cust.Id");
        master.Capture(x => "Left Join Employees as Emp ON Items.CustId = Emp.Id");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Customers", entry.Body.ToString());
        Assert.Equal("Cust", entry.Alias);
        Assert.IsType<DbTokenLiteral>(entry.Condition); Assert.Equal("Items.CustId = Cust.Id", entry.Condition.ToString());
        entry = Assert.IsType<FragmentJoin.Entry>(master[1]);
        Assert.Equal("Left Join", entry.JoinType);
        Assert.IsType<DbTokenLiteral>(entry.Body); Assert.Equal("Employees", entry.Body.ToString());
        Assert.Equal("Emp", entry.Alias);
        Assert.IsType<DbTokenLiteral>(entry.Condition); Assert.Equal("Items.CustId = Emp.Id", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal(
            "JOIN Customers as Cust ON Items.CustId = Cust.Id " +
            "Left Join Employees as Emp ON Items.CustId = Emp.Id"
            , builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many_Info()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Customers as Cust ON Items.CustId = {0}", "007");
        master.Capture(x => "Left Join Employees as Emp ON Items.CustId = {0}", null);
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("Customers", entry.Body.ToString());
        Assert.Equal("Cust", entry.Alias);
        Assert.IsType<DbTokenCommandInfo>(entry.Condition); Assert.Equal("Items.CustId = #0", entry.Condition.ToString());
        entry = Assert.IsType<FragmentJoin.Entry>(master[1]);
        Assert.Equal("Left Join", entry.JoinType);
        Assert.IsType<DbTokenCommandInfo>(entry.Body); Assert.Equal("Employees", entry.Body.ToString());
        Assert.Equal("Emp", entry.Alias);
        Assert.IsType<DbTokenCommandInfo>(entry.Condition); Assert.Equal("Items.CustId = #0", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal(
            "JOIN Customers as Cust ON Items.CustId = #0 " +
            "Left Join Employees as Emp ON Items.CustId = NULL"
            , builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Items.CustId == x.Cust.Id));
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Customers]", entry.Body.ToString());
        Assert.Equal("[Cust]", entry.Alias);
        Assert.IsType<DbTokenBinary>(entry.Condition); Assert.Equal("(x.[Items].[CustId] Equal x.[Cust].[Id])", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal("JOIN [Customers] As [Cust] On ([Items].[CustId] = [Cust].[Id])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_JoinMethod()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.LeftJoin().Customers.As(x.Cust).On(x.Items.CustId == x.Cust.Id));
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("Left Join", entry.JoinType);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Customers]", entry.Body.ToString());
        Assert.Equal("[Cust]", entry.Alias);
        Assert.IsType<DbTokenBinary>(entry.Condition); Assert.Equal("(x.[Items].[CustId] Equal x.[Cust].[Id])", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal("Left Join [Customers] As [Cust] On ([Items].[CustId] = [Cust].[Id])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Null_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Items.CustId == null));
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Customers]", entry.Body.ToString());
        Assert.Equal("[Cust]", entry.Alias);
        Assert.IsType<DbTokenBinary>(entry.Condition); Assert.Equal("(x.[Items].[CustId] Equal NULL)", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal("JOIN [Customers] As [Cust] On ([Items].[CustId] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Other_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Items.CustId == "007"));
        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Customers]", entry.Body.ToString());
        Assert.Equal("[Cust]", entry.Alias);
        Assert.IsType<DbTokenBinary>(entry.Condition); Assert.Equal("(x.[Items].[CustId] Equal '007')", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal("JOIN [Customers] As [Cust] On ([Items].[CustId] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Invalid()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master = new(command);

        try { master.Capture(x => x); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => x(x)); Assert.Fail(); } // Resolves to argument...
        catch (ArgumentException) { }

        try { master.Capture(x => 7); Assert.Fail(); } // Resolves to raw value...
        catch (ArgumentException) { }

        try { master.Capture(x => x.As()); Assert.Fail(); } // Empty alias...
        catch (ArgumentException) { }

        try { master.Capture(x => x.As(x.Any)); Assert.Fail(); } // No main part...
        catch (ArgumentException) { }

        try { master.Capture(x => x.On()); Assert.Fail(); } // Empty condition...
        catch (ArgumentException) { }

        try { master.Capture(x => x.On(x.Any)); Assert.Fail(); } // No main part...
        catch (ArgumentException) { }

        try { master.Capture(x => x.LeftJoin()); Assert.Fail(); } // No main part...
        catch (ArgumentException) { }

        try { master.Capture(x => x.LeftJoin(x.Other).Cust); Assert.Fail(); } // No parameterless Join spec...
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamic_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        FragmentJoin.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Items.CustId == "007"));
        master.Capture(x => x.Employees.As(x.Emp).On(x.Items.CustId == null));
        Assert.Equal(2, master.Count);

        entry = Assert.IsType<FragmentJoin.Entry>(master[0]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Customers]", entry.Body.ToString());
        Assert.Equal("[Cust]", entry.Alias);
        Assert.IsType<DbTokenBinary>(entry.Condition); Assert.Equal("(x.[Items].[CustId] Equal '007')", entry.Condition.ToString());

        entry = Assert.IsType<FragmentJoin.Entry>(master[1]);
        Assert.Equal("JOIN", entry.JoinType);
        Assert.IsType<DbTokenIdentifier>(entry.Body); Assert.Equal("x.[Employees]", entry.Body.ToString());
        Assert.Equal("[Emp]", entry.Alias);
        Assert.IsType<DbTokenBinary>(entry.Condition); Assert.Equal("(x.[Items].[CustId] Equal NULL)", entry.Condition.ToString());

        builder = master.Visit();
        Assert.Equal(
            "JOIN [Customers] As [Cust] On ([Items].[CustId] = #0) " +
            "JOIN [Employees] As [Emp] On ([Items].[CustId] IS NULL)"
            , builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
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
        master.Capture(x => x.Customers.As(x.Cust).On(x.Items.CustId == "007"));
        master.Capture(x => x.Employees.As(x.Emp).On(x.Items.CustId == null));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        builder = target.Visit();
        Assert.Equal(
            "JOIN [Customers] As [Cust] ON ([Items].[CustId] = #0) " +
            "JOIN [Employees] As [Emp] ON ([Items].[CustId] IS NULL)"
            , builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentJoin.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Customers.As(x.Cust).On(x.Items.CustId == "007"));
        master.Capture(x => x.Employees.As(x.Emp).On(x.Items.CustId == null));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}