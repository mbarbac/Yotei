namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class CompilationExtensions
{
    /// <summary>
    /// Gets the compilation unit syntax the given node belongs to.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static CompilationUnitSyntax GetCompilationUnitSyntax(this SyntaxNode node)
    {
        node.ThrowWhenNull();

        while (node != null)
        {
            if (node is CompilationUnitSyntax item) return item;
            node = node.Parent!;
        }

        throw new ArgumentException(
            $"Cannot get the compilation unit syntax of node: {node}");
    }
}