namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents the beta (pre-release) part of a semantic version.
/// </summary>
public record SemanticBeta
{
    public static ImmutableArray<char> ValidDigits = "0123456789".ToImmutableArray();
    public static ImmutableArray<char> ValidLowers = "abcdefghijklmnopqrstuvwxyz".ToImmutableArray();
    public static ImmutableArray<char> ValidUppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToImmutableArray();

    /// <summary>
    /// An empty instance.
    /// </summary>
    public static SemanticBeta Empty { get; } = new();
    protected SemanticBeta() { }

    /// <summary>
    /// Initializes a new instanace.
    /// </summary>
    /// <param name="value"></param>
    public SemanticBeta(string value) => Value = value;

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Value;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(SemanticBeta beta) => beta.Value;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator SemanticBeta(string value) => new(value);

    /// <summary>
    /// The value carried by this instance, without its heading '-' character.
    /// </summary>
    public string Value
    {
        get => _Value;
        init => _Value = Validate(value);
    }
    string _Value = string.Empty;

    /// <summary>
    /// Determines if this instance is an empty one or not.
    /// </summary>
    public bool IsEmpty => _Value.Length == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where its original value has been increased according to the given
    /// options. Empty instances cannot be increased, and this method throws an exception. If not
    /// expansion is requested, the out <paramref name="carry"/> argument indicates if the beta
    /// portion has been exhausted or not.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="carry"></param>
    /// <returns></returns>
    public SemanticBeta Increase(SemanticOptions options, out bool carry)
    {
        var expand = options.HasFlag(SemanticOptions.BetaExpand);
        carry = false;

        // Special case when this instance is an empty one...
        if (Value.Length == 0)
        {
            throw new InvalidOperationException(
                "Cannot increase the value of an empty beta version instance.");
        }

        // Standard case...
        var value = new StringBuilder(Value);
        int index = value.Length - 1;

        while (true)
        {
            // Expansion is required...
            if (index < 0)
            {
                if (expand) // Expansion is allowed...
                {
                    var head =
                        ValidDigits.IndexOf(value[0]) >= 0 ? ValidDigits[0] :
                        ValidLowers.IndexOf(value[0]) >= 0 ? ValidLowers[0] :
                        ValidUppers[0];

                    value = value.Insert(0, head);
                    return value.ToString();
                }
                else // Expansion not allowed, request carry over...
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        value[i] =
                            ValidDigits.IndexOf(value[i]) >= 0 ? ValidDigits[0] :
                            ValidLowers.IndexOf(value[i]) >= 0 ? ValidLowers[0] :
                            ValidUppers[0];
                    }

                    carry = true;
                    return value.ToString();
                }
            }

            // Increasing the character at the current index...
            bool decreased = false;
            if (OnIncrease(ValidDigits, out decreased)) return value.ToString(); if (decreased) continue;
            if (OnIncrease(ValidLowers, out decreased)) return value.ToString(); if (decreased) continue;
            if (OnIncrease(ValidUppers, out decreased)) return value.ToString(); if (decreased) continue;

            throw new UnExpectedException($"Invalid char '{value[index]}' found in '{value}'");
        }

        /// <summary> Invoked when increasing the character at the current index.
        /// </summary>
        bool OnIncrease(ImmutableArray<char> valids, out bool decreased)
        {
            // Finding a valid character...
            var pos = valids.IndexOf(value[index]);
            if (pos >= 0)
            {
                if (pos == valids.Length - 1) // Decrease index...
                {
                    value[index] = valids[0];
                    index--;
                    decreased = true;
                    return false;
                }
                else // Standard case...
                {
                    value[index] = valids[pos + 1];
                    decreased = false;
                    return true;
                }
            }

            // Character not found...
            decreased = false;
            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated beta string, or throws an exception. Validated beta string are either
    /// empty ones, or contains only valid characters, where its heading '-' one is removed if
    /// needed.
    /// </summary>
    /// <param name="beta"></param>
    /// <returns></returns>
    public static string Validate(string beta)
    {
        beta = beta.ThrowIfNull().Trim();

        while (beta.StartsWith('-')) beta = beta[1..];

        for (int i = 0; i < beta.Length; i++)
            if (!ValidDigits.Contains(beta[i]) &&
                !ValidLowers.Contains(beta[i]) &&
                !ValidUppers.Contains(beta[i]))
                throw new ArgumentException(
                    $"Beta string contains invalid characters: {beta}");

        return beta;
    }
}