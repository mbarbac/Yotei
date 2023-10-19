namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a builder method parameter. Format is: '[name][=value][!]', where:
/// <br/> [name]: the name of the method argument.
/// <br/> [=value]: the optional source from where to obtain the value of the element.
/// <br/> [!]: whether to use a clone of the value, or not.
/// </summary>
internal class BuilderArgument
{
    /// <summary>
    /// <inheritdoc cref="BuilderArgument"/>
    /// </summary>
    /// <param name="specs"></param>
    public BuilderArgument(string specs)
    {
        specs = specs.NotNullNotEmpty(nameof(specs));

        var n = specs.IndexOf('=');

        if (n < 0) // No value portion...
        {
            Name = specs;
            if (Name[0] == '*' && Name.Length > 1) throw new ArgumentException("[Name] '*' must be alone.").WithData(specs, nameof(specs));

            if (Name[Name.Length - 1] == '!')
            {
                Name = Name.Substring(0, Name.Length - 1).NotNullNotEmpty(nameof(Name));
                UseClone = true;
            }
        }

        else // Value portion detected...
        {
            Name = specs.Substring(0, n).NotNullNotEmpty(nameof(Name));
            if (Name[0] == '*') throw new ArgumentException("[Name] '*' must be alone.").WithData(specs, nameof(specs));

            Value = specs.Substring(n + 1).NotNullNotEmpty(nameof(Value));
            if (Value[0] == '*') throw new ArgumentException("[Value] '*' is not allowed.").WithData(specs, nameof(specs));
            if (Value[0] == '!') throw new ArgumentException("[Value] '!' is not allowed.").WithData(specs, nameof(specs));

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
        sb.Append(Name);
        if (Value != null) sb.Append($"={Value}");
        if (UseClone) sb.Append('!');
        return sb.ToString();
    }

    /// <summary>
    /// The name of the method argument.
    /// <br/> [*]: indicates that all method arguments shall be tested. No [value] or [!]
    /// portions are then allowed.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Determines if the name is an asterisk specification, or not.
    /// </summary>
    public bool IsNameAsterisk => Name == "*";

    /// <summary>
    /// If not null, then the source from where the value of this argument is obtained.
    /// <br/> [@]: used to obtain the value from the enforced member, if any.
    /// <br/> [this]: used to inject the value or reference of the hosting type.
    /// <br/> If null, then a corresponding type member shall be found and its value used.
    /// </summary>
    public string? Value { get; private set; }

    /// <summary>
    /// Determines if the value is an enforced specification, or not.
    /// </summary>
    public bool IsValueEnforced => Value == "@";

    /// <summary>
    /// Determines if the value is a "this" specification, or not.
    /// </summary>
    public bool IsValueThis => string.Compare(Value, "this", ignoreCase: true) == 0;

    /// <summary>
    /// Determines if a clone of the value shall be used instead of the value itself.
    /// </summary>
    public bool UseClone { get; private set; }
}