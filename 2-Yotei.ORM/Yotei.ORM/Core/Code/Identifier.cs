namespace Yotei.ORM;

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
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract string? Value { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler one, if such is possible.
    /// </summary>
    /// <returns></returns>
    public abstract IIdentifier Reduce();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool Match(IIdentifier target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var sparts = GetParts(this);
        var tparts = GetParts(target);
        var max = sparts.Length > tparts.Length ? sparts.Length : tparts.Length;
        sparts = sparts.ResizeHead(max);
        tparts = tparts.ResizeHead(max);

        for (int i = 0; i < tparts.Length; i++)
            if (tparts[i] == null) sparts[i] = null;

        for (int i = 0; i < sparts.Length; i++)
        {
            var svalue = sparts[i];
            var tvalue = tparts[i];
            var same = string.Compare(svalue, tvalue, !Engine.CaseSensitiveNames) == 0;
            if (!same) return false;
        }
        return true;

        // Gets the parts of the given identifier.
        static string?[] GetParts(IIdentifier item)
        {
            return item is IIdentifierSinglePart spart
                ? [spart.NonTerminatedValue]
                : ((IIdentifierMultiPart)item).Select(x => x.NonTerminatedValue).ToArray();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new identifier of the simpler possible form based upon the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value = null) => value == null
        ? new IdentifierSinglePart(engine)
        : new IdentifierMultiPart(engine, value).Reduce();
}