namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
public interface IIdentifier : IEquatable<IIdentifier>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    string? Value { get; }
}

// ========================================================
public static class IIdentifierExtensions
{
    /// <summary>
    /// Determines if this identifier matches the given specifications. Comparison is performed
    /// by comparing parts from right to left, where any null or empty specification one is taken
    /// as an implicit match.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static bool Match(this IIdentifier identifier, string? specs)
    {
        identifier.ThrowWhenNull(specs);

        if ((specs = specs.NullWhenEmpty()) == null) return true;

        var engine = identifier.Engine;
        var source = identifier is IIdentifierChain chain ? chain : new Code.IdentifierChain(engine, ((IIdentifierPart)identifier).Value);
        var target = new Code.IdentifierChain(engine, specs);

        for (int i = 0; ; i++)
        {
            if (i >= target.Count) break;
            if (i >= source.Count)
            {
                while (i < target.Count)
                {
                    var value = target[^(i + 1)].UnwrappedValue;
                    if (value != null) return false;
                    i++;
                }
                break;
            }

            var tvalue = target[^(i + 1)].UnwrappedValue; if (tvalue == null) continue;
            var svalue = source[^(i + 1)].UnwrappedValue;
            if (string.Compare(svalue, tvalue, !engine.CaseSensitiveNames) != 0) return false;
        }

        return true;
    }
}