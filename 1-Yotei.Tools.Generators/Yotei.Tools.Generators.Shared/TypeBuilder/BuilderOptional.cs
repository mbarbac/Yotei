namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents an optional init/set builder argument from the remaining members not yet used
/// in a given builder.
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

        var n = spec.IndexOf('=');
        Name = n < 0 ? spec : spec.Substring(0, n).NotNullNotEmpty(nameof(Name));

        if (Name == "*")
        {
            if (n >= 0) throw new ArgumentException(
                "No further specifications allowed after a '*' symbol.")
                .WithData(spec, nameof(spec));

            return;
        }

        if (IsExclude)
        {
            if (n >= 0) throw new ArgumentException(
                "Only name specification is allowed for 'Exclude' ones.")
                .WithData(spec, nameof(spec));

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
    public bool IsInclude { get; }

    /// <summary>
    /// Whether this is an exclude specification, or not.
    /// </summary>
    public bool IsExclude { get; }

    /// <summary>
    /// The actual name of the builder argument.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name of the member from which to obtain the value of this parameter. If it is null,
    /// or it has a given value, then a corresponding member will be found. If it is [@], then
    /// the value shall be obtained from an external enforced variable.
    /// </summary>
    public string? Member { get; }

    /// <summary>
    /// Whether a clone of the value of the corresponding member shall be used, instead of the
    /// straight value of that member.
    /// </summary>
    public bool UseClone { get; }

    /// <summary>
    /// Whether the value to use shall be obtained from an external enforced variable, or not.
    /// </summary>
    public bool UseEnforced { get; }
}