﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic binary operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeBinary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public LambdaNodeBinary(
        LambdaParser parser,
        LambdaNode left, ExpressionType operation, LambdaNode right) : base(parser)
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaOperation = operation;
        LambdaRight = right.ThrowWhenNull();

        LambdaHelpers.Print(LambdaHelpers.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaLeft} {LambdaOperation} {LambdaRight})";

    /// <inheritdoc/>
    public override LambdaNodeBinary Clone() => new(
        LambdaParser,
        LambdaLeft.Clone(),
        LambdaOperation,
        LambdaRight.Clone());

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaLeft { get; }

    /// <summary>
    /// The dynamic binary operation represented by this instance.
    /// <br/> The caller is responsable for setting an appropriate value.
    /// </summary>
    public ExpressionType LambdaOperation { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaRight { get; }
}