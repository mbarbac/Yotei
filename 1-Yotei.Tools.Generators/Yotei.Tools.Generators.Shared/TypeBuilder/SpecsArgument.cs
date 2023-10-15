namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a regular argument with the '[name][=@|member[!]]' format where:
/// <br/>- [name]: the actual name of the method argument, or '*' to indicate all arguments.
/// <br/>- [=@|member]: the source from which to obtain the value of the method argument. If not
/// used, then the name of a corresponding member will be used instead. If '@' then the name of
/// the enforced member, if any, will be used. If 'member', the actual name of the member from
/// which to obtain the value.
/// <br/>- [!]: if used then a clone of the value will be used instead of the value itself.
/// </summary>
internal class SpecsArgument
{
    /// <summary>
    /// Initializes a new instance with the format: '[name][=@|member|this[!]]'.
    /// </summary>
    /// <param name="specs"></param>
    public SpecsArgument(string specs)
    {
        specs = specs.NotNullNotEmpty(nameof(specs));

        var n = specs.IndexOf('=');
        Name = n < 0 ? specs : specs.Substring(0, n);
        Name = Name.NotNullNotEmpty(nameof(Name));

        if (Name[0] == '*' && Name.Length > 1) throw new ArgumentException(
            "Invalid name specification.")
            .WithData(specs, nameof(specs));

        if (Name.EndsWith("!")) throw new ArgumentException(
            "No '!' can be applied to the argument name.")
            .WithData(specs, nameof(specs));

        Member = n < 0 ? null : specs.Substring(n + 1).NotNullNotEmpty(nameof(Member));
        if (Member != null)
        {
            if (Member.EndsWith("!"))
            {
                UseClone = true;
                Member = Member.Substring(0, Member.Length - 1).NullWhenEmpty();
            }

            if (Member == null) throw new EmptyException(
                "Empty source specification.")
                .WithData(specs, nameof(specs));

            if (IsNameAsterisk) throw new ArgumentException(
                "No '*' specification can be applied to an asterisk name.")
                .WithData(specs, nameof(specs));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder(Name);
        if (Member != null) sb.Append($"={Member}");
        if (UseClone) sb.Append('!');
        return sb.ToString();
    }

    /// <summary>
    /// The name of the method argument, or '*' to indicate that all arguments shall be used. If
    /// so, then no more modifiers are allowed.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Determines if the name is an asterisk specification, or not.
    /// </summary>
    public bool IsNameAsterisk => Name == "*";

    /// <summary>
    /// The source from which to obtain the value of the method argument. If it is 'null', then
    /// the name of a corresponding member will be found. If it is '@' then the name of the given
    /// enforced member will be used, if any. Otherwise, the name of an actual member.
    /// </summary>
    public string? Member { get; }

    /// <summary>
    /// Determines if the source is the name of a given enforced member, or not.
    /// </summary>
    public bool IsEnforcedMember => Member == "@";

    /// <summary>
    /// Determines if a clone of the source value should be used instead of the value itself.
    /// </summary>
    public bool UseClone { get; }
}