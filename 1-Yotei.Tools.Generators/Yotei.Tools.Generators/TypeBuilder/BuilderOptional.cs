namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents an optional init/set argument. Format is: '[+|-][name][=value][!]', where:
/// <br/> [+|-]: whether this is an include or exclude specification.
/// <br/> [name]: the name of the optional init/set member.
/// <br/> [=value]: the optional source from where to obtain the value of the element.
/// <br/> [!]: whether to use a clone of the value, or not.
/// </summary>
internal class BuilderOptional
{
    /// <summary>
    /// <inheritdoc cref="BuilderOptional"/>
    /// </summary>
    /// <param name="specs"></param>
    public BuilderOptional(string specs)
    {
        specs = specs.NotNullNotEmpty(nameof(specs));

        switch (specs[0])
        {
            case '-': IsExclude = true; break;
            case '+': IsInclude = true; break;
            default: throw new ArgumentException("No include or exclude portion.").WithData(specs, nameof(specs));
        }
        specs = specs.Substring(1).NotNullNotEmpty(nameof(specs));

        var n = specs.IndexOf('=');

        if (n < 0) // No value portion...
        {
            Name = specs;

            if (Name[0] == '*' && Name.Length > 1) throw new ArgumentException("[Name] '*' must be alone.").WithData(specs, nameof(specs));
            if (Name[0] == '@' && Name.Length > 1) throw new ArgumentException("[Name] '@' must be alone.").WithData(specs, nameof(specs));
            if (Name[0] == '@' && IsExclude) throw new ArgumentException("Exclude '@' is not allowed.").WithData(specs, nameof(specs));

            if (Name[Name.Length-1] == '!')
            {
                Name = Name.Substring(0, Name.Length - 1).NotNullNotEmpty(nameof(Name));
                if (IsExclude) throw new ArgumentException("'!' is not allowed.").WithData(specs, nameof(specs));
                UseClone = true;
            }
        }
        else // Value portion detected...
        {
            Name = specs.Substring(0,n).NotNullNotEmpty(nameof(Name));
            if (Name[0] == '*') throw new ArgumentException("[Name] '*' must be alone.").WithData(specs, nameof(specs));
            if (Name[0] == '@') throw new ArgumentException("[Name] '@' must be alone.").WithData(specs, nameof(specs));
            if (Name[Name.Length - 1] == '!') throw new ArgumentException("Clone [Name] not allowed if [Value] is provided.").WithData(specs, nameof(specs));

            Value = specs.Substring(n + 1).NotNullNotEmpty(nameof(Value));
            if (Value[0] == '*') throw new ArgumentException("[Value] '*' is not allowed.").WithData(specs, nameof(specs));
            if (IsExclude) throw new ArgumentException("Exclude [Value] is not allowed.").WithData(specs, nameof(specs));

            if (Value[Value.Length - 1] == '!')
            {
                Value = Value.Substring(0, Value.Length - 1).NotNullNotEmpty(nameof(Value));
                UseClone = true;
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(IsExclude ? '-' : '+');
        sb.Append(Name);
        if (Value != null) sb.Append($"={Value}");
        if (UseClone) sb.Append('!');
        return sb.ToString();
    }

    /// <summary>
    /// Determines if this is an exclude specification, or not.
    /// </summary>
    public bool IsExclude { get; }

    /// <summary>
    /// Determines if this is an exclude-all specification or not.
    /// </summary>
    public bool IsExcludeAll => IsExclude && IsNameAsterisk;

    /// <summary>
    /// Determines if this is an include specification, or not.
    /// </summary>
    public bool IsInclude { get; }

    /// <summary>
    /// Determines if this is an include-all specification or not.
    /// </summary>
    public bool IsIncludeAll => IsInclude && IsNameAsterisk;

    /// <summary>
    /// The name of the init/set member.
    /// <br/> [-*]: indicates that all previous specifications should be deleted.
    /// <br/> [+*]: indicates all remaining init/set members shall be added.
    /// <br/> If '*' is used then no [value] or [!] portions are allowed.
    /// <br/> [+@]: indicates that the enforced member member and value shall be added. No
    /// [value] or [!] portions are allowed. [-@] is not allowed.
    /// <br/> [-name]: removes any previous element with that name. No [value] or [!] are
    /// allowed.
    /// <br/> [+name]: add the init/set members whose name is given.
    /// </summary>
    public string Name { get; } = default!;

    /// <summary>
    /// Determines if the name is an asterisk specification, or not.
    /// </summary>
    public bool IsNameAsterisk => Name == "*";

    /// <summary>
    /// Determines if the name is an enforced specification, or not.
    /// </summary>
    public bool IsNameEnforced => Name == "@";

    /// <summary>
    /// If not null, then the source from where the value of this init/set element is obtained.
    /// In this case, the value is copied as such.
    /// <br/> [@]: used to obtain the value from the enforced member, if any.
    /// <br/> If null, then a corresponding type member shall be found and its value used.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Determines if a clone of the value shall be used instead of the value itself.
    /// </summary>
    public bool UseClone { get; }
}