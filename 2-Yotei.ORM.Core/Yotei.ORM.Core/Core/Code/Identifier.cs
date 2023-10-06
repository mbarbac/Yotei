namespace Yotei.ORM.Core.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
public abstract class Identifier : IIdentifier
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it is an empty or missed one.
    /// </summary>
    public abstract string? Value { get; init; }

    /// <summary>
    /// Reduces this instance to a simpler one, if such is possible.
    /// </summary>
    /// <returns></returns>
    public abstract IIdentifier Reduce();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a single part instance if the given value was null, or a single-part one, or a
    /// multipart instance otherwise.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value)
    {
        var items = new IdentifierMultiPart(engine, value);
        return items.Count switch
        {
            0 => new IdentifierSinglePart(engine),
            1 => items[0],
            _ => items,
        };
    }
}