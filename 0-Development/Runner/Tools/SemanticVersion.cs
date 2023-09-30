namespace Runner;

// ========================================================
/// <summary>
/// Represents the optional pre-release portion of a semantic version, composed by its value and
/// metadata parts. From 'https://semver.org':
/// <br/> A normal version must take the form 'X.Y.Z', where X, Y and Z are are non-negative
/// integers, and must NOT contain leading zeroes. X is the major version, Y is the minor version,
/// and Z is the patch version.
/// <br/> A pre-release version may be denoted by appending a hyphen and a series of dot separated
/// identifiers immediately following the patch version.Identifiers must comprise only ASCII
/// alphanumerics and hyphens[0 - 9A - Za - z -], and must not be empty. Numeric identifiers must
/// not  include leading zeroes. Pre-release versions have a lower precedence than the associated
/// normal version.
/// <br/> Metadata may be denoted by appending a plus sign and a series of dot separated
/// identifiers immediately following the patch or pre-release version.Identifiers must comprise
/// only ASCII alphanumerics and hyphens[0 - 9A - Za - z -]. Identifiers must not be empty.
/// <br/> Precedence must be calculated by separating the version into major, minor, patch and
/// pre-release identifiers in that order.Precedence is determined by the first difference when
/// comparing each of these identifiers from left to right.Major, minor, and patch versions are
/// always compared numerically.
/// <br/> Precedence for two pre-release versions is determined by comparing each dot separated
/// identifier from left to right until a difference is found.Identifiers consisting of only
/// digits are compared numerically.Identifiers with letters or hyphens are compared lexically
/// in ASCII sort order.Numeric identifiers always have lower precedence than non-numeric
/// identifiers.A larger set of pre-release fields has a higher precedence than a smaller set,
/// if all of the preceding identifiers are equal.
/// <br/> When comparing precedence, build metadata shall be ignored.
/// </summary>
public record SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    // <summary>
    /// A shared empty instance.
    /// </summary>
    public static SemanticRelease Empty { get; } = new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SemanticVersion() { }

    /// <summary>
    /// Initializes a new instance with the given parts.
    /// </summary>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="patch"></param>
    /// <param name="preRelease"></param>
    public SemanticVersion(int major, int minor, int patch, SemanticRelease preRelease)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
    }

    /// <summary>
    /// Initializes a new instance with the values obtained from the given string.
    /// </summary>
    /// <param name="value"></param>
    public SemanticVersion(string value)
    {
        value = value.ThrowWhenNull();

        if (value.Length == 0) return;
        int index;
        string str;

        index = value.IndexOf('-'); if (index >= 0)
        {
            PreRelease = value[index..];
            value = value[..index];
        }
        index = value.IndexOf('+'); if (index >= 0)
        {
            if (!PreRelease.IsEmpty) throw new ArgumentException(
                "Build metadata cannot come before than the pre-release version.")
                .WithData(value);

            PreRelease = value[index..];
            value = value[..index];
        }

        NotTrailingDot(value);
        if (value.Length > 0)
        {
            index = value.IndexOf('.');
            str = index < 0 ? value : value[..index];

            Major = ValidatedValue(str, "Major");
            value = index < 0 ? string.Empty : value.Remove(str + '.');
        }

        NotTrailingDot(value);
        if (value.Length > 0)
        {
            index = value.IndexOf('.');
            str = index < 0 ? value : value[..index];

            Minor = ValidatedValue(str, "Minor");
            value = index < 0 ? string.Empty : value.Remove(str + '.');
        }

        NotTrailingDot(value);
        if (value.Length > 0)
        {
            index = value.IndexOf('.');
            str = index < 0 ? value : value[..index];

            Patch = ValidatedValue(str, "Patch");
            value = index < 0 ? string.Empty : value.Remove(str + '.');
        }
    }

    // Returns a validated value for the major, minor and patch parts...
    int ValidatedValue(string value, string description)
    {
        if (value.Length == 0) throw new ArgumentException(
            $"{description} is empty.")
            .WithData(value);

        if (!value.All(char.IsDigit)) throw new ArgumentException(
            $"{description} carries non-digit characters.")
            .WithData(value);

        if (value.Length > 1 && value[0] == '0')
            throw new ArgumentException($"{description} carries leading ceroes.")
            .WithData(value);

        return int.Parse(value);
    }

    // Throws an exception if the value ends with a dot.
    static void NotTrailingDot(string value)
    {
        if (value.Length > 0 && value[^1] == '.')
            throw new ArgumentException("Value ends with a dot.")
            .WithData(value);
    }

    /// <summary>
    /// <inheritdoc/> This emthod always emit the major and minor versions, even if they are
    /// cero.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = $"{Major}.{Minor}.{Patch}";
        if (!PreRelease.IsEmpty)
        {
            if (PreRelease.Value.Length > 0) str += $"-{PreRelease.Value}";
            if (PreRelease.Metadata.Length > 0) str += $"+{PreRelease.Metadata}";
        }
        return str;
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(SemanticVersion value)
    {
        value = value.ThrowWhenNull();
        return value.ToString();
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticVersion(string value) => new(value);

    /// <summary>
    /// The major version carried by this instance.
    /// </summary>
    public int Major
    {
        get => _Major;
        init => _Major = value >= 0 ? value : throw new ArgumentException(
            "Major version must be cero or greater.")
            .WithData(value);
    }
    int _Major = 0;

    /// <summary>
    /// The minor version carried by this instance.
    /// </summary>
    public int Minor
    {
        get => _Minor;
        init => _Minor = value >= 0 ? value : throw new ArgumentException(
            "Minor version must be cero or greater.")
            .WithData(value);
    }
    int _Minor = 0;

    /// <summary>
    /// The patch version carried by this instance.
    /// </summary>
    public int Patch
    {
        get => _Patch;
        init => _Patch = value >= 0 ? value : throw new ArgumentException(
            "Patch version must be cero or greater.")
            .WithData(value);
    }
    int _Patch = 0;

    /// <summary>
    /// The pre-release version carried by this instance.
    /// </summary>
    public SemanticRelease PreRelease
    {
        get => _PreRelease;
        init => _PreRelease = value.ThrowWhenNull();
    }
    SemanticRelease _PreRelease = SemanticRelease.Empty;

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    public bool IsEmpty => Major == 0 && Minor == 0 && Patch == 0 && PreRelease.IsEmpty;

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance with its major value increased, and its minor, patch and
    /// pre-release ones cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMajor() => new(Major + 1, 0, 0, "");

    /// <summary>
    /// Returns a new instance with its minor value increased, and its patch and pre-release
    /// ones cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMinor() => new(Major, Minor + 1, 0, "");

    /// <summary>
    /// Returns a new instance with its patch value increased and its pre-release one cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreasePatch() => new(Major, Minor, Patch + 1, "");

    /// <summary>
    /// Returns a new instance with its pre-release value increased and its metadata cleared.
    /// If the original pre-release value was empty, then the given template one is used as
    /// the value to increase.
    /// </summary>
    /// <param name="templateIfEmpty"></param>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease(string templateIfEmpty = "")
    {
        var temp = PreRelease.Increase(templateIfEmpty);
        return new(Major, Minor, Patch, temp);
    }

    // ------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the
    /// first one precedes, follows, or occurs in the same position in sort order as the
    /// second one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(SemanticVersion? x, SemanticVersion? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;

        if (x.Major != y.Major) return x.Major.CompareTo(y.Major);
        if (x.Minor != y.Minor) return x.Minor.CompareTo(y.Minor);
        if (x.Patch != y.Patch) return x.Patch.CompareTo(y.Patch);
        return x.PreRelease.CompareTo(y.PreRelease);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SemanticVersion? other) => Compare(this, other);

    public static bool operator >(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) <= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(SemanticVersion? other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/> This method only takes into consideration the version value carried by
    /// this instance, and not its metadata one.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, PreRelease);
}