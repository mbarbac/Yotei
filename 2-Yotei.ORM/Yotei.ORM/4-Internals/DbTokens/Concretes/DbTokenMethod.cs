namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a method invocation of a given host.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenMethod : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="ignoreNameCase"></param>
    public DbTokenMethod(
        IDbToken host,
        string name, bool ignoreNameCase)
        : this(host, name, ignoreNameCase, [], []) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="ignoreNameCase"></param>
    /// <param name="types"></param>
    public DbTokenMethod(
        IDbToken host,
        string name, bool ignoreNameCase,
        IEnumerable<Type> types)
        : this(host, name, ignoreNameCase, types, []) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="ignoreNameCase"></param>
    /// <param name="args"></param>
    public DbTokenMethod(
        IDbToken host,
        string name, bool ignoreNameCase,
        IEnumerable<IDbToken> args)
        : this(host, name, ignoreNameCase, [], args) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="ignoreNameCase"></param>
    /// <param name="types"></param>
    /// <param name="args"></param>
    public DbTokenMethod(
        IDbToken host,
        string name, bool ignoreNameCase,
        IEnumerable<Type> types,
        IEnumerable<IDbToken> args) : base(host)
    {
        IgnoreNameCase = ignoreNameCase;
        Name = DbToken.ValidateTokenName(name);
        TypeArguments = DbToken.ToTypeArguments(types, allowEmpty: true);
        Arguments = DbToken.ToArguments(args, allowEmpty: true);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string str;
        var sb = new StringBuilder();
        sb.Append($"{Host}.{Name}");

        if (TypeArguments.Length > 0)
        {
            str = $"<{string.Join(", ", TypeArguments.Select(x => x.EasyName()))}>";
            sb.Append(str);
        }

        str = Arguments.ToString("(", ")", ", ");
        sb.Append(str);

        return sb.ToString();
    }

    /// <summary>
    /// Determines if when comparing this instance to another one the case of the method name
    /// shall be ignored or not.
    /// </summary>
    public bool IgnoreNameCase { get; }

    /// <summary>
    /// The name of the method.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type arguments of this instance, if any.
    /// </summary>
    public ImmutableArray<Type> TypeArguments { get; }

    /// <summary>
    /// The regular arguments of this instance, if any.
    /// </summary>
    public DbTokenChain Arguments { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenMethod valid) return false;

        if (IgnoreNameCase != valid.IgnoreNameCase) return false;
        if (string.Compare(Name, valid.Name, IgnoreNameCase) != 0) return false;

        if (TypeArguments.Length != valid.TypeArguments.Length) return false;
        for (int i = 0; i < TypeArguments.Length; i++)
        {
            var item = TypeArguments[i];
            var temp = valid.TypeArguments[i];
            if (item != temp) return false;
        }

        if (Arguments.Count != valid.Arguments.Count) return false;
        for (int i = 0; i < Arguments.Count; i++)
        {
            var item = Arguments[i];
            var temp = valid.Arguments[i];
            var same = item.Equals(temp);
            if (!same) return false;
        }
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenMethod);

    public static bool operator ==(DbTokenMethod? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenMethod? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, IgnoreNameCase);
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, TypeArguments);
        for (int i = 0; i < TypeArguments.Length; i++) code = HashCode.Combine(code, TypeArguments[i]);
        code = HashCode.Combine(code, Arguments);
        for (int i = 0; i < Arguments.Count; i++) code = HashCode.Combine(code, Arguments[i]);
        return code;
    }
}