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
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract string? Value { get; init; }

    /// <summary>
    /// <inheritdoc/>
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

        if (this is IIdentifierSinglePart sitem && target is IIdentifierSinglePart starget)
        {
            var vsource = sitem.NonTerminatedValue;
            var vtarget = starget.NonTerminatedValue;

            return vsource == null || vtarget == null
                ? true
                : string.Compare(vsource, vtarget, !Engine.CaseSensitiveNames) == 0;
        }

        var sources = GetArray(this);
        var targets = GetArray(target);
        var max = sources.Length > targets.Length ? sources.Length : targets.Length;

        if (sources.Length < max) sources = sources.ResizeHead(max, new IdentifierSinglePart(Engine));
        if (targets.Length < max) targets = targets.ResizeHead(max, new IdentifierSinglePart(target.Engine));

        for (int i = max - 1; i >= 0; i--)
        {
            var vsource = sources[i].NonTerminatedValue; if (vsource == null) continue;
            var vtarget = targets[i].NonTerminatedValue; if (vtarget == null) continue;

            if (string.Compare(vsource, vtarget, !Engine.CaseSensitiveNames) != 0)
                return false;
        }
        return true;

        // Obtains an array from the given identifier...
        static IIdentifierSinglePart[] GetArray(IIdentifier item) => item switch
        {
            IIdentifierSinglePart temp => [temp],
            IIdentifierMultiPart temp => temp.ToArray(),
            _ => throw new UnExpectedException("Invalid identifier type.").WithData(item)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Creates a new instance of the simpler possible form.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value = null)
    {
        var items = new IdentifierMultiPart(engine, value);
        return items.Reduce();
    }
}