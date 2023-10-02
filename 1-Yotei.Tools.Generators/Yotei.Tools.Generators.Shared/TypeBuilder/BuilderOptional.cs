namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents an optional init/set additional argument with the '[+|-][*|member][=@][!]' format
/// where:
/// <br/>>- [+|-]: determines if it is an include or exclude specification.
/// <br/>>- [*|member]: if '*', the specification affects to all remaining members, and that any
/// previous ones are erased. Otherwise, the name of the member to use from the set of remaining
/// ones.
/// <br/>>- [=@]: if used then the name of the value for the enforced member, if
/// any, will be used.
/// <br/>>- [!]: If used, then a clone of the value will be used instead.
/// </summary>
internal class BuilderOptional
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="spec"></param>
    public BuilderOptional(string spec)
    {
        spec = spec.NotNullNotEmpty(nameof(spec));

        switch (spec[0])
        {
            case '+': IsInclude = true; break;
            case '-': IsExclude = true; break;

            default:
                throw new ArgumentException(
                "Optional specification must beguin with '+' or '-'.")
                .WithData(spec, nameof(spec));
        }
        spec = spec.Substring(1).NotNullNotEmpty(nameof(spec));

        if (spec.EndsWith("!"))
        {
            if (IsExclude) throw new ArgumentException(
                "No '!' allowed for exclude specification.")
                .WithData(spec, nameof(spec));

            UseClone = true;
            spec = spec.Substring(0, spec.Length - 1);
        }

        var n = spec.IndexOf('=');
        Member = n < 0 ? spec : spec.Substring(0, n);

        if (IsMemberAsterisk && UseClone) throw new ArgumentException(
            "No '!' allowed after an asterisk.")
            .WithData(spec, nameof(spec));

        if (n > 0)
        {
            if (IsMemberAsterisk) throw new ArgumentException(
                "No '=...' allowed after an asterisk.")
                .WithData(spec, nameof(spec));

            if (IsExclude) throw new ArgumentException(
                "No '=...' allowed for exclude specification.")
                .WithData(spec, nameof(spec));

            var str = spec.Substring(n + 1).NotNullNotEmpty(nameof(IsMemberEnforced));
            if (str == "@") IsMemberEnforced = true;

            else throw new ArgumentException(
                "Only '@' allowed after an equal sign.")
                .WithData(spec, nameof(spec));
        }
    }

    /// <summary>
    /// <inheritdoc/> '[+|-][*|member][=@][!]'
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder($"{(IsExclude ? "-" : "+")}{Member}");
        if (IsMemberEnforced) sb.Append("=@");
        if (UseClone) sb.Append("!");
        return sb.ToString();
    }

    /// <summary>
    /// Determines if this is an exclude specification, or not.
    /// </summary>
    public bool IsExclude
    {
        get => _IsExclude;
        set => _IsExclude = value;
    }
    bool _IsExclude = false;

    /// <summary>
    /// Determines if this is an exclude-all specification, or not.
    /// </summary>
    public bool IsExcludeAll => IsExclude && IsMemberAsterisk;

    /// <summary>
    /// Determines if this is an include specification, or not.
    /// </summary>
    public bool IsInclude
    {
        get => !IsExclude;
        set => IsExclude = !value;
    }

    /// <summary>
    /// Determines if this is an include-all specification, or not.
    /// </summary>
    public bool IsIncludeAll => IsInclude && IsMemberAsterisk;

    /// <summary>
    /// The actual name of the init/set member, or '*' to indicate that all ones shall be used.
    /// If so, then no more modifiers can be specified.
    /// </summary>
    public string Member
    {
        get => _Member;
        set => _Member = value.NotNullNotEmpty(nameof(Member));
    }
    string _Member = default!;

    /// <summary>
    /// Determines if the name is an '*' specification, or not.
    /// </summary>
    public bool IsMemberAsterisk => Member == "*";

    /// <summary>
    /// Returns the effective member name.
    /// </summary>
    /// <returns></returns>
    public string GetMember() => IsMemberAsterisk
        ? throw new InvalidOperationException("Member is asterisk.")
        : Member;

    /// <summary>
    /// Determines if this must be associated with the name of the variable from which to obtain
    /// the value, or not. If 'false', it can be still associated with that enforced member if
    /// ther is a match in the member name.
    /// </summary>
    public bool IsMemberEnforced { get; set; }

    /// <summary>
    /// Whether to use a clone of the source value, or not.
    /// </summary>
    public bool UseClone { get; set; }

    /// <summary>
    /// Gets the code that represents the value to use with the init/set member, taken into
    /// consideration if a clone shall be obtained, or not.
    /// </summary>
    /// <param name="enforcedMember"></param>
    /// <returns></returns>
    public string GetValue(EnforcedMember? enforcedMember)
    {
        var value = TheValue(); return UseClone
            ? $"({value} is null) ? null : {value}.Clone()"
            : value;

        string TheValue()
        {
            if (IsMemberEnforced &&
                enforcedMember != null) return enforcedMember.ValueName;

            return IsMemberAsterisk
                ? throw new InvalidOperationException("Member is asterisk.")
                : Member;
        }
    }
}