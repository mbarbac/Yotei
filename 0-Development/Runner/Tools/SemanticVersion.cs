using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;

namespace Runner;

// ========================================================
/// <summary>
/// Represents a semantic version specification as defined at 'https://semver.org'.
/// <br/> A normal version takes the form 'X.Y.Z' where X is the major version, Y is the minor
/// one, and Z is the patch specification. Each part is a not-negative integer that MUST NOT
/// contain leading zeroes.
/// <br/> A pre-release version is denoted by appending a hypen and a series of dot separated
/// identifiers, that use ASCII alphanumeric characters [0 - 9, A - Z, a - z] only, and MUST NOT
/// be empty. Hyphens are only allowed once as the separator. Numeric identifiers MUST not
/// include leading zeroes.
/// <br/> Metadata is denoted by appending a plus sign and a series of dot-separated identifiers
/// immediately following the patch or pre-release version. Identifiers must comprise ASCII
/// alphanumerics [0 - 9, A - Z, a - z] characters only, and MUST NOT be empty.
/// <br/> Precedence is calculated firstly by comparing the major, minor and patch values, in
/// that order. Pre-release versions always have lower precedence than their associated normal
/// ones, and is calculated by comparing each dot-separated identifier from left to right until
/// a difference is found either numerically or is ASCII sort order. Numeric identifiers always
/// have lower precendence than alphanumeric ones. When comparing precedence build metadata is
/// ignored, but used for equality purposes.
/// </summary>
public partial record SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
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
    public SemanticVersion(int major, int minor, int patch, SemanticRelease prerelease)
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
        value.NotNullNotEmpty();

        string? metadata = null;

        if (value is not null) Major = GetValue(value, out value);
        if (value is not null) Minor = GetValue(value, out value);
        if (value is not null) Patch = GetValue(value, out value);
        if (metadata is not null) PreRelease = metadata;

        // Gets the value of the given major, minor or patch part...
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

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = $"{Major}.{Minor}";
        if (Patch > 0) str += $".{Patch}";
        if (!PreRelease.IsEmpty) str += PreRelease.ToString(hyphen: true);
        return str;
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(SemanticVersion value) => value.ThrowWhenNull().ToString();

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticVersion(string value) => new(value);

    // ----------------------------------------------------

    /// <summary>
    /// The zero or positive major version carried by this instance.
    /// </summary>
    public int Major
    {
        get => _Major;
        init
        {
            _Major = value >= 0 ? value : throw new ArgumentException(
                "Major version must be zero or greater.")
                .WithData(value);
        }
    }
    int _Major;

    /// <summary>
    /// The zero or positive minor version carried by this instance.
    /// </summary>
    public int Minor
    {
        get => _Minor;
        init
        {
            _Minor = value >= 0 ? value : throw new ArgumentException(
                "Minor version must be zero or greater.")
                .WithData(value);
        }
    }
    int _Minor;

    /// <summary>
    /// The zero or positive patch version carried by this instance.
    /// </summary>
    public int Patch
    {
        get => _Patch;
        init
        {
            _Patch = value >= 0 ? value : throw new ArgumentException(
                "Patch version must be zero or greater.")
                .WithData(value);
        }
    }
    int _Patch;

    /// <summary>
    /// The pre-release version carried by this instance, or an empty one.
    /// </summary>
    public SemanticRelease PreRelease
    {
        get => _PreRelease;
        init => SetRelease(value);
    }
    SemanticRelease _PreRelease = SemanticRelease.Empty;
    void SetRelease(SemanticRelease prerelease) => _PreRelease = prerelease.ThrowWhenNull();

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    public bool IsEmpty => Major == 0 && Minor == 0 && Patch == 0 && PreRelease.IsEmpty;

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position as the second one. Comparison is
    /// performed by comparing in order the values of the major, minor and patch parts, and then,
    /// the prerelease one, but not taking into consideration its build metadata.
    /// 
    /// the parts in the regular value, but does not take into consideration
    /// the build metadata.
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
    /// <inheritdoc/> Comparison is performed by comparing in order the values of the major,
    /// minor and patch parts, and then, the prerelease one, but not taking into consideration
    /// its build metadata.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SemanticVersion? other) => Compare(this, other);

    public static bool operator >(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticVersion? x, SemanticVersion? y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> Comparison is performed by comparing the parts in the regular value, and
    /// by comparing the build metadata as well.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(SemanticVersion? other)
    {
        return
            Compare(this, other) == 0 &&
            other is not null &&
            PreRelease.Equals(other.PreRelease);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, PreRelease);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original major version has been increased by one unit.
    /// The minor and patch values, and the build metadata have been cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMajor() => new(Major + 1, 0, 0, "");

    /// <summary>
    /// Returns a new instance where the original minor version has been increased by one unit.
    /// The patch value, and the build metadata have been cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreaseMinor() => new(Major, Minor + 1, 0, "");

    /// <summary>
    /// Returns a new instance where the original patch version has been increased by one unit.
    /// The build metadata has been cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreasePatch() => new(Major, Minor, Patch + 1, "");

    /// <summary>
    /// Returns a new instance where the original value of the prerelease part has been increased,
    /// provided that such is not empty and that it can be treated as a trailing numeric one. In
    /// any case, the build metadata part is always cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease() => IncreasePreRelease(out _);

    /// <summary>
    /// Returns a new instance where the original value of the prerelease part has been increased,
    /// provided that such is not empty and that it can be treated as a trailing numeric one. If
    /// so, the out argument is set to <c>true</c>, or otherwise set to false. In any case, the
    /// build metadata part is always cleared.
    /// </summary>
    /// <param name="increased"></param>
    /// <returns></returns>
    public SemanticVersion IncreasePreRelease(out bool increased)
    {
        var temp = PreRelease.Increase(out increased);
        return new(Major, Minor, Patch, temp);
    }
}