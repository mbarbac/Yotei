namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a method invocation on a given host token.
public class DbTokenMethod : DbTokenHosted
{
    /// <summary>
    /// Initializes a new parameter-less instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public DbTokenMethod(DbToken host, string name) : this(host, name, [], []) { }

    /// <summary>
    /// Initializes a new instance with the given name and regular arguments
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="args"></param>
    public DbTokenMethod(
        DbToken host, string name, IEnumerable<DbToken> args) : this(host, name, [], args) { }

    /// <summary>
    /// Initializes a new parameter-less instance with the given name and type arguments.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    public DbTokenMethod(
        DbToken host, string name, IEnumerable<Type> types) : this(host, name, types, []) { }

    /// <summary>
    /// Initializes a new instance with the given name, type arguments, and regular ones.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="args"></param>
    public DbTokenMethod(
        DbToken host, string name, IEnumerable<Type> types, IEnumerable<DbToken> args) : base(host)
    {
        Name = ValidateTokenName(name);
        TypeArguments = ToTypeArguments(types);
        Arguments = new(args);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an immutable array of types based upon the given collection.
    /// </summary>
    static ImmutableArray<Type> ToTypeArguments(IEnumerable<Type> types)
    {
        types.ThrowWhenNull();

        var list = new List<Type>(); foreach (var type in types)
        {
            if (type is null) throw new ArgumentException(
                "Collection of types carries null elements.")
                .WithData(types);

            list.Add(type);
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

        var args = Arguments.ToString(true);

        return $"{Host}.{Name}{types}{args}";
    }

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenMethod xother)
        {
            if (Name !=  xother.Name) return false;
            if (!Arguments.Equals(xother.Arguments)) return false;

            if (TypeArguments.Length != xother.TypeArguments.Length) return false;
            for (int i = 0; i < TypeArguments.Length; i++)
            {
                var item = TypeArguments[i];
                var temp = xother.TypeArguments[i];
                if (item != temp) return false;
            }
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, TypeArguments);
        code = HashCode.Combine(code, Arguments);
        return code;
    }
}