#pragma warning disable IDE0018 // Inline variable declaration

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_RawCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        text = command.GetText(out parameters);
        Assert.Empty(text);
        Assert.Empty(parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Text_NoArguments()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append("SELECT * FROM [Emp]");
        text = command.GetText(out parameters);
        Assert.Equal("SELECT * FROM [Emp]", text);
        Assert.Empty(parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_NoText_Positional_Arguments()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append(null, "Bond", 50);
        text = command.GetText(out parameters);
        Assert.Empty(text);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("#0", parameters[0].Name); Assert.Equal("Bond", parameters[0].Value);
        Assert.Equal("#1", parameters[1].Name); Assert.Equal(50, parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_NoText_NullArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append(null, null!);
        text = command.GetText(out parameters);
        Assert.Empty(text);
        Assert.Single(parameters);
        Assert.Equal("#0", parameters[0].Name); Assert.Null(parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Text_And_Positional_Arguments()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append("WHERE [Id] = {0}", "007");
        text = command.GetText(out parameters);
        Assert.Equal("WHERE [Id] = #0", text);
        Assert.Single(parameters);
        Assert.Equal("#0", parameters[0].Name); Assert.Equal("007", parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Text_And_Positional_Arguments()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append("WHERE [Id] = {0}", "007");
        command.Append(" AND [ManagerID] IS {0}", null!);
        text = command.GetText(out parameters);
        Assert.Equal("WHERE [Id] = #0 AND [ManagerID] IS #1", text);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("#0", parameters[0].Name); Assert.Equal("007", parameters[0].Value);
        Assert.Equal("#1", parameters[1].Name); Assert.Null(parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Text_And_Anonymous_Arguments()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append("WHERE [Id] = {id}", new { id = "007" });
        text = command.GetText(out parameters);
        Assert.Equal("WHERE [Id] = {id}", text);
        Assert.Single(parameters);
        Assert.Equal("id", parameters[0].Name); Assert.Equal("007", parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Text_And_Mixed_Arguments()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        string text;
        IParameterList parameters;

        command = connection.Records.Raw();
        command.Append("WHERE [Id] = {id} AND [ManagerId] = {1}",
            new { id = "007" },
            null);
        text = command.GetText(out parameters);
        Assert.Equal("WHERE [Id] = {id} AND [ManagerId] = #1", text);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("id", parameters[0].Name); Assert.Equal("007", parameters[0].Value);
        Assert.Equal("#1", parameters[1].Name); Assert.Null(parameters[1].Value);

        command.Append(" AND [Age] = {0}", 50);
        text = command.GetText(out parameters);
        Assert.Equal("WHERE [Id] = {id} AND [ManagerId] = #1 AND [Age] = #2", text);
        Assert.Equal(3, parameters.Count);
        Assert.Equal("id", parameters[0].Name); Assert.Equal("007", parameters[0].Value);
        Assert.Equal("#1", parameters[1].Name); Assert.Null(parameters[1].Value);
        Assert.Equal("#2", parameters[2].Name); Assert.Equal(50, parameters[2].Value);
    }
}