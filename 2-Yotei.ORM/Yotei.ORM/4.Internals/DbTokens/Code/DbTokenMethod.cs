namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a method invocation on a given host token.
/// </summary>
[Cloneable]
public partial class DbTokenMethod : DbTokenHosted
{
    /// <summary>
    /// Initializes a new parameter-less instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public DbTokenMethod(IDbToken host, string name) : this(host, [], name, []) { }

    /// <summary>
    /// Initializes a new instance with the given name and regular arguments
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="args"></param>
    public DbTokenMethod(
        IDbToken host, string name, IEnumerable<IDbToken> args) : this(host, [], name, args) { }

    /// <summary>
    /// Initializes a new parameter-less instance with the given name and type arguments.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="types"></param>
    /// <param name="name"></param>
    public DbTokenMethod(
        IDbToken host, IEnumerable<Type> types, string name) : this(host, types, name, []) { }

    /// <summary>
    /// Initializes a new instance with the given name, type arguments, and regular ones.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="types"></param>
    /// <param name="name"></param>
    /// <param name="args"></param>
    public DbTokenMethod(
        IDbToken host, IEnumerable<Type> types, string name, IEnumerable<IDbToken> args) : base(host)
    {
        Name = DbToken.ValidateTokenName(name);
        TypeArguments = DbToken.ToTypeArguments(types);
        Arguments = new(args);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenMethod(DbTokenMethod source) : this(
        source.Host.Clone(),
        source.TypeArguments,
        source.Name,
        source.Arguments.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var types = TypeArguments.Length == 0
            ? string.Empty
            : $"<{string.Join(", ", TypeArguments.Select(x => x.EasyName()))}>";

        var args = Arguments.ToString(rounded: true);

        return $"{Host}.{Name}{types}{args}";
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenMethod valid) return false;

        if (Name != valid.Name) return false;
        if (!Arguments.Equals(valid.Arguments)) return false;

        if (TypeArguments.Length != valid.TypeArguments.Length) return false;
        for (int i = 0; i < TypeArguments.Length; i++)
        {
            var item = TypeArguments[i];
            var temp = valid.TypeArguments[i];
            if (item != temp) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenMethod? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenMethod? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, TypeArguments);
        code = HashCode.Combine(code, Arguments);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The method's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The collection of type arguments used by this method, or an empty one if any.
    /// </summary>
    public ImmutableArray<Type> TypeArguments { get; }

    /// <summary>
    /// The arguments used by this instance, which may be an empty collection.
    /// </summary>
    public DbTokenChain Arguments { get; }
}