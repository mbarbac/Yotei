namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a method invocation operation in a chain of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class LambdaNodeMethod : LambdaNodeHosted
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
        LambdaName = LambdaParser.ValidateName(name);
        LambdaArguments = LambdaParser.ValidateArguments(arguments, canBeEmpty: true);
        LambdaTypeArguments = [];
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
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
        LambdaName = LambdaParser.ValidateName(name);
        LambdaArguments = LambdaParser.ValidateArguments(arguments, canBeEmpty: true);
        LambdaTypeArguments = LambdaParser.ValidateTypeArguments(types);
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{LambdaHost}.{LambdaName}");

        if (LambdaTypeArguments.Length != 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", LambdaTypeArguments.Select(static x => x.EasyName())));
            sb.Append('>');
        }
        sb.Append('(');
        sb.Append(string.Join(", ", LambdaArguments.Select(static x => x.ToString())));
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
    public ImmutableArray<Type> LambdaTypeArguments { get; }

    /// <summary>
    /// The collection of arguments used for the method invocation operation, which can be an
    /// empty one when needed.
    /// </summary>
    public ImmutableArray<LambdaNode> LambdaArguments { get; }
}