namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a direct invocation of a given host.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
/// <Notes>
/// Invoke nodes are used to modify the way other elements are emitted in a database command.
/// - If it is a first-level one with an unique string argument, then that string in injected
///   in the command as a literal and, by convention, it won't be captured as an argument.
/// - When an invoke node has several arguments, then their representations are just joined in
///   the produced database command.
/// - The above two capabilities, combined, permit to inject any arbitrary contents in the
///   database commands, for instance when using pre-defined command methods that, otherwise,
///   would not accept so.
/// </Notes>
public class DbTokenInvoke : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance with empty arguments.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    public DbTokenInvoke(IDbToken host) : base(host) => Arguments = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    public DbTokenInvoke(IDbToken host, IEnumerable<IDbToken> args) : base(host)
    {
        Arguments = DbToken.ToArguments(args, allowEmpty: true);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = $"({string.Join(", ", Arguments)})";
        return $"{Host}{str}";
    }

    /// <summary>
    /// The arguments of this instance, if any.
    /// </summary>
    public ImmutableArray<IDbToken> Arguments { get; }

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
        if (other is not DbTokenInvoke valid) return false;

        if (Arguments.Length != valid.Arguments.Length) return false;
        for (int i = 0; i < Arguments.Length; i++)
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
    public override bool Equals(object? obj) => Equals(obj as DbTokenInvoke);

    public static bool operator ==(DbTokenInvoke? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenInvoke? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Arguments);
        for (int i = 0; i < Arguments.Length; i++) code = HashCode.Combine(code, Arguments[i]);
        return code;
    }
}