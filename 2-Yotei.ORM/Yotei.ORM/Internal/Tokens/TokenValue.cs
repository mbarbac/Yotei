namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary value in a database expression that is intended to be captured as an
/// argument.
/// </summary>
public sealed class TokenValue : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenValue(object? value) => Value = ValidateValue(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Value switch
    {
        bool item => item.ToString().ToUpper(),
        string item => $"'{item}'",
        null => "NULL",
        _ => $"'{Value.Sketch()}'"
    };

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object? ValidateValue(object? value)
    {
        if (value is LambdaNode or Delegate or Token)
            throw new ArgumentException("Value is not of a supported type.")
            .WithData(value);

        return value;
    }
}