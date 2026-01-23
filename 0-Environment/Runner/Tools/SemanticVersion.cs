namespace Runner;

// ========================================================
/// <summary>
/// Represents a semantic specification.
/// <br/> See: https://semver.org
/// <para>
/// Semantic versions are of the form 'X.Y.Z' where X is the major version, Y is the minor one, and
/// Z is the patch specification. Each part is a not-negative integer that must not contain leading
/// zeroes.
/// <br/>  Pre-release specifications are denoted by appending a hypen and then dot-separated parts,
/// using ASCII alphanumeric characters only [0-9, A-Z, a-z], that must not be empty. Hypens are
/// allowed only once. Numeric identifiers must not include leading zeroes.
/// <br/> Metadata is then denoted by appending a plus sign '+' following the patch or pre-release
/// version and a series of dot-separated identifiers with the same syntax.
/// <br/> Precendence is calculated by firstly comparing the major, minor and patch specifications,
/// in order, until a difference is found. If not, then each dot-separated metadata identifier from
/// left to righ until a difference is found, either numeracally or in ASCII sort-order. Numeric
/// identifiers always have lower precendence than alphanumeric ones. When comparing precedence,
/// metadata is ignored, but used for equality purposes.
/// </para>
/// </summary>
public record SemanticVersion : IComparable<SemanticVersion>
{
    /// <summary>
    /// A common shared empty instance.
    /// </summary>
    public static SemanticVersion Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SemanticVersion() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="patch"></param>
    public SemanticVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="patch"></param>
    /// <param name="prerelease"></param>
    public SemanticVersion(int major, int minor, int patch, SemanticPreRelease prerelease)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = prerelease;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public SemanticVersion(string value)
    {
        value.ThrowWhenNull(); if (value.Length == 0) return;
        value.NotNullNotEmpty(true);

        string? metadata = null;

        if (value is not null) Major = GetValue(value, out value);
        if (value is not null) Minor = GetValue(value, out value);
        if (value is not null) Patch = GetValue(value, out _);
        if (metadata is not null) PreRelease = metadata;

        /// <summary>
        /// Used to get the value of the given major, minor o patch part.
        /// </summary>
        int GetValue(string value, out string newvalue)
        {
            newvalue = null!;

            var index = value.IndexOf('-'); if (index >= 0)
            {
                metadata = value[index..];
                value = value[..index];
            }

            index = value.IndexOf('+'); if (index >= 0)
            {
                metadata = value[index..];
                value = value[..index];
            }

            index = value.IndexOf('.');
            if (index >= 0)
            {
                newvalue = value[(index + 1)..];
                value = value[..index];
            }

            if (value.Length == 0) throw new EmptyException("Part cannot be empty.").WithData(value);
            if (!value.All(char.IsAsciiDigit)) throw new ArgumentException("Part must contain digits only.").WithData(value);
            if (value.Length > 1 && value[0] == '0') throw new ArgumentException("Part must not have leading zeroes.").WithData(value);

            return int.TryParse(value, out var r) ? r : 0;
        }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public SemanticVersion(SemanticVersion source)
    {
        source.ThrowWhenNull();
        _Major = source.Major;
        _Minor = source.Minor;
        _Patch = source.Patch;
        _PreRelease = source.PreRelease;
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(
        SemanticVersion value) => value.ThrowWhenNull().ToString();

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticVersion(string value) => new(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var str = $"{Major}.{Minor}.{Patch}";
        if (!PreRelease.IsEmpty) str += PreRelease.ToString();
        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The zero or positive major version carried by this instance.
    /// </summary>
    public int Major
    {
        get => _Major;
        init => _Major = value >= 0 ? value : throw new ArgumentException(
            "Major version must be zero or greater.")
            .WithData(value);
    }
    int _Major;

    /// <summary>
    /// The zero or positive minor version carried by this instance.
    /// </summary>
    public int Minor
    {
        get => _Minor;
        init => _Minor = value >= 0 ? value : throw new ArgumentException(
            "Minor version must be zero or greater.")
            .WithData(value);
    }
    int _Minor;

    /// <summary>
    /// The zero or positive patch version carried by this instance.
    /// </summary>
    public int Patch
    {
        get => _Patch;
        init => _Patch = value >= 0 ? value : throw new ArgumentException(
            "Patch version must be zero or greater.")
            .WithData(value);
    }
    int _Patch;

    /// <summary>
    /// The pre-release version carried by this instance, or an empty one.
    /// </summary>
    public SemanticPreRelease PreRelease
    {
        get => _PreRelease;
        init => _PreRelease = value.ThrowWhenNull();
    }
    SemanticPreRelease _PreRelease = SemanticPreRelease.Empty;

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    public bool IsEmpty => Major == 0 && Minor == 0 && Patch == 0 && PreRelease.IsEmpty;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IComparable.CompareTo(object?)"/>
    /// </summary>
    /// <param name="x"><inheritdoc cref="IComparable.CompareTo(object?)"/></param>
    /// <param name="y"><inheritdoc cref="IComparable.CompareTo(object?)"/></param>
    /// <returns><inheritdoc cref="IComparable.CompareTo(object?)"/></returns>
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
    /// <param name="other"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public int CompareTo(SemanticVersion? other) => Compare(this, other);

    public static bool operator >(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public virtual bool Equals(SemanticVersion? other)
    {
        if (other is null) return false;
        if (Major != other.Major) return false;
        if (Minor != other.Minor) return false;
        if (Patch != other.Patch) return false;
        if (!PreRelease.Equals(other.PreRelease)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, PreRelease);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original major version has been increased by one unit.
    /// <br/> The minor, patch and prerelease elements are cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMajor() => new(Major + 1, 0, 0, "");

    /// <summary>
    /// Returns a new instance where the original minor version has been increased by one unit.
    /// <br/> The patch and prerelease elements are cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMinor() => new(Major, Minor + 1, 0, "");

    /// <summary>
    /// Returns a new instance where the original patch version has been increased by one unit.
    /// <br/> Theprerelease element is cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreasePatch() => new(Major, Minor, Patch + 1, "");

    /// <summary>
    /// Returns a new instance where the original pre-release value is increased, provided it is
    /// not an empty one, and that it can be treated as a trailing numeric one. If it was an empty
    /// one, then the optional template (if not null) is used.
    /// <br/> The existing build metadata, if any, is erased. You can add it later if needed.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease(
        string? template = null) => IncreasePreRelease(out _, template);

    /// <summary>
    /// Returns a new instance where the original pre-release value is increased, provided it is
    /// not an empty one, and that it can be treated as a trailing numeric one. If it was an empty
    /// one, then the optional template (if not null) is used.
    /// <br/> If the value cannot be increased the out argument is set to <c>false</c>.
    /// <br/> The existing build metadata, if any, is erased. You can add it later if needed.
    /// </summary>
    /// <param name="increased"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease(out bool increased, string? template = null)
    {
        var temp = PreRelease.Increase(out increased, template);
        return new(Major, Minor, Patch, temp);
    }
}