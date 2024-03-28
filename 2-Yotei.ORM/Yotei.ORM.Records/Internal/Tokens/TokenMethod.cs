namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents the invocation of a method on a given host.
/// </summary>
public sealed class TokenMethod : TokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public TokenMethod(
        Token host, string name) : this(host, name, [], []) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public TokenMethod(
        Token host, string name, IEnumerable<Token> arguments) : this(host, name, [], arguments) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    public TokenMethod(
        Token host, string name, IEnumerable<Type> types) : this(host, name, types, []) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="arguments"></param>
    public TokenMethod(
        Token host,
        string name,
        IEnumerable<Type> types,
        IEnumerable<Token> arguments) : base(host)
    {
        Name = ValidateTokenName(name);
        TypeArguments = GetTypeArguments(types);
        Arguments = new TokenChain(arguments);
    }

    // Each element might actually be an instance of the internal 'RuntimeType' class, which is
    // a concrete implementation of the abstract 'Type' one.
    static ImmutableArray<Type> GetTypeArguments(IEnumerable<Type> types)
    {
        types.ThrowWhenNull();

        var list = new List<Type>();
        foreach (var item in types)
        {
            var temp = (item as Type) ?? throw new ArgumentException(
                "List of generic types carries null elements.")
                .WithData(types);

            list.Add(temp);
        }
        return list.ToImmutableArray();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string ToString()
    {
        var types = TypeArguments.Length == 0
            ? string.Empty
            : $"<{string.Join(", ", TypeArguments.Select(x => x.EasyName()))}>";

        var args = Arguments.ToString('(', ", ", ')');

        return $"{Host}.{Name}{types}{args}";
    }

    /// <summary>
    /// The name of this method.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The generic arguments of this method, or an empty collection if any.
    /// </summary>
    public ImmutableArray<Type> TypeArguments { get; }

    /// <summary>
    /// The arguments used by this instance.
    /// </summary>
    public TokenChain Arguments { get; }
}