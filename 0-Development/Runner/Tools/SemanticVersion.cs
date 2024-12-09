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
public partial record SemanticVersion
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

        throw null;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = $"{Major}.{Minor}";
        if (Patch > 0) str += $".{Patch}";
        if (!PreRelease.IsEmpty) str += PreRelease.ToString(hypen: true);
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
}