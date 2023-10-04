namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents an optional init/set element.
/// </summary>
internal class InitElement
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="member"></param>
    /// <param name="useClone"></param>
    public InitElement(ISymbol member, bool useClone)
    {
        Member = member.ThrowWhenNull(nameof(member));
        UseClone = useClone;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Member.Name
        + (UseClone ? ":Clone" : string.Empty)
        + (IsInit ? ":Init" : string.Empty);

    /// <summary>
    /// The member this instance refers to.
    /// </summary>
    public ISymbol Member { get; }
    public IPropertySymbol? Property => Member is IPropertySymbol item ? item : null;
    public IFieldSymbol? Field => Member is IFieldSymbol item ? item : null;

    /// <summary>
    /// Whether to obtain a clone of the value, or to use the value itself.
    /// </summary>
    public bool UseClone { get; set; }

    /// <summary>
    /// Determines if this instance can be used as a init element, or not.
    /// </summary>
    public bool IsInit
        => Field != null
        || (Property != null && Property.SetMethod != null && Property.SetMethod.IsInitOnly);

    /// <summary>
    /// Gets the code that represents the value of this init/set argument.
    /// </summary>
    /// <param name="enforcedMember"></param>
    /// <param name="enforcedUsed"></param>
    /// <returns></returns>
    public string GetValue(EnforcedMember? enforcedMember, out bool enforcedUsed)
    {
        string value;

        if (enforcedMember != null && enforcedMember.Member.Name == Member.Name)
        {
            value = enforcedMember.ValueName;
            enforcedUsed = true;
        }
        else
        {
            value = Member.Name;
            enforcedUsed = false;
        }

        return UseClone
            ? $"({value} is null) ? null : {value}.Clone()"
            : value;

    }
}