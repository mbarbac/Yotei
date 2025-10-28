namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class SymbolExtensions
{
    extension(ISymbol symbol)
    {
        /// <summary>
        /// Gets the symbol for the containing type or the nearest enclosing namespace, or null
        /// if any.
        /// </summary>
        public ISymbol? ContainingTypeOrNamespace =>
            (ISymbol?)symbol.ContainingType ??
            (ISymbol?)symbol.ContainingNamespace;
    }
}