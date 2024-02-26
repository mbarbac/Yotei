namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
/// <param name="parent"></param>
/// <param name="candidate"></param>
internal class XTypeNode(
    INode parent, TypeCandidate candidate) : TypeNodeEx(parent, candidate)
{
    // The collection of inherited types from the attributes that decorates this syntax node.
    readonly List<InheritedType> InheritedTypes = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        InheritedTypes.Clear();

        // We need an inheritance chain to implement...
        if (Syntax.BaseList != null)
        {
            var nodes = Syntax.BaseList!.ChildNodes().ToList();
            var ats = Syntax.AttributeLists.GetAttributes();

            // We process all attributes in order, accepting any to override others...
            foreach (var at in ats)
            {
                List<int> indexes = [];
                bool change = false;
                bool prevent = false;

                // Using the attribute arguments...
                if (at.ArgumentList != null)
                {
                    for (int i = 0; i < at.ArgumentList.Arguments.Count; i++)
                    {
                        var arg = at.ArgumentList.Arguments[i];

                        // Named arguments...
                        if (arg.NameEquals != null)
                        {
                            if (arg.NameEquals.Name.ShortName() == "ChangeProperties")
                            {
                                var expr = (LiteralExpressionSyntax)arg.Expression;
                                change = (bool)expr.Token.Value!;
                                continue;
                            }
                            if (arg.NameEquals.Name.ShortName() == "PreventVirtual")
                            {
                                var expr = (LiteralExpressionSyntax)arg.Expression;
                                prevent = (bool)expr.Token.Value!;
                                continue;
                            }
                        }

                        // First argument...
                        if (i == 0)
                        {
                            if (arg.Expression is LiteralExpressionSyntax unique)
                            {
                                var temp = (int)unique.Token.Value!;
                                if (!indexes.Contains(temp)) indexes.Add(temp);
                                continue;
                            }

                            if (arg.Expression is CollectionExpressionSyntax multiple)
                            {
                                foreach (var entry in multiple.Elements.Cast<ExpressionElementSyntax>())
                                {
                                    var expr = (LiteralExpressionSyntax)entry.Expression;
                                    var temp = (int)expr.Token.Value!;
                                    if (!indexes.Contains(temp)) indexes.Add(temp);
                                    continue;
                                }
                            }
                        }
                    }
                }

                // We may have no index specification in the attribute...
                if (indexes.Count == 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var item = Create(nodes, i, change, prevent);
                        Update(InheritedTypes, item);
                    }
                }

                // Or using the captured indexes from the attribute...
                else
                {
                    foreach (var index in indexes)
                    {
                        var item = Create(nodes, index, change, prevent);
                        Update(InheritedTypes, item);
                    }
                }
            }
        }

        // No inherited types found...
        if (InheritedTypes.Count == 0)
        {
            context.NoInheritedElements(Syntax);
            return false;
        }
        return true;
    }

    // Creates a new inherited type instance...
    InheritedType Create(List<SyntaxNode> nodes, int index, bool change, bool prevent)
    {
        var syntax = (SimpleNameSyntax)((SimpleBaseTypeSyntax)nodes[index]).Type;
        var symbol = (INamedTypeSymbol)SemanticModel.GetSymbolInfo(syntax).Symbol!;
        return new InheritedType(syntax, symbol, change, prevent);
    }

    // Finds the index of the symbol in the captured inherited types...
    static int IndexOf(List<InheritedType> items, INamedTypeSymbol symbol)
    {
        for (int i = 0; i < items.Count; i++)
            if (SymbolEqualityComparer.Default.Equals(items[i].Symbol, symbol)) return i;

        return -1;
    }

    // Updates the inherited type...
    static void Update(List<InheritedType> items, InheritedType item)
    {
        var index = IndexOf(items, item.Symbol);

        if (index < 0) items.Add(item);
        else items[index] = item;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        var comparer = SymbolEqualityComparer.Default;
        var hostType = Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var prev = false;

        foreach (var type in InheritedTypes)
        {
            if (!prev) cb.AppendLine();
            prev = true;

            // Properties...
            foreach (var property in type.Symbol.GetMembers().OfType<IPropertySymbol>())
            {
                if (!comparer.Equals(type.Symbol, property.Type)) continue; // Not same property type...
            }

            // Methods...
            foreach (var method in type.Symbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (!comparer.Equals(type.Symbol, method.ReturnType)) continue; // Not same return type...
            }
        }
    }
}