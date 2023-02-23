namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a method invocation in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeMethod : DynamicNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public DynamicNodeMethod(
        DynamicNode host, string name, IEnumerable<DynamicNode> arguments)
        : base(host)
    {
        DynamicName = ValidateDynamicName(name);
        DynamicArguments = ValidateDynamicArguments(arguments, canbeEmpty: true);
        DynamicTypeArguments = ImmutableList<Type>.Empty;
        DebugPrintNew();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="arguments"></param>
    public DynamicNodeMethod(
        DynamicNode host, string name, IEnumerable<Type> types, IEnumerable<DynamicNode> arguments)
        : base(host)
    {
        DynamicName = ValidateDynamicName(name);
        DynamicArguments = ValidateDynamicArguments(arguments, canbeEmpty: true);
        DynamicTypeArguments = ValidateGenericArguments(types);
        DebugPrintNew();
    }

    static IImmutableList<Type> ValidateGenericArguments(IEnumerable<Type> types)
    {
        types = types.ThrowIfNull();

        var items = types.ToList();
        if (items.Count == 0) throw new EmptyException(
            "List of generic type arguments cannot be empty.");

        if (items.Any(x => x == null)) throw new ArgumentException(
            "List of generic type arguments contains null elements.")
            .WithData(items, nameof(types));

        return items.ToImmutableList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var sb = new StringBuilder($"{DynamicHost}.{DynamicName}");

        if (DynamicTypeArguments.Count > 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", DynamicTypeArguments.Select(x => x.EasyName())));
            sb.Append('>');
        }

        sb.Append('(');
        sb.Append(string.Join(", ", DynamicArguments.Select(x => x.ToString())));
        sb.Append(')');

        return sb.ToString();
    }

    /// <summary>
    /// The name of the dynamic method.
    /// </summary>
    public string DynamicName { get; }

    /// <summary>
    /// The collection of generic type arguments used to define the method.
    /// </summary>
    public IImmutableList<Type> DynamicTypeArguments { get; }

    /// <summary>
    /// The arguments to use for the method invocation. This collection may be an empty one.
    /// </summary>
    public IImmutableList<DynamicNode> DynamicArguments { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicNodeArgument? GetArgument() => DynamicHost.GetArgument();
}