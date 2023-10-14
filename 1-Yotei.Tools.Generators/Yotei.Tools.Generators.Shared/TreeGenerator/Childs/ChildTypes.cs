namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a collection of child types.
/// </summary>
internal class ChildTypes : NoDuplicatesList<TypeNode>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildTypes() : base()
    {
        ThrowDuplicates = false;
        Equivalent = (x, y) => Comparer(x.Symbol, y.Symbol);
    }

    /// <summary>
    /// Invoked to compare two elements.
    /// </summary>
    bool Comparer(ITypeSymbol xsymbol, ITypeSymbol ysymbol)
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
    public TypeNode? Find(ITypeSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var index = IndexOf(x => Comparer(x.Symbol, symbol));
        return index >= 0 ? this[index] : null;
    }
}