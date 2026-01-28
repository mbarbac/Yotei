namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Caveats
{
    //[Enforced]
    [Fact]
    public static void SetMember_Parse_Assign_Assigment_To_Itself_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha = (x.Alpha = x.Beta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);

        // For unknown reasons the DLR understands both are equivalent and prefers the second,
        // but not always (!!??), and there is no consistency among test runs (in particular,
        // but not only, when the tests where executed from a console application).
        var str = node.ToString();
        var eq1 = str == "(x.Alpha = (x.Alpha = x.Beta))"; Debug.WriteLine($"> Eq1: {eq1}");
        var eq2 = str == "(x.Alpha = x.Beta)"; Debug.WriteLine($"> Eq2: {eq2}");
        Assert.True(eq1 || eq2);
    }
}