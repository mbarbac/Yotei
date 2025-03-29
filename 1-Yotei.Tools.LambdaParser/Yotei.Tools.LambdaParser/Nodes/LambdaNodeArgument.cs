namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic argument used in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeArgument : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="name"></param>
    public LambdaNodeArgument(
        LambdaParser parser, string name) : base(parser)
    {
        LambdaName = ValidateName(name);

        LambdaDebug.Print(LambdaDebug.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => LambdaName;

    /// <inheritdoc/>
    public override LambdaNodeArgument Clone() => new(LambdaParser, LambdaName);

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string LambdaName { get; }
}