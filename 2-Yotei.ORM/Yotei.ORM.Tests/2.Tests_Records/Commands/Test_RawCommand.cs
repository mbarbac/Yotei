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

        info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal("SELECT * FROM [Emps] WHERE [Id] = #0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("007", info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Text_Arguments()
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

        command.Append(x => """
             AND [Name] <> {0} AND Name >= {1}
            """,
            null, "James");

        info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal(
            "SELECT * FROM [Emps] WHERE [Id] = #0 AND [Name] <> NULL AND Name >= #1",
            info.Text);

        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var connection = new FakeConnection(new FakeEngine());

        var source = connection.Records.Raw(x => """
            SELECT *
            FROM [Emps]
            WHERE [Id] = {0}
            """,
            "007");

        var target = source.Clone();
        var info = target.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal("SELECT * FROM [Emps] WHERE [Id] = #0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("007", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var connection = new FakeConnection(new FakeEngine());

        var command = connection.Records.Raw(x => """
            SELECT *
            FROM [Emps]
            WHERE [Id] = {0}
            """,
            "007");

        command.Clear();

        var info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
    }
}