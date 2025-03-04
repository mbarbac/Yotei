namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class NodeExtensions
{
    /// <summary>
    /// Gets the candidate associated with the given node, if any.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IValidCandidate? GetCandidate(this INode node) => node.ThrowWhenNull() switch
    {
        TypeNode item => item.Candidate,
        PropertyNode item => item.Candidate,
        FieldNode item => item.Candidate,
        MethodNode item => item.Candidate,

        _ => null
    };

    // ----------------------------------------------------

    /// <summary>
    /// Returns the list of attributes captured for the given node, through its associated
    /// candidate, if any, or returns an empty list otherwise. Attributes are only those that
    /// have been specified by the tree generator, not any other ones.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static List<AttributeData> GetCapturedAttributes(
        this INode node)
        => node.GetCandidate()?.Attributes.ToList() ?? [];
}