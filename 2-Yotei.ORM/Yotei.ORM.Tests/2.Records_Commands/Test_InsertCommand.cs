using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_InsertCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var connection = new FakeConnection(new FakeEngine());
        IInsertCommand command;
        ICommandInfo info;

        command = connection.Records.Insert(x => x.Employees);
        info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);

        command = connection.Records.Insert(x => "dbo..Employees");
        info = command.GetCommandInfo();
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_HeadAndTails()
    {
        var connection = new FakeConnection(new FakeEngine());
        IInsertCommand command;
        ICommandInfo info;

        command = connection.Records.Insert(x => x.Employees)
            .WithHead(x => "-pre-")
            .WithTail(x => "-post");
        info = command.GetCommandInfo();
        Assert.Equal("-pre-INSERT INTO [Employees]-post", info.Text);
        Assert.Empty(info.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Lambda()
    {
        var connection = new FakeConnection(new FakeEngine());
        IInsertCommand command;
        ICommandInfo info;

        command = connection.Records.Insert(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond");
        info = command.GetCommandInfo();
        Assert.Equal("INSERT INTO [Employees] ([First], [Last]) VALUES (#0, #1)", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Literal()
    {
        var connection = new FakeConnection(new FakeEngine());
        IInsertCommand command;
        ICommandInfo info;

        command = connection.Records.Insert(x => x.Employees)
            .Columns(x => "First = James")
            .Columns(x => "Last = Bond");
        info = command.GetCommandInfo();
        Assert.Equal("INSERT INTO [Employees] (First, Last) VALUES (James, Bond)", info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_As_Iterable()
    {
        var connection = new FakeConnection(new FakeEngine());
        IInsertCommand command;
        ICommandInfo info;

        command = connection.Records.Insert(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond");

        info = command.GetCommandInfo(iterable: true);
        Assert.Equal(
            "INSERT INTO [Employees] ([First], [Last]) " +
            "OUTPUT INSERTED.* " +
            "VALUES (#0, #1)",
            info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var command = connection.Records.Insert(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond");

        var target = command.Clone();
        var info = target.GetCommandInfo();
        Assert.Equal("INSERT INTO [Employees] ([First], [Last]) VALUES (#0, #1)",info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var connection = new FakeConnection(new FakeEngine());
        IInsertCommand command;
        ICommandInfo info;

        command = connection.Records.Insert(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond");

        command.Clear();
        info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
    }
}