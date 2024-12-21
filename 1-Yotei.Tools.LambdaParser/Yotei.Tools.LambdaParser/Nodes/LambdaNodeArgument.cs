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
    /// <param name="name"></param>
    public LambdaNodeArgument(string name) : base()
    {
        LambdaName = ValidateName(name);
        LambdaParser.Print($"- New: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => LambdaName;

    /// <inheritdoc/>
    public override LambdaNodeArgument Clone() => new(LambdaName);

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => this;

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string LambdaName { get; }

    /// <summary>
    /// The parser associated to this instance, or null if any.
    /// </summary>
    internal LambdaParser? LambdaParser { get; set; }
}