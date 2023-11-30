namespace Experimental.Templates.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantElement"/>
/// </summary>
public class InvariantElement(string name) : IInvariantElement
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name ?? string.Empty;

    /// <summary>
    /// The name by which this element is known.
    /// </summary>
    public string Name { get; } = name;
}