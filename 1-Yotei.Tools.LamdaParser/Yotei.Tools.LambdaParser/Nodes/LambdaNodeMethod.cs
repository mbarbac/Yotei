namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic method invocation operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeMethod : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public LambdaNodeMethod(
        LambdaNode host,
        string name,
        IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaName = ValidateLambdaName(name);
        LambdaArguments = ValidateLambdaArguments(arguments, canbeEmpty: true);
        LambdaTypeArguments = ImmutableList<Type>.Empty;

        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="arguments"></param>
    public LambdaNodeMethod(
        LambdaNode host,
        string name,
        IEnumerable<Type> types,
        IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaName = ValidateLambdaName(name);
        LambdaArguments = ValidateLambdaArguments(arguments, canbeEmpty: true);
        LambdaTypeArguments = ValidateTypeArguments(types);

        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{LambdaHost}.{LambdaName}");

        if (LambdaTypeArguments.Count != 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", LambdaTypeArguments.Select(x => x.EasyName())));
            sb.Append('>');
        }
        sb.Append('(');
        sb.Append(string.Join(", ", LambdaArguments.Select(x => x.ToString())));
        sb.Append(')');

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic method.
    /// </summary>
    public string LambdaName { get; }

    /// <summary>
    /// The collection of types used as the generic type arguments of the method invocation, or
    /// an empty one if no generic type arguments are used.
    /// </summary>
    public IImmutableList<Type> LambdaTypeArguments { get; }

    /// <summary>
    /// The collection of arguments used for the method invocation operation, which can be an
    /// empty one when needed.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}