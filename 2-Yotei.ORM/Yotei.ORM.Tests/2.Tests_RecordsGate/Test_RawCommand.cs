namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_RawCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var connection = new FakeConnection(new FakeEngine());
        IRawCommand command;
        ICommandInfo info;

        command = connection.Records.Raw();
        info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        var connection = new FakeConnection(new FakeEngine());
        IRawCommand command;
        ICommandInfo info;

        command = connection.Records.Raw(x => "SELECT *");
        info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal("SELECT *", info.Text);
        Assert.Empty(info.Parameters);

        try { _ = new RawCommand(connection, x => null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new RawCommand(connection, x => ""); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_Arguments()
    {
        var connection = new FakeConnection(new FakeEngine());
        IRawCommand command;
        ICommandInfo info;

        command = connection.Records.Raw(x => """
            SELECT *
            FROM [Emps]
            WHERE [Id] = {0}
            """,
            "007");

        //info = command.GetCommandInfo();
        //Assert.False(info.IsEmpty);
        //Assert.Equal("SELECT * FROM [Emps] WHERE [Id] = #0", info.Text);
        //Assert.Single(info.Parameters);
        //Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);

        try { _ = new RawCommand(connection, x => "[Id] = {0}"); Assert.Fail(); }
        catch (ArgumentException) { }

        //try { _ = new RawCommand(connection, x => "Any", "007"); Assert.Fail(); }
        //catch (ArgumentException) { }
    }
}