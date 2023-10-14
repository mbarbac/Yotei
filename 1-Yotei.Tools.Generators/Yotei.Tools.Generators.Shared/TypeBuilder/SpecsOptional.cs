using System.Net.Security;

namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents an optional init/set argument with the '[+|-][*|member[=@][!]' format where:
/// <br/>- [-*]: all previous specifications will be deleted.
/// <br/>- [+*]: all type members not used in the corresponding method will be added.
/// <br/>- [-member]: deletes the member from the list of members to be used.
/// <br/>- [+member]: adds the member to the list of members to be used, and then:
/// <br/>- [...=@]: obtains the value of the type member from the enforced one.
/// <br/>- [...!]: use a clone of the value instead of the value itself.
/// </summary>
internal class SpecsOptional
{
    /// <summary>
    /// Initializes a new instance with the format: '[+|-][*|member[=@][!]]'.
    /// </summary>
    /// <param name="specs"></param>
    public SpecsOptional(string specs)
    {
        specs = specs.NotNullNotEmpty(nameof(specs));

        switch (specs[0])
        {
            case '+': IsInclude = true; break;
            case '-': IsExclude = true; break;
            default:
                throw new ArgumentException(
                "Optional specification must begin with '+' or '-'.")
                .WithData(specs, nameof(specs));
        }
        specs = specs.Substring(1).NotNullNotEmpty(nameof(specs));

        var n = specs.IndexOf('=');
        Member = n < 0 ? specs : specs.Substring(0, n);
        Member = Member.NotNullNotEmpty(nameof(Member));

        if (Member[0] == '*' && Member.Length > 1) throw new ArgumentException(
            "Invalid name specification.")
            .WithData(specs, nameof(specs));

        if (Member.EndsWith("!"))
        {
            UseClone = true;
            Member = Member.Substring(0, Member.Length - 1).NotNullNotEmpty(nameof(Member));
        }

        if (UseClone && IsMemberAsterisk) throw new ArgumentException(
            "No '!' can be applied to an asterisk.")
            .WithData(specs, nameof(specs));

        if (n > 0)
        {
            if (IsMemberAsterisk) throw new ArgumentException(
                "No '=' allowed after an asterisk.")
                .WithData(specs, nameof(specs));

            var s = specs.Substring(n + 1).NotNullNotEmpty(nameof(IsMemberEnforced));
            if (s == "@")
            {
                IsMemberEnforced = true;

                if (UseClone) throw new ArgumentException(
                    "No '!' can be applied to an enforced member.")
                    .WithData(specs, nameof(specs));
            }
            else throw new ArgumentException(
                "Only '@' is allowed after the '=' symbol.")
                .WithData(specs, nameof(specs));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(IsExclude ? "-" : "+");
        sb.Append(Member);
        if (IsMemberEnforced) sb.Append("=@");
        if (UseClone) sb.Append('!');
        return sb.ToString();
    }

    /// <summary>
    /// Determines if this is an exclude specification or not.
    /// </summary>
    public bool IsExclude { get; }

    /// <summary>
    /// Determines if this is an exclude-all specification or not.
    /// </summary>
    public bool IsExcludeAll => IsExclude && IsMemberAsterisk;

    /// <summary>
    /// Determines if this is an include specification or not.
    /// </summary>
    public bool IsInclude { get; }

    /// <summary>
    /// Determines if this is an include-all specification or not.
    /// </summary>
    public bool IsIncludeAll => IsInclude && IsMemberAsterisk;

    /// <summary>
    /// The name of the member, or '*' to indicate that this specification affects all remaining
    /// ones. If so, no more modifiers are allowed.
    /// </summary>
    public string Member { get; }

    /// <summary>
    /// Determines if the member is an asterisk specification, or not.
    /// </summary>
    public bool IsMemberAsterisk => Member == "*";

    /// <summary>
    /// Determines if the member is an enforced specification, or not. If its value is 'false'
    /// then the member can still be matched with an enforced one if their names match.
    /// </summary>
    public bool IsMemberEnforced { get; }

    /// <summary>
    /// Determines if a clone of the member value should be used instead of the value itself.
    /// </summary>
    public bool UseClone { get; }
}