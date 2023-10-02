namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a collection of child types.
/// </summary>
internal class ChildTypes : NoDuplicatesList<TypeNode>
{
    bool OnComparer(ITypeSymbol xSymbol, ITypeSymbol ySymbol)
    {
        return
            SymbolEqualityComparer.Default.Equals(xSymbol, ySymbol) &&
            SymbolEqualityComparer.Default.Equals(xSymbol.ContainingType, ySymbol.ContainingType);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildTypes() : base()
    {
        ThrowDuplicates = false;
        Comparer = (x, y) => OnComparer(x.Symbol, y.Symbol);
    }

    /// <summary>
    /// Returns the element in this collection that matches the given arguments.
    /// </summary>
    /// <param name="longName"></param>
    /// <returns></returns>
    public TypeNode? Find(ITypeSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var index = IndexOf(x => OnComparer(symbol, x.Symbol));
        return index >= 0 ? this[index] : null;
    }
}