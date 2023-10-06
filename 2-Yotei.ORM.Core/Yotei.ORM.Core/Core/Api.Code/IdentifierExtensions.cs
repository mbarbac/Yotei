namespace Yotei.ORM.Core;

// ========================================================
public static class IdentifierExtensions
{
    /// <summary>
    /// Determines if the source instance matches the target one, or not. Matching is performed
    /// from right to left, regardless of the number of parts of each instance. Missed or null
    /// parts are considered an implicit match. Value comparisons use the non-terminated value
    /// of each part, with the case sensitive settings of the source instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Match(this IIdentifier source, IIdentifier target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var sources = GetArray(source);
        var targets = GetArray(target);
        var max = sources.Length > targets.Length ? sources.Length : targets.Length;

        if (sources.Length < max) sources = sources.ResizeHead(max, new IdentifierSinglePart(source.Engine));
        if (targets.Length < max) targets = targets.ResizeHead(max, new IdentifierSinglePart(target.Engine));

        for (int i = max - 1; i >= 0; i--)
        {
            var vsource = sources[i].NonTerminatedValue; if (vsource == null) continue;
            var vtarget = targets[i].NonTerminatedValue; if (vtarget == null) continue;

            if (string.Compare(vsource, vtarget, !source.Engine.CaseSensitiveNames) != 0)
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
}