namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class ChildNodeExtensions
{
    /// <summary>
    /// Returns the root of the hierarchy the given node belongs to.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static Hierarchy GetHierarchy(this IChildNode node)
    {
        while (true)
        {
            var parent = node.ParentNode;
            if (parent is Hierarchy hierarchy) return hierarchy;
            
            if (parent is IChildNode child) node = child;
            else throw new ApplicationException("Invalid parent node.");
        }
    }
}