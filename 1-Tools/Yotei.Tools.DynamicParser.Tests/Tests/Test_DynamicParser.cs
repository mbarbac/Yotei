using static System.Console;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_DynamicParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Constant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => null!;
        node = node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("'NULL'", node.ToString());

        func = x => 7;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("'7'", node.ToString());

        func = x => "any";
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("'any'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Constant_With_Cast()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => (string)null!;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("'NULL'", node.ToString());

        func = x => (int)10.5;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("'10'", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetMember()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetMember_Chained()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetArgument_ToConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x = 7;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("'7'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha", node.ToString());

        func = x => x = x.Alpha.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetMember_ToConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha = 7;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetMember_Chained_ToConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha.Beta = 7;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha.Beta = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetMember_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetMember_Chained_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha.Alpha = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha.Alpha = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetMember_Complex()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha.Beta = x.Beta.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha.Beta = x.Beta.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x['7']", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxConstant_IxConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7, 9];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x['7', '9']", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxConstant_Chained_IxConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7][9];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x['7']['9']", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[x]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxArgument_Chained_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x][x];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[x][x]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxArgument_Nested_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x[x]];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[x[x]]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[x.Alpha]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxDynamic_Chained_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha][x.Beta];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[x.Alpha][x.Beta]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxDynamic_Nested_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha[x.Alpha]];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[x.Alpha[x.Alpha]]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha['7']", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxConstant_IxConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7, 9];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha['7', '9']", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxConstant_Chained_IxConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7][9];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha['7']['9']", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha[x]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxArgument_Chained_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x][x];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha[x][x]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxArgument_Nested_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x[x]];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha[x[x]]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha[x.Alpha]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxDynamic_Chained_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha][x.Beta];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha[x.Alpha][x.Beta]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedMember_IxDynamic_Nested_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha[x.Alpha]];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha[x.Alpha[x.Alpha]]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxConstant_ToConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7] = 9;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x['7'] = '9')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxConstant_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x['7'] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxConstant_IxConstant_ToConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7, 9] = null!;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x['7', '9'] = 'NULL')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxConstant_IxConstant_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7, 9] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x['7', '9'] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxConstant_Chained_IxConstant_ToConstant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7][9] = null!;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x['7']['9'] = 'NULL')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_GetIndexedArgument_IxConstant_Chained_IxConstant_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[7][9] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x['7']['9'] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxArgument_Chained_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x][x] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x][x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxArgument_Chained_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x][x] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x][x] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxArgument_Nested_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x[x]] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x[x]] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxArgument_Nested_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x[x]] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x[x]] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x.Alpha] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x.Alpha] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxDynamic_Chained_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha][x.Beta] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x.Alpha][x.Beta] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxDynamic_Nested_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha[x.Alpha]] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x.Alpha[x.Alpha]] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedArgument_IxDynamic_Nested_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x.Alpha[x.Alpha]] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x[x.Alpha[x.Alpha]] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxConstant_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha['7'] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxConstant_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha['7'] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxConstant_IxConstant_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7, 9] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha['7', '9'] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxConstant_IxConstant_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7, 9] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha['7', '9'] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxConstant_Chained_IxConstant_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7][9] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha['7']['9'] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxConstant_Chained_IxConstant_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[7][9] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha['7']['9'] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxArgument_Chained_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x][x] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x][x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxArgument_Chained_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x][x] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x][x] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxArgument_Nested_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x[x]] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x[x]] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxArgument_Nested_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x[x]] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x[x]] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x.Alpha] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x.Alpha] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxDynamic_Chained_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha][x.Beta] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x.Alpha][x.Beta] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxDynamic_Chained_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha][x.Beta] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x.Alpha][x.Beta] = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxDynamic_Nested_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha[x.Alpha]] = x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x.Alpha[x.Alpha]] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SetIndexedMember_IxDynamic_Nested_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha[x.Alpha[x.Alpha]] = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha[x.Alpha[x.Alpha]] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x();
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Chained()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x()();
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x()()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x(x)", node.ToString());

        func = x => x(x(x(x(x))));
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x(x(x(x(x))))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x(x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Chained_IxArgument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x)(x);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x(x)(x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Chained_IxDynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x.Alpha)(x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x(x.Alpha)(x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxArgumentSetter()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x = x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x(x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxMemberSetter()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x.Alpha = x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x((x.Alpha = x.Alpha))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxMemberSetter_02()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x.Alpha = x.Beta)(x.Beta = x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x((x.Alpha = x.Beta))((x.Beta = x.Alpha))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Combined_Invoke_Indexed_01()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x(x[x] = x);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x((x[x] = x))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Combined_Invoke_Indexed_02()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x().x = x];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[(x().x = x)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Combined_Invoke_Indexed_03()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x[x().Alpha = x.Alpha];
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x[(x().Alpha = x.Alpha)]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_NoArguments()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha();
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_Chained_NoArguments()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha.Alpha();
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha.Alpha()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_WithArguments()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Alpha, x.Beta, 7);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha(x.Alpha, x.Beta, '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_Chained_WithArguments()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha.Alpha(x.Alpha, x.Beta, 7);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha.Alpha(x.Alpha, x.Beta, '7')", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_Nested_NoArguments()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Alpha());
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha(x.Alpha())", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_Nested_WithArguments()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Alpha(x.Alpha));
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha(x.Alpha(x.Alpha))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_Generic_Type_Arguments_01()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha<int>(x.Beta);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha<Int32>(x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_Generic_Type_Arguments_02()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha<int, string>(x.Beta);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha<Int32, String>(x.Beta)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Combine_Method_Indexed_Setter_01()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Alpha = x.Alpha()[x]);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha()[x]))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Combine_Method_Indexed_Setter_02()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Alpha = x.Alpha()[x.Alpha = x.Beta]);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha()[(x.Alpha = x.Beta)]))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Combine_Method_Indexed_Setter_03()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Alpha = x.Alpha(x.Beta = x.Alpha)[x.Alpha = x.Beta]);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal(
            "x.Alpha((x.Alpha = x.Alpha((x.Beta = x.Alpha))[(x.Alpha = x.Beta)]))",
            node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Combine_Method_Indexed_Setter_04()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x(x.Alpha = x.Beta), x(x[x.Beta = x.Alpha]));
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal(
            "x.Alpha(x((x.Alpha = x.Beta)), x(x[(x.Beta = x.Alpha)]))",
            node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_Equal_Argument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

#pragma warning disable CS1718 // Comparison made to same variable
        func = x => x == x;
#pragma warning restore CS1718 // Comparison made to same variable
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x Equal x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_Equal_Dynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x == x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x Equal x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_Equal_Constant()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x == 1;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x Equal '1')", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_Equal_Argument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha == x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha Equal x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_Equal_Same_Dynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha == x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha Equal x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_Equal_Setter()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha == (x.Alpha = x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha Equal (x.Alpha = x.Alpha))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_OpSingleAnd_Argument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x & x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x And x)", node.ToString());

        var item = Assert.IsType<DynamicNodeBinary>(node);
        Assert.Equal(ExpressionType.And, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_OpSingleAnd_Dynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x & x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x And x.Alpha)", node.ToString());

        var item = Assert.IsType<DynamicNodeBinary>(node);
        Assert.Equal(ExpressionType.And, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_OpSingleAnd_Dynamic_And_Assign()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha = x.Alpha & x.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha = (x.Alpha And x.Beta))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_OpSingleAnd_Dynamic_Cascaded()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => (x & x.Alpha).Alpha = x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("((x And x.Alpha).Alpha = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Argument_OpDoubleAnd_Dynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x && x.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x And x.Beta)", node.ToString());

        var item = Assert.IsType<DynamicNodeBinary>(node);
        Assert.Equal(ExpressionType.And, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_OpDoubleAnd_Argument()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha && x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha And x)", node.ToString());

        var item = Assert.IsType<DynamicNodeBinary>(node);
        Assert.Equal(ExpressionType.And, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_OpDoubleAnd_Dynamic()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha && x.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha And x.Beta)", node.ToString());

        var item = Assert.IsType<DynamicNodeBinary>(node);
        Assert.Equal(ExpressionType.And, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Dynamic_OpDoubleAnd_Dynamic_And_Assign()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha = x.Alpha && x.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha = (x.Alpha And x.Beta))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_AddAssign()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha += x.Beta;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(x.Alpha = (x.Alpha AddAssign x.Beta))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Unary_Argument_Not()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => !x;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(Not x)", node.ToString());

        var item = Assert.IsType<DynamicNodeUnary>(node);
        Assert.Equal(ExpressionType.Not, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Unary_Dynamic_Not()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => !x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(Not x.Alpha)", node.ToString());

        var item = Assert.IsType<DynamicNodeUnary>(node);
        Assert.Equal(ExpressionType.Not, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Unary_Dynamic_Negate()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => -x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(Negate x.Alpha)", node.ToString());

        var item = Assert.IsType<DynamicNodeUnary>(node);
        Assert.Equal(ExpressionType.Negate, item.DynamicOperation);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Unary_PreIncrement()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => ++x.Alpha;
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("(Increment x.Alpha)", node.ToString());

        var item = Assert.IsType<DynamicNodeUnary>(node);
        Assert.Equal(ExpressionType.Increment, item.DynamicOperation);
    }

    //[Enforced]
    [Fact]
    public static void ERROR_NOT_SUPPORTED_Unary_PostIncrement()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        try
        {
            func = x => x.Beta = x.Alpha++;
            node = DynamicParser.Parse(func).Result;
            WriteLine($"\n> Result: {node}");
            Assert.Equal("(Increment x.Alpha)", node.ToString());

            var item = Assert.IsType<DynamicNodeUnary>(node);
            Assert.Equal(ExpressionType.Increment, item.DynamicOperation);
        }
        catch (Xunit.Sdk.EqualException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_01()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Cast(typeof(int), x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Cast('Int32', x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_02()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Cast<int>(x.Alpha);
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Cast<Int32>(x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_03()
    {
        Func<dynamic, object> func;
        DynamicNode node;

        func = x => x.Alpha(x.Cast<string>(x.Alpha = x.Beta));
        node = DynamicParser.Parse(func).Result;
        WriteLine($"\n> Result: {node}");
        Assert.Equal("x.Alpha(x.Cast<String>((x.Alpha = x.Beta)))", node.ToString());
    }
}