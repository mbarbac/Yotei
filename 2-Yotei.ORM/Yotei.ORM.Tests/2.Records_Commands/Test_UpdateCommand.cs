using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_UpdateCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var connection = new FakeConnection(new FakeEngine());
        IUpdateCommand command;
        ICommandInfo info;

        command = connection.Records.Update(x => x.Employees);        
        info = command.GetCommandInfo();
        Assert.Equal("UPDATE [Employees]", info.Text);
        Assert.Empty(info.Parameters);

        command = connection.Records.Update(x => "dbo..Employees");
        info = command.GetCommandInfo();
        Assert.Equal("UPDATE [dbo]..[Employees]", info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_HeadAndTails()
    {
        var connection = new FakeConnection(new FakeEngine());
        IUpdateCommand command;
        ICommandInfo info;

        command = connection.Records.Update(x => x.Employees)
            .WithHeads(x => "-pre-")
            .WithTails(x => "-post");        
        info = command.GetCommandInfo();
        Assert.Equal("-pre-UPDATE [Employees]-post", info.Text);
        Assert.Empty(info.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Lambda()
    {
        var connection = new FakeConnection(new FakeEngine());
        IUpdateCommand command;
        ICommandInfo info;

        command = connection.Records.Update(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond")
            .Where(x => x.Id == "007");
        info = command.GetCommandInfo();
        Assert.Equal(
            "UPDATE [Employees] SET (([First] = #0), ([Last] = #1)) WHERE ([Id] = #2)",
            info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal("007", info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Literal()
    {
        var connection = new FakeConnection(new FakeEngine());
        IUpdateCommand command;
        ICommandInfo info;

        // Note that within 'Where(...)' we need to use '=' but not '==' because, when using
        // literals, we need to follow SQL conventions...

        command = connection.Records.Update(x => x.Employees)
            .Columns(x => "First = James")
            .Columns(x => "Last = Bond")
            .Where(x => "Id = 007");
        info = command.GetCommandInfo();
        Assert.Equal(
            "UPDATE [Employees] SET ((First = James), (Last = Bond)) WHERE Id = 007",
            info.Text);
        Assert.Empty(info.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_As_Iterable()
    {
        var connection = new FakeConnection(new FakeEngine());
        IUpdateCommand command;
        ICommandInfo info;

        command = connection.Records.Update(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond")
            .Where(x => x.Id == "007");

        info = command.GetCommandInfo(iterable: true);
        Assert.Equal(
            "UPDATE [Employees] SET (([First] = #0), ([Last] = #1)) " +
            "OUTPUT INSERTED.* " +
            "WHERE ([Id] = #2)",
            info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal("007", info.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var connection = new FakeConnection(new FakeEngine());
        IUpdateCommand command;
        ICommandInfo info;

        command = connection.Records.Update(x => x.Employees)
            .Columns(x => x.First = "James")
            .Columns(x => x.Last = "Bond")
            .Where(x => x.Id == "007");

        command.Clear();
        info = command.GetCommandInfo();
        Assert.Equal("UPDATE [Employees]", info.Text);
        Assert.Empty(info.Parameters);
    }
}