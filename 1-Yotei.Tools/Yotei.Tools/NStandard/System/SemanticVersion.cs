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
record SemanticVersion
{
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
}