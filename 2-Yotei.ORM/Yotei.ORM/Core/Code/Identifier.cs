namespace Yotei.ORM.Code;

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
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract IIdentifier Reduce();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Match(IIdentifier item)
    {
        item.ThrowWhenNull();

        if (MatchSingleSingle(this, item)) return true;
        if (MatchSingleMulti(this, item)) return true;
        if (MatchMultiSingle(this, item)) return true;
        if (MatchMultiMulti(this, item)) return true;

        return false;

        // Matching when both are single-part instances...
        bool MatchSingleSingle(IIdentifier source, IIdentifier target)
        {
            if (source is not IIdentifierSinglePart _source) return false;
            if (target is not IIdentifierSinglePart _target) return false;

            return Compare(_source, _target);
        }

        // Matching when source is single-part...
        bool MatchSingleMulti(IIdentifier source, IIdentifier target)
        {
            if (source is not IIdentifierSinglePart _source) return false;
            if (target is not IIdentifierMultiPart _target) return false;

            if (_target.Count == 0) return true; // Missed filter is considered a match...

            // If previous filters are valid, then no match!
            for (int i = 0; i < _target.Count - 2; i++)
                if (_target[i].Value != null) return false;

            return Compare(_source, _target[^1]);
        }

        // Matching when target is single-part...
        bool MatchMultiSingle(IIdentifier source, IIdentifier target)
        {
            if (source is not IIdentifierMultiPart _source) return false;
            if (target is not IIdentifierSinglePart _target) return false;

            if (target.Value == null) return true; // Missed filter is considered a match...
            if (_source.Count == 0) return false; // But source cannot match...
            return Compare(_source[^1], _target);
        }

        // Matching when both are multi-parts...
        bool MatchMultiMulti(IIdentifier source, IIdentifier target)
        {
            if (source is not IIdentifierMultiPart _source) return false;
            if (target is not IIdentifierMultiPart _target) return false;

            var nsource = _source.Count;
            var ntarget = _target.Count;
            if (nsource == 0 && ntarget == 0) return true; // Both are empty...

            if (ntarget == 0) return true; // Missed filter is considered a match...
            if (nsource == 0) return false; // But source cannot match...

            nsource--;
            ntarget--;
            while (nsource >= 0 && ntarget >= 0)
            {
                var xsource = _source[nsource];
                var xtarget = _target[ntarget];
                if (!Compare(xsource, xtarget)) return false;

                nsource--; if (nsource < 0) // If any previous filter, then no match!
                {
                    for (int i = 0; i < ntarget; i++)
                        if (_target[i].Value != null) return false;
                }
                ntarget--;
            }
            return true;
        }

        // Compares the non-terminated values of the two given parts...
        bool Compare(IIdentifierSinglePart source, IIdentifierSinglePart target)
        {
            var svalue = source.NonTerminatedValue;
            var tvalue = target.NonTerminatedValue;

            // Empty or null filter is considered a match...
            if (tvalue == null) return true;
            return string.Compare(svalue, tvalue, !source.Engine.CaseSensitiveNames) == 0;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new identifier of the simpler possible form based upon the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value = null)
    {
        if (value == null) return new IdentifierSinglePart(engine);
        if (!value.Contains('.')) return new IdentifierSinglePart(engine, value);
        return new IdentifierMultiPart(engine, value).Reduce();
    }
}