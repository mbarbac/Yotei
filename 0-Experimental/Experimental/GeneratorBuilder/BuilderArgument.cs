namespace Experimental;

// ========================================================
/// <summary>
/// Represents a regular builder argument with the '[name][=@|member][!]' format, where:
/// <br/>- [name]: the actual name of the builder argument, or '*' to indicate that all arguments
/// shall be taken into consideration.
/// <br/>- [=@|member]: the source from which to obtain the value of that argument. If it is not
/// used, the name of a matching member will be used. If '=@', then the name of the enforced member
/// (if any) will be used instead. Otherwise, the actual name of the member that becomes the source
/// of that value.
/// <br/>>- [!]: If used, then a clone of the value will be used instead.
/// </summary>
internal class BuilderArgument
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="spec"></param>
    public BuilderArgument(string spec)
    {
        spec = spec.NotNullNotEmpty();

        if (spec.EndsWith('!'))
        {
            UseClone = true;
            spec = spec.Substring(0, spec.Length - 1);
        }

        var n = spec.IndexOf('=');
        Name = n < 0 ? spec : spec.Substring(0, n);

        if (IsNameAsterisk && UseClone) throw new ArgumentException(
            "No '!' allowed after an asterisk.")
            .WithData(spec);

        if (n > 0)
        {
            if (IsNameAsterisk) throw new ArgumentException(
                "No '=...' allowed after an asterisk.")
                .WithData(spec);

            Member = spec.Substring(n + 1);
        }
    }

    /// <summary>
    /// <inheritdoc/> '[name][=@|member][!]'
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder(Name);
        if (Member != null) sb.Append($"={Member}");
        if (UseClone) sb.Append("!");
        return sb.ToString();
    }

    /// <summary>
    /// The actual name of the builder argument, or '*' to indicate that all arguments shall be
    /// used. If so, then no more modifiers can be specified.
    /// </summary>
    public string Name
    {
        get => _Name;
        set => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;

    /// <summary>
    /// Determines if the name is an '*' specification, or not.
    /// </summary>
    public bool IsNameAsterisk => Name == "*";

    /// <summary>
    /// The source from which to obtain the value of the builder argument. If null, then a member
    /// with a matching name will be used. If '@', then the name of the variable from which to
    /// obtaine the value of the enforced member will be used, if any. Otherwise, the name of the
    /// matching member.
    /// </summary>
    public string? Member
    {
        get => _Member;
        set => _Member = value?.NotNullNotEmpty();
    }
    string? _Member = null;

    /// <summary>
    /// Determines if this must be associated with the name of the variable from which to obtain
    /// the value, or not. If 'false', it can be still associated with that enforced member if
    /// ther is a match in the member name.
    /// </summary>
    public bool IsMemberEnforced => Member == "@";

    /// <summary>
    /// Returns the effective member name.
    /// </summary>
    /// <param name="enforcedMember"></param>
    /// <returns></returns>
    public string GetMember(EnforcedMember? enforcedMember)
    {
        if (IsMemberEnforced && enforcedMember != null) return enforcedMember.Name;
        if (Member != null && !IsMemberEnforced) return Member;

        return IsNameAsterisk
            ? throw new InvalidOperationException("Name is asterisk.")
            : Name;
    }

    /// <summary>
    /// Whether to use a clone of the source value, or not.
    /// </summary>
    public bool UseClone { get; set; }

    /// <summary>
    /// Gets the code that represents the value to use with the builder argument, taken into
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
            if (IsMemberEnforced && enforcedMember != null) return enforcedMember.ValueName;
            if (Member != null && !IsMemberEnforced) return Member;

            return IsNameAsterisk
                ? throw new InvalidOperationException("Name is asterisk.")
                : Name;
        }
    }
}