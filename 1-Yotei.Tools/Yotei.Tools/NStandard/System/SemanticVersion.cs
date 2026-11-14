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
    public SemanticVersion(string value) => throw null;

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
    public override string ToString()
    {
        var str = $"{Major}.{Minor}";
        if (Patch > 0) str += $".{Patch}";
        if (!PreRelease.IsEmpty) str += PreRelease.ToString();
        return str;
    }

    /// <summary>
    /// Returns a string that represents this object with its three parts, despite the patch one
    /// is zero or not.
    /// </summary>
    /// <returns></returns>
    public string To3String()
    {
        var str = $"{Major}.{Minor}.{Patch}";
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

        return x.PreRelease.CompareTo(y.PreRelease);
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
    /// Returns a new instance where the pre-released version has been increased.
    /// <br/>- If the original value was empty, then the given template is used as the returned
    /// value, unless it is a null one itself. If this happens, then no increase is performed and
    /// the <paramref name="increased"/> value is set to <c>false</c>. In all other cases the
    /// value is increased, and the out argument is set to <c>true</c>.
    /// <br/>- Any existing metadata is lost.
    /// </summary>
    /// <param name="increased"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease(out bool increased, string? template = null)
    {
        var value = PreRelease.Increase(out increased, template);
        return new(Major, Minor, Patch, value);
    }
}