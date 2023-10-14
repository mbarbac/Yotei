namespace Yotei.Tools.Generators.Shared;

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
    public InitElement(ISymbol member)
    {
        Member = member.ThrowWhenNull(nameof(member));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Member.Name + (UseClone ? ":Clone" : string.Empty);

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
    /// Whether this element shall use the enforced one, or not.
    /// </summary>
    public bool UseEnforced { get; set; }

    /// <summary>
    /// Determines if this element is an init-only property, or not.
    /// </summary>
    public bool IsInitOnly => Property?.SetMethod?.IsInitOnly ?? false;
}