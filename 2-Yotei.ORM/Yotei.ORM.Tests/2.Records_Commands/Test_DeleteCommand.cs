using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_DeleteCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var connection = new FakeConnection(new FakeEngine());
        IDeleteCommand command;
        ICommandInfo info;

        command = connection.Records.Delete(x => x.Employees).WithIsEmptyValid(true);
        info = command.GetCommandInfo();
        Assert.Equal("DELETE FROM [Employees]", info.Text);
        Assert.Empty(info.Parameters);

        command = connection.Records.Delete(x => "dbo..Employees").WithIsEmptyValid(true);
        info = command.GetCommandInfo();
        Assert.Equal("DELETE FROM [dbo]..[Employees]", info.Text);
        Assert.Empty(info.Parameters);

        // By default the value of this property is 'false'...
        command.IsEmptyValid = false;
        try { command.GetCommandInfo(); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_HeadAndTails()
    {
        var connection = new FakeConnection(new FakeEngine());
        IDeleteCommand command;
        ICommandInfo info;

        command = connection.Records.Delete(x => x.Employees).WithIsEmptyValid(true)
            .WithHead(x => "-pre-")
            .WithTail(x => "-post");
        info = command.GetCommandInfo();
        Assert.Equal("-pre-DELETE FROM [Employees]-post", info.Text);
        Assert.Empty(info.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Lambda()
    {
        var connection = new FakeConnection(new FakeEngine());
        IDeleteCommand command;
        ICommandInfo info;

        command = connection.Records.Delete(x => x.Employees).Where(x => x.Id == "007");
        info = command.GetCommandInfo();
        Assert.Equal(
            "DELETE FROM [Employees] WHERE ([Id] = #0)",
            info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Literal()
    {
        var connection = new FakeConnection(new FakeEngine());
        IDeleteCommand command;
        ICommandInfo info;

        // Note that within 'Where(...)' we need to use '=' but not '==' because, when using
        // literals, we need to follow SQL conventions...

        command = connection.Records.Delete(x => x.Employees).Where(x => "Id = 007");
        info = command.GetCommandInfo();
        Assert.Equal(
            "DELETE FROM [Employees] WHERE Id = 007",
            info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_As_Iterable()
    {
        var connection = new FakeConnection(new FakeEngine());
        IDeleteCommand command;
        ICommandInfo info;

        command = connection.Records.Delete(x => x.Employees).Where(x => x.Id == "007");
        info = command.GetCommandInfo(iterable: true);
        Assert.Equal("DELETE FROM [Employees] OUTPUT DELETED.* WHERE ([Id] = #0)", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var command = connection.Records.Delete(x => x.Employees)
            .Where(x => x.Id == "007")
            .WithIsEmptyValid(true);

        var target = command.Clone();
        Assert.True(target.IsEmptyValid);

        var info = target.GetCommandInfo();
        Assert.Equal("DELETE FROM [Employees] WHERE ([Id] = #0)", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var connection = new FakeConnection(new FakeEngine());
        IDeleteCommand command;
        ICommandInfo info;

        command = connection.Records.Delete(x => x.Employees).Where(x => x.Id == "007");
        command.IsEmptyValid = true;

        command.Clear();
        Assert.True(command.IsEmptyValid); // By convention, not modified by 'Clear()'...

        info = command.GetCommandInfo();
        Assert.Equal("DELETE FROM [Employees]", info.Text);
        Assert.Empty(info.Parameters);
    }
}