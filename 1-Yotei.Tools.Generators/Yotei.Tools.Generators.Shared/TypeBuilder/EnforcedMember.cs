namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents an enforced member of the type being built, whose value will be modified with the
/// one obtained from an external variable.
/// </summary>
internal class EnforcedMember
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="member"></param>
    /// <param name="valueName"></param>
    public EnforcedMember(ISymbol member, string valueName)
    {
        Member = member.ThrowWhenNull(nameof(member));
        ValueName = valueName.NotNullNotEmpty(nameof(ValueName));

        if (member is not IPropertySymbol and not IFieldSymbol)
            throw new ArgumentException(
                $"Member '{member.Name}' is neither a property nor a field.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Member}={ValueName})";

    /// <summary>
    /// The member this instance refers to.
    /// </summary>
    public ISymbol Member { get; }
    public IPropertySymbol? Property => Member is IPropertySymbol item ? item : null;
    public IFieldSymbol? Field => Member is IFieldSymbol item ? item : null;

    /// <summary>
    /// The name of the external variable from which to obtain the new value of the associated
    /// member.
    /// </summary>
    public string ValueName { get; }
}