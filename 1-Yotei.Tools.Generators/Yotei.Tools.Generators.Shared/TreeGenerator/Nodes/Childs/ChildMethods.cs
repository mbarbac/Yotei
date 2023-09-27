namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a collection of child methods.
/// </summary>
internal class ChildMethods : NoDuplicatesList<MethodNode>
{
    bool OnComparer(IMethodSymbol xSymbol, IMethodSymbol ySymbol)
    {
        return
            SymbolEqualityComparer.Default.Equals(xSymbol, ySymbol) &&
            SymbolEqualityComparer.Default.Equals(xSymbol.ContainingType, ySymbol.ContainingType);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ChildMethods() : base()
    {
        ThrowDuplicates = false;
        Comparer = (x, y) => OnComparer(x.Symbol, y.Symbol);
    }

    /// <summary>
    /// Returns the element in this collection that matches the given arguments.
    /// </summary>
    /// <param name="longName"></param>
    /// <returns></returns>
    public MethodNode? Find(IMethodSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var index = IndexOf(x => OnComparer(symbol, x.Symbol));
        return index >= 0 ? this[index] : null;
    }

    /// <summary>
    /// Returns the node in the collection that matches the given arguments, or invokes the
    /// given action to create a new one that will be added to the collection and returned.
    /// </summary>
    /// <param name="longName"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    //public MethodNode Locate(IMethodSymbol symbol, Func<MethodNode> create)
    //{
    //    create = create.ThrowWhenNull(nameof(create));

    //    var node = Find(symbol);
    //    if (node == null)
    //    {
    //        node = create();
    //        Add(node);
    //    }
    //    return node;
    //}
}