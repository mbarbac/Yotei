namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic method invocation operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeMethod : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public LambdaNodeMethod(
        LambdaNode host, string name, IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaName = ValidateLambdaName(name);
        LambdaArguments = ValidateLambdaArguments(arguments, canBeEmpty: true);
        LambdaTypeArguments = ImmutableList<Type>.Empty;
        PrintInitialized();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public LambdaNodeMethod(
        LambdaNode host, string name, IEnumerable<Type> types, IEnumerable<LambdaNode> arguments)
        : base(host)
    {
        LambdaName = ValidateLambdaName(name);
        LambdaArguments = ValidateLambdaArguments(arguments, canBeEmpty: true);
        LambdaTypeArguments = ValidateLambdaTypes(types);
        PrintInitialized();
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeMethod Clone()
    {
        if (LambdaTypeArguments.Count == 0)
        {
            return new(
                LambdaHost.Clone(),
                LambdaName,
                LambdaArguments.Select(x => x.Clone()).ToImmutableList());
        }
        else
        {
            return new(
                LambdaHost.Clone(),
                LambdaName,
                LambdaTypeArguments,
                LambdaArguments.Select(x => x.Clone()).ToImmutableList());
        }
    }

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string LambdaName { get; }

    /// <summary>
    /// The collection of generic type arguments of the given method, or an empty list if any.
    /// </summary>
    public IImmutableList<Type> LambdaTypeArguments { get; }

    /// <summary>
    /// The arguments to use for the method invocation, or an empty list if any.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}