namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents an optional init/set element after the optional specifications have been parsed.
/// </summary>
internal class InitElement
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="member"></param>
    /// <param name="value"></param>
    /// <param name="clone"></param>
    public InitElement(ISymbol member, string value, bool clone)
    {
        Member = member.ThrowWhenNull(nameof(member));
        Value = value.NotNullNotEmpty(nameof(value));
        UseClone = clone;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Member.Name}:{Value}";

    /// <summary>
    /// The member this instance refers to.
    /// </summary>
    public ISymbol Member { get; }
    public IPropertySymbol? Property => Member is IPropertySymbol item ? item : null;
    public IFieldSymbol? Field => Member is IFieldSymbol item ? item : null;

    /// <summary>
    /// The value to assign the the member.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Whether to obtain a clone of the value, or to use the value itself.
    /// </summary>
    public bool UseClone { get; }
}