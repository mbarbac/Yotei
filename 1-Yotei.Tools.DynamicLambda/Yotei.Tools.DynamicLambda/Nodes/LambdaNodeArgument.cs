namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents the dynamic arguments of a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeArgument : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public LambdaNodeArgument(string name)
    {
        DLambdaName = LambdaParser.ValidateName(name);
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => DLambdaName;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => this;

    // ----------------------------------------------------

    /// <summary>
    /// The name of this dynamic argument.
    /// </summary>
    public string DLambdaName { get; }
}