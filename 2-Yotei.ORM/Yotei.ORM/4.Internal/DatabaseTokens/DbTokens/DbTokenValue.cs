namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary value in a database expression that typically is intended to be
/// captured as an argument.
/// </summary>
public class DbTokenValue : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenValue(object? value) => Value = Value = ValidateValue(value);

    /// <inheritdoc/>
    public override string ToString() => Value switch
    {
        bool item => item.ToString().ToUpper(),
        string item => $"'{item}'",
        null => "NULL",
        _ => $"'{Value.Sketch()}'"
    };

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => null;

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? Value { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object? ValidateValue(object? value)
    {
        if (value is LambdaNode or Delegate or DbToken)
            throw new ArgumentException("Value is not of a supported type.")
            .WithData(value);

        return value;
    }
}