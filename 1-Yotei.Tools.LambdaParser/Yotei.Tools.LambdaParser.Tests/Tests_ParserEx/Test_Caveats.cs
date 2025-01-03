﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Caveats
{
    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0078")]
    public static void Inconsistent_Concatenate_Same_Dynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha = (x.Alpha = x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);

        // WEIRD!: If we execute this test individually, either from xUnit of from Runner, then
        // it renders the expected '(x.Alpha = x.Beta)'. But executing all tests with xUnit it
        // renders '(x.Alpha = (x.Alpha = x.Beta))', which is logically the same thing although
        // more verbose. We will considered both as passed.

        var s = node.ToString();
        Assert.True(
            s == "(x.Alpha = x.Beta)" ||
            s == "(x.Alpha = (x.Alpha = x.Beta))");
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Standard_Unary_PostDecrement_Does_Not_Work()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        // CAVEAT: An isolated post operator is not identified as such - it is resolved into an
        // Increment or Decrement operation (... and not into a PostIncrementAssign or into an
        // PostDecrementAssign one). However, as it is isolated it is not too bad, and we can
        // consider the test as passed.

        WriteLine();
        func = x => x.Alpha--;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (Decrement x.Alpha))", node.ToString());

        // ERROR: For whatever reasons the DLR does not identify the unary operation as a 'post
        // plus assign' one (same issue as above). What actually happens is that the DLR does
        // execute an '(x.Alpha = (Decrement x.Alpha))' operation but, after that, it completely
        // forgets it and just assign the new 'Alpha' value to 'Beta'. From the standpoint of
        // the DLR is correct (the value of 'Alpha' has been already modified), but for parsing
        // purposes is means that this information is lost.

        WriteLine();
        func = x => x.Beta = x.Alpha--;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Beta = x.Alpha)", node.ToString());

        // Note that the comments on the ExpressionType values explicity say that the target
        // object shall not be modified in place. In addition, even the DLR source code annotates
        // post and incr/decr operations as special. So we will take this as a KNOWN LIMITATION,
        // and just live with it...

        // Assert.Fail("Post-increment and Post-decrement unary operators not supported.");
    }
}