#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a semantic versioning specification. See: https://semver.org
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
record SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    /// <summary>
    /// An empty shared instance.
    /// </summary>
    public static SemanticVersion Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the SemanticVersion class.
    /// </summary>
    public SemanticVersion()
    {
        Major = Minor = Patch = 0;
        PreRelease = SemanticPreRelease.Empty;
    }

    /// <summary>
    /// Initializes a new instance with the given values.
    /// </summary>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="patch"></param>
    public SemanticVersion(int major, int minor, int patch = 0)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = SemanticPreRelease.Empty;
    }

    /// <summary>
    /// Initializes a new instance with the given values.
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
    /// Initializes a new instance with the values obtained from the given one.
    /// </summary>
    /// <param name="value"></param>
    public SemanticVersion(string value) : this()
    {
        // Empty is allowed, but not null or blanks-only...
        value.ThrowWhenNull();
        if (value.Length == 0) return;
        value = value.NotNullNotEmpty(trim: true);

        // Must start with a major version...
        if (!char.IsDigit(value[0])) throw new ArgumentException(
            "Specification must start with a digit.")
            .WithData(value);

        // Parsing parts...
        string meta = "";
        Major = GetValue(value, out value);
        if (value.Length > 0) Minor = GetValue(value, out value);
        if (value.Length > 0) Patch = GetValue(value, out value);
        if (meta.Length > 0) PreRelease = meta;

        /// <summary>
        /// Extracts the current int part, and produces the remaining value and metadata.
        /// </summary>
        int GetValue(string value, out string remain)
        {
            remain = "";

            // Extracts pre-release part...
            var index = value.IndexOf('-'); if (index >= 0)
            {
                meta = value[index..];
                value = value[..index]; if (value.Length == 0) return 0;
            }

            // Extracts pre-release part...
            index = value.IndexOf('+'); if (index >= 0)
            {
                meta = value[index..];
                value = value[..index]; if (value.Length == 0) return 0;
            }

            // Extracts next part...
            index = value.IndexOf('.'); if (index >= 0)
            {
                remain = value[(index + 1)..];
                value = value[..index];
            }

            // Computing value...
            value = value.NotNullNotEmpty(trim: true);

            if (!value.All(char.IsDigit)) throw new ArgumentException(
                "Part must be a digits-only one.")
                .WithData(value);

            if (value.Length > 1 && value[0] == '0') throw new ArgumentException(
                "Part must not have leading zeroes")
                .WithData(value);

            return int.TryParse(value, out var r) ? r : 0;
        }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    public SemanticVersion(SemanticVersion other)
    {
        other.ThrowWhenNull();

        Major = other.Major;
        Minor = other.Minor;
        Patch = other.Patch;
        PreRelease = other.PreRelease;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToString(false);

    /// <summary>
    /// Returns a string that represents this object, with an enforced <see cref="Patch"/> part
    /// or not as requested.
    /// </summary>
    /// <param name="patch"></param>
    /// <returns></returns>
    public string ToString(bool patch)
    {
        var str = $"{Major}.{Minor}";
        if (Patch > 0 || patch) str += $".{Patch}";
        if (!PreRelease.IsEmpty) str += PreRelease.ToString();
        return str;
    }

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    /// <summary>
    /// The zero or positive major version.
    /// </summary>
    public int Major
    {
        get;
        init => field = value >= 0 ? value :
            throw new ArgumentException("Invalid major version.").WithData(value);
    }

    /// <summary>
    /// The zero or positive minor version.
    /// </summary>
    public int Minor
    {
        get;
        init => field = value >= 0 ? value :
            throw new ArgumentException("Invalid minor version.").WithData(value);
    }

    /// <summary>
    /// The zero or positive patch version.
    /// </summary>
    public int Patch
    {
        get;
        init => field = value >= 0 ? value :
            throw new ArgumentException("Invalid patch version.").WithData(value);
    }

    /// <summary>
    /// The pre-release version carried by this instance, or an empty one.
    /// </summary>
    public SemanticPreRelease PreRelease
    {
        get;
        init => field = value.ThrowWhenNull();
    }

    /// <summary>
    /// Determines if this instance is an empty one or not.
    /// </summary>
    public bool IsEmpty => Major == 0 && Minor == 0 && Patch == 0 && PreRelease.IsEmpty;

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns a value that determines if the first one
    /// precedes, follows, or is equal to the other given one.
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

        if (!x.PreRelease.IsEmpty && !y.PreRelease.IsEmpty)
            return x.PreRelease.CompareTo(y.PreRelease);

        if (!x.PreRelease.IsEmpty) return -1;
        if (!y.PreRelease.IsEmpty) return +1;
        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Note that this method DOES NOT take into consideration the metadata parts.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SemanticVersion? other) => Compare(this, other);

    public static bool operator >(SemanticVersion x, SemanticVersion y) => Compare(x, y) > 0;

    public static bool operator <(SemanticVersion x, SemanticVersion y) => Compare(x, y) < 0;

    public static bool operator >=(SemanticVersion x, SemanticVersion y) => Compare(x, y) >= 0;

    public static bool operator <=(SemanticVersion x, SemanticVersion y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Note that this method DOES NOT take into consideration the metadata part.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(SemanticVersion? other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Note that this method DOES NOT take into consideration the metadata part.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, PreRelease);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the major version has been increased, and the minor, patch
    /// and pre-release ones cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMajor() => new(Major + 1, 0, 0, "");

    /// <summary>
    /// Returns a new instance where the minor version has been increased, and the patch and
    /// pre-release ones cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMinor() => new(Major, Minor + 1, 0, "");

    /// <summary>
    /// Returns a new instance where the patch version has been increased, and the pre-release
    /// one cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreasePatch() => new(Major, Minor, Patch + 1, "");

    /// <summary>
    /// Returns a new instance where the release value has been increased, if possible, setting
    /// the value of the out argument accordingly. Only the last numeric part, or the last one
    /// containing a trailing numeric trunk, is increased. Alphanumeric parts are skipped and
    /// not increased.
    /// </summary>
    /// <param name="increased"></param>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease(out bool increased)
    {
        increased = false;
        if (IsEmpty || (Major == 0 && Minor == 0 && Patch == 0)) return Empty;

        var temp = PreRelease.Increase(out increased);
        return increased
            ? new(Major, Minor, Patch, temp)
            : this;
    }
}