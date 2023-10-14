namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a collection of child methods.
/// </summary>
internal class ChildMethods : NoDuplicatesList<MethodNode>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildMethods() : base()
    {
        ThrowDuplicates = false;
        Equivalent = (x, y) => Comparer(x.Symbol, y.Symbol);
    }

    /// <summary>
    /// Invoked to compare two elements.
    /// </summary>
    bool Comparer(IMethodSymbol xsymbol, IMethodSymbol ysymbol)
    {
        return
            SymbolEqualityComparer.Default.Equals(xsymbol, ysymbol) &&
            SymbolEqualityComparer.Default.Equals(xsymbol.ContainingType, ysymbol.ContainingType);
    }

    /// <summary>
    /// Returns the element in this collection that matches the given criteria.
    /// </summary>
    /// <param name="longName"></param>
    /// <returns></returns>
    public MethodNode? Find(IMethodSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var index = IndexOf(x => Comparer(x.Symbol, symbol));
        return index >= 0 ? this[index] : null;
    }
}