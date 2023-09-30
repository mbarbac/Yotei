namespace Experimental;

// ========================================================
/// <summary>
/// Represents an optional init/set builder argument(s) from the remaining members not yet used
/// in a given builder, with the format: '[+|-][*|name][=[@]|[member[!]]', where:
/// <br/>- [+|-] indicates whether this is an include or exclude specification. Exclude ones only
/// accept a [name] or a [*] element.
/// <br/>- [name] is the actual name of the builder argument. If [*] is used it means that all
/// possible remaining ones will be included or excluded. In addition, if [*] is used then no
/// further specifications are allowed for this element.
/// <br/>- [=[@]|[member[!]] indicates that the value of the parameter will be taken from the
/// given source.
/// <br/> If it is null, then the value of a corresponding member will be used.
/// <br/> If it is [=!], then a clone of the value of that corresponding member will be used
/// instead of its straight one.
/// <br/> If it is [@], then the value of the external enforced value will be used.
/// <br/> If it is [member], it is the name of the member from which to obtain the value of the
/// parameter, and it can be followed by a [!] character to use its clone instead.
/// </summary>
internal class BuilderOptional
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="spec"></param>
    public BuilderOptional(string spec)
    {
        spec = spec.NotNullNotEmpty(true, nameof(spec));

        switch (spec[0])
        {
            case '+': IsInclude = true; break;
            case '-': IsExclude = true; break;

            default: throw new ArgumentException(
                "Optional specification must beguin with '+' or '-'.")
                .WithData(spec);
        }
        spec = spec.Substring(1).NotNullNotEmpty(true, nameof(spec));

        var n = spec.IndexOf('=');
        Name = n < 0 ? spec : spec.Substring(0, n).NotNullNotEmpty(true, nameof(Name));

        if (Name == "*")
        {
            if (n >= 0) throw new ArgumentException(
                "No further specifications allowed after a '*' symbol.")
                .WithData(spec);

            return;
        }

        if (IsExclude)
        {
            if (n >= 0) throw new ArgumentException(
                "Only name specification is allowed for 'Exclude' ones.")
                .WithData(spec);

            return;
        }

        if (n >= 0)
        {
            spec = spec.Substring(n + 1);

            if (spec.Length == 0) throw new ArgumentException(
                "Missed specification after the '=' symbol.")
                .WithData(spec, nameof(spec));

            if (spec == "@") { UseEnforced = true; }
            else if (spec == "@!") { UseEnforced = true; UseClone = true; }
            else
            {
                if (spec.EndsWith("!"))
                {
                    spec = spec.Substring(0, spec.Length - 1);
                    UseClone = true;
                }
                Member = spec.NullWhenEmpty();
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var s = IsInclude ? "+" : "-";
        s += Name;

        var eq = false;

        if (Member != null)
        {
            s += $"={Member}";
            eq = true;
        }
        if (UseEnforced)
        {
            if (!eq) s += "=";
            s += "@";
            eq = true;
        }
        if (UseClone)
        {
            if (!eq) s += "=";
            s += "!";
        }
        return s;
    }

    /// <summary>
    /// Whether this is an include specification, or not.
    /// </summary>
    public bool IsInclude { get; } = false;

    /// <summary>
    /// Whether this is an exclude specification, or not.
    /// </summary>
    public bool IsExclude { get; } = false;

    /// <summary>
    /// The actual name of the builder argument.
    /// </summary>
    public string Name { get; } = default!;

    /// <summary>
    /// The name of the member from which to obtain the value of this parameter. If it is null,
    /// or it has a given value, then a corresponding member will be found. If it is [@], then
    /// the value shall be obtained from an external enforced variable.
    /// </summary>
    public string? Member { get; } = null;

    /// <summary>
    /// Whether a clone of the value of the corresponding member shall be used, instead of the
    /// straight value of that member.
    /// </summary>
    public bool UseClone { get; } = false;

    /// <summary>
    /// Whether the value to use shall be obtained from an external enforced variable, or not.
    /// </summary>
    public bool UseEnforced { get; } = false;
}