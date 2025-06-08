namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic argument used in a dynamic lambda expression.
/// <para>Instances of this class are intended to be immutable ones.</para>
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
        LambdaName = LambdaHelpers.ValidateName(name);
        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString() => LambdaName;

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => this;

    /// <inheritdoc/>
    public override LambdaNodeArgument Clone() => new(LambdaName);

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string LambdaName { get; }
}