namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a semantic version.
/// </summary>
public record SemanticVersion
{
    internal static char[] Uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    internal static char[] Lowers = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    internal static char[] Digits = "0123456789".ToCharArray();

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SemanticVersion Empty { get; } = new();
    private SemanticVersion() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="version"></param>
    public SemanticVersion(string version)
    {
        if (version == null || version.Length == 0) return;
        version = version.NotNullNotEmpty();

        string beta = string.Empty;
        var pos = version.IndexOf('-');
        if (pos >= 0)
        {
            beta = version[pos..];
            version = version[..pos];
        }
        var temps = version.Split('.');
        var major = temps.Length > 0 ? temps[0] : string.Empty;
        var minor = temps.Length > 1 ? temps[1] : string.Empty;
        var patch = temps.Length > 2 ? version[(major.Length + minor.Length + 2)..] : string.Empty;

        Major = int.TryParse(major, out var num) ? num : 0;
        Minor = int.TryParse(minor, out num) ? num : 0;
        Patch = int.TryParse(patch, out num) ? num : 0;
        Beta = new SemanticBeta(beta);
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Value;

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public bool IsEmpty =>
        Major == 0 &&
        Minor == 0 &&
        Patch == 0 &&
        Beta.IsEmpty;

    /// <summary>
    /// Gets a string with the value carried by this instance.
    /// </summary>
    public string Value => $"{Major}.{Minor}.{Patch}" + (Beta.IsEmpty ? string.Empty : $"-{Beta}");

    /// <summary>
    /// The major version.
    /// </summary>
    public int Major
    {
        get => _Major;
        init => _Major = value >= 0
            ? value
            : throw new ArgumentException($"Invalid major: {value}");
    }
    int _Major = 0;

    /// <summary>
    /// The minor version.
    /// </summary>
    public int Minor
    {
        get => _Minor;
        init => _Minor = value >= 0
            ? value
            : throw new ArgumentException($"Invalid minor: {value}");
    }
    int _Minor = 0;

    /// <summary>
    /// The patch level.
    /// </summary>
    public int Patch
    {
        get => _Patch;
        init => _Patch = value >= 0
            ? value
            : throw new ArgumentException($"Invalid patch: {value}");
    }
    int _Patch = 0;

    /// <summary>
    /// Represents the pre-release (beta) portion of this semantic version, without its heading
    /// '-' character, or an empty one.
    /// </summary>
    public SemanticBeta Beta
    {
        get => _Beta;
        init => _Beta = value.ThrowIfNull(nameof(Beta));
    }
    SemanticBeta _Beta = SemanticBeta.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance whose value has been increased according to the given options.
    /// The <paramref name="beta"/> value can be left to null unless a beta-alike increase is
    /// requested.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    public SemanticVersion Increase(SemanticOptions options, string? beta = null)
    {
        var temp = this;
        if (options.HasFlag(SemanticOptions.Major)) temp = temp with { Major = temp.Major + 1 };
        if (options.HasFlag(SemanticOptions.Minor)) temp = temp with { Minor = temp.Minor + 1 };
        if (options.HasFlag(SemanticOptions.Patch)) temp = temp with { Patch = temp.Patch + 1 };

        if (options.HasFlag(SemanticOptions.Beta) ||
            options.HasFlag(SemanticOptions.BetaExpand) ||
            options.HasFlag(SemanticOptions.BetaExpandPatch))
        {
            beta = SemanticBeta.ValidateValue(beta!);

            var value = Beta.IsEmpty
                ? beta
                : IncreaseBeta(new StringBuilder(temp.Beta.Value));

            temp = temp with { Beta = new SemanticBeta(value) };
        }

        return temp;

        /// <summary> Invoked when expanding the beta portion...
        /// </summary>
        string IncreaseBeta(StringBuilder value)
        {
            // Increasing characters in reverse order...
            var index = value.Length - 1;
            while (true)
            {
                if (index < 0)
                {
                    // We shall expand the path number...
                    if (options.HasFlag(SemanticOptions.BetaExpandPatch))
                    {
                        if (!options.HasFlag(SemanticOptions.Patch))
                            temp = temp with { Patch = temp.Patch + 1 };

                        return beta;
                    }

                    // We can expand the beta portion...
                    if (options.HasFlag(SemanticOptions.BetaExpand))
                    {
                        var head =
                            Array.IndexOf(Digits, value[0]) >= 0 ? Digits[0] :
                            Array.IndexOf(Lowers, value[0]) >= 0 ? Lowers[0] :
                            Uppers[0];

                        var other = new char[value.Length + 1];
                        Array.Copy(value.ToString().ToCharArray(), 0, other, 1, value.Length);
                        other[0] = head;

                        return new string(other);
                    }

                    // We cannot expand...
                    throw new SemanticException(
                        $"Cannot increase beta portion '{value}' because its size exhausted.");
                }

                // Increasing character at the current index...
                var carry = false;
                if (OnIncrease(Digits, out carry)) return value.ToString(); if (carry) continue;
                if (OnIncrease(Lowers, out carry)) return value.ToString(); if (carry) continue;
                if (OnIncrease(Uppers, out carry)) return value.ToString(); if (carry) continue;

                throw new UnExpectedException($"Invalid char '{value[index]}' found in '{value}'");
            }

            /// <summary> Invoked when increasing the character at the current index...
            /// </summary>
            bool OnIncrease(char[] valids, out bool carry)
            {
                var pos = Array.IndexOf(valids, value[index]);
                if (pos >= 0)
                {
                    if (pos == valids.Length - 1) // Carry over...
                    {
                        value[index] = valids[0];
                        index--;
                        carry = true;
                        return false;
                    }
                    else // Regular increase...
                    {
                        value[index] = valids[pos + 1];
                        carry = false;
                        return true;
                    }
                }

                // Character at current index not found...
                carry = false;
                return false;
            }
        }
    }
}