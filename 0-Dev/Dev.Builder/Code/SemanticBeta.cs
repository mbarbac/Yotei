namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents the pre-release (beta) portion of a semantic version. This portion does not
/// implement completely the specification, and only accepts letters and digits.
/// </summary>
public record SemanticBeta
{
    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SemanticBeta Empty { get; } = new();
    private SemanticBeta() { }

    /// <summary>
    /// Initializes a new value.
    /// </summary>
    /// <param name="value"></param>
    public SemanticBeta(string value)
    {
        if (value == null || value.Length == 0) return;

        if (value.StartsWith('-')) value = value[1..].NotNullNotEmpty();
        Value = value;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Value;

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public bool IsEmpty => Value.Length == 0;

    /// <summary>
    /// The value carried by this instance, without its heading '-' character.
    /// </summary>
    public string Value
    {
        get => _Value;
        init => _Value = value == null || value.Length == 0
            ? string.Empty
            : ValidateValue(value);
    }
    string _Value = string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated beta value, or throws an appropriate exception.
    /// </summary>
    /// <param name="beta"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ValidateValue(string beta)
    {
        beta = beta.NotNullNotEmpty();

        for (int i = 0; i < beta.Length; i++)
            if (!SemanticVersion.Digits.Contains(beta[i]) &&
                !SemanticVersion.Lowers.Contains(beta[i]) &&
                !SemanticVersion.Uppers.Contains(beta[i]))
                throw new ArgumentException(
                    $"Invalid char '{beta[i]}' found in value '{beta}'.");

        return beta;
    }
}