namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a hosted dynamic method invocation operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeMethod : DLambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public DLambdaNodeMethod(DLambdaNode host,
        string name,
        IEnumerable<DLambdaNode> arguments) : base(host)
    {
        DLambdaName = DLambdaParser.ValidateName(name);
        DLambdaArguments = DLambdaParser.ValidateArguments(arguments, canBeEmpty: true);
        DLambdaTypeArguments = [];
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="arguments"></param>
    public DLambdaNodeMethod(
        DLambdaNode host,
        string name,
        IEnumerable<Type> types,
        IEnumerable<DLambdaNode> arguments) : base(host)
    {
        DLambdaName = DLambdaParser.ValidateName(name);
        DLambdaArguments = DLambdaParser.ValidateArguments(arguments, canBeEmpty: true);
        DLambdaTypeArguments = DLambdaParser.ValidateTypeArguments(types);
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{DLambdaHost}.{DLambdaName}");

        if (DLambdaTypeArguments.Length != 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", DLambdaTypeArguments.Select(static x => x.EasyName())));
            sb.Append('>');
        }
        sb.Append('(');
        sb.Append(string.Join(", ", DLambdaArguments.Select(static x => x.ToString())));
        sb.Append(')');

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic method.
    /// </summary>
    public string DLambdaName { get; }

    /// <summary>
    /// The collection of types used as the generic type arguments of the method invocation, or
    /// an empty one if no generic type arguments are used.
    /// </summary>
    public ImmutableArray<Type> DLambdaTypeArguments { get; }

    /// <summary>
    /// The collection of arguments used for the method invocation operation, which can be an
    /// empty one when needed.
    /// </summary>
    public ImmutableArray<DLambdaNode> DLambdaArguments { get; }
}