namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic argument in a tree of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class LambdaNodeArgument : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public LambdaNodeArgument(string name) : base()
    {
        LambdaName = ValidateLambdaName(name);
        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => LambdaName;

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => this;

    // ----------------------------------------------------

    /// <summary>
    /// The name of this dynamic argument.
    /// </summary>
    public string LambdaName { get; }
}