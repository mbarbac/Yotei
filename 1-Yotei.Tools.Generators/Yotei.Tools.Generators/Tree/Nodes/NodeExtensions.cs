namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class NodeExtensions
{
    /// <summary>
    /// Returns the candidate associated with the given node, if any.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static INodeCandidate? GetCandidate(this INode node)
    {
        node.ThrowWhenNull();

        return node switch
        {
            TypeNode item => item.Candidate,
            PropertyNode item => item.Candidate,
            FieldNode item => item.Candidate,
            MethodNode item => item.Candidate,

            _ => null
        };
    }

    // -----------------------------------------------------

    /// <summary>
    /// Returns the list of attributes captured for the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static List<AttributeData> GetCapturedAttributes(this INode node)
    {
        return node.GetCandidate()?.Attributes.ToList() ?? [];
    }
}