namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a semantic version.
/// </summary>
public record SemanticVersion
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static SemanticVersion Empty { get; } = new();
    protected SemanticVersion() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public SemanticVersion(string value)
    {
        value = value.ThrowIfNull().Trim();

        var beta = string.Empty;
        var pos = value.IndexOf('-');
        if (pos > 0)
        {
            beta = value[pos..];
            value = value[..pos];
        }

        var temps = value.Split('.');
        var major = temps.Length > 0 ? temps[0] : string.Empty;
        var minor = temps.Length > 1 ? temps[1] : string.Empty;
        var patch = temps.Length > 2 ? value[(major.Length + minor.Length + 2)..] : string.Empty;

        Major = int.TryParse(major, out var num) ? num : 0;
        Minor = int.TryParse(minor, out num) ? num : 0;
        Patch = int.TryParse(patch, out num) ? num : 0;
        Beta = beta;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Value;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(SemanticVersion beta) => beta.Value;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator SemanticVersion(string value) => new(value);

    /// <summary>
    /// Gets a string with the value carried by this instance.
    /// </summary>
    public string Value =>
        $"{Major}.{Minor}.{Patch}" + (Beta.Value.Length == 0 ? string.Empty : $"-{Beta}");

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

    /// <summary>
    /// Returns a new instance where its original value has been increased according to the given
    /// options. The out <paramref name="carry"/> argument indicates if the beta portion has been
    /// exhausted and the path value has been increased because of it.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public SemanticVersion Increase(SemanticOptions options, out bool carry)
    {
        var temp = this;
        carry = false;

        if (options.HasFlag(SemanticOptions.Major)) temp = temp with { Major = temp.Major + 1 };
        if (options.HasFlag(SemanticOptions.Minor)) temp = temp with { Minor = temp.Minor + 1 };
        if (options.HasFlag(SemanticOptions.Patch)) temp = temp with { Patch = temp.Patch + 1 };

        if (options.HasFlag(SemanticOptions.Beta) ||
            options.HasFlag(SemanticOptions.BetaExpand))
        {
            var beta = Beta.Increase(options, out var carried);
            temp = temp with { Beta = beta };

            if (carried && !options.HasFlag(SemanticOptions.Patch))
            {
                temp = temp = temp with { Patch = temp.Patch + 1 };
                carry = true;
            }
        }

        return temp;
    }
}