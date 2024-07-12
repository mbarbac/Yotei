namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="SketchExtensions"/> methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// The options to use when the element is a <see cref="Type"/>.
    /// </summary>
    public EasyTypeOptions TypeOptions
    {
        get => _TypeOptions;
        init => _TypeOptions = value.ThrowWhenNull();
    }
    EasyTypeOptions _TypeOptions = default!;

    /// <summary>
    /// The options to use when the element is a reflection one.
    /// </summary>
    public EasyMemberOptions MemberOptions
    {
        get => _MemberOptions;
        init => _MemberOptions = value.ThrowWhenNull();
    }
    EasyMemberOptions _MemberOptions = default!;

    // ----------------------------------------------------

    /// <summary>
    /// The string used to represent null values.
    /// </summary>
    public string NullStr
    {
        get => _NullStr;
        init => _NullStr = value.ThrowWhenNull();
    }
    string _NullStr = default!;

    /// <summary>
    /// If not null, the options to use to preceed values with their types between rounded
    /// brackets.
    /// </summary>
    public EasyTypeOptions? UseSourceType { get; init; }

    /// <summary>
    /// If not null, the format specification to use.
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// If not null, the format provider to use.
    /// </summary>
    public IFormatProvider? Provider { get; init; }

    /// <summary>
    /// If true uses the source's shape, if such would be needed.
    /// </summary>
    public bool UseShape { get; init; }

    /// <summary>
    /// If true enforces including private members for objects' shapes.
    /// </summary>
    public bool UsePrivateMembers { get; init; }

    /// <summary>
    /// If true enforces including static members for objects' shapes.
    /// </summary>
    public bool UseStaticMembers { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions()
    {
        TypeOptions = EasyTypeOptions.Empty;
        MemberOptions = EasyMemberOptions.Empty;
        NullStr = string.Empty;
        UseSourceType = null;
        Format = null;
        Provider = null;
        UseShape = false;
        UsePrivateMembers = false;
        UseStaticMembers = false;
    }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SketchOptions Empty { get; } = new();

    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static SketchOptions Default { get; } = new()
    {
        TypeOptions = EasyTypeOptions.Default,
        MemberOptions = EasyMemberOptions.Default,
        NullStr = "NULL",
        UseShape = true,
    };

    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static SketchOptions Full { get; } = new()
    {
        TypeOptions = EasyTypeOptions.Full,
        MemberOptions = EasyMemberOptions.Full,
        NullStr = "NULL",
        UseSourceType = EasyTypeOptions.Full,
        UseShape = true,
        UsePrivateMembers = true,
        UseStaticMembers = true,
    };
}