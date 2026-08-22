namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary value in a database expression. Values carried by instances of this
/// type are typically captured as command arguments.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenValue : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenValue(object? value) => Value = ValidateValue(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value switch
    {
        bool item => item.ToString().ToUpper(),
        string item => $"'{item}'",
        null => "NULL",
        _ => $"'{Value.Sketch()}'"
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => null;

    /// <summary>
    /// The value carried by this instance.
    /// </summary>
    public object? Value { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Validates and returns the given value, or throws an appropriate exception.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object? ValidateValue(object? value) => value switch
    {
        LambdaNode => throw new ArgumentException("Not supported token value.").WithData(value),
        Delegate => throw new ArgumentException("Not supported token value.").WithData(value),
        IDbToken => throw new ArgumentException("Not supported token value.").WithData(value),
        _ => value
    };

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenValue valid) return false;

        if (Value.EqualsEx(valid.Value)) return true;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenValue);

    public static bool operator ==(DbTokenValue? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenValue? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Value);
        return code;
    }
}