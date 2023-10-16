namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a collection of child properties.
/// </summary>
internal class ChildProperties : CoreList<PropertyNode>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildProperties() : base()
    {
        AcceptDuplicate = (item) => false;
        Compare = (x, y) => Comparer(x.Symbol, y.Symbol);
    }

    /// <summary>
    /// Invoked to compare two elements.
    /// </summary>
    bool Comparer(IPropertySymbol xsymbol, IPropertySymbol ysymbol)
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
    public PropertyNode? Find(IPropertySymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var index = IndexOf(x => Comparer(x.Symbol, symbol));
        return index >= 0 ? this[index] : null;
    }
}