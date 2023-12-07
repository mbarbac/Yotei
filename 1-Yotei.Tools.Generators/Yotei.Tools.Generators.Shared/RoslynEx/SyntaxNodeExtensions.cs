namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class SyntaxNodeExtensions
{
    /// <summary>
    /// Returns the compilation unit syntax that is the parent of the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static CompilationUnitSyntax GetCompilationUnitSyntax(this SyntaxNode node)
    {
        while (node != null)
        {
            if (node is CompilationUnitSyntax item) return item;
            node = node.Parent!;
        }

        throw new ArgumentException(
            $"Cannot get the compilation unit syntax of node: {node}");
    }
}