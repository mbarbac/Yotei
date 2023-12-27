namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the dynamic argument used in a dynamic lambda expression.
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
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => LambdaName;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument Clone() => new(LambdaName);

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if any.
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => this;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string LambdaName { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The parser associated to this instance, or null if any.
    /// </summary>
    internal LambdaParser? LambdaParser { get; set; }
}