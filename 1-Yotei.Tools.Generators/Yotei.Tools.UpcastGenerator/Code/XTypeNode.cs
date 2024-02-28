namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode(INode parent, TypeCandidate candidate) : TypeNodeEx(parent, candidate)
{
    readonly ChildList<UpcastType> UTypes = [];
    readonly ChildList<IPropertySymbol> UProperties = [];
    readonly ChildList<IFieldSymbol> UFields = [];

    // ----------------------------------------------------

    protected override bool OnValidate(SourceProductionContext context)
    {
        if (Syntax.BaseList == null)
        {
            throw new Exception("TO-DO");
            return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// Capturing before the default invocation the actual list of inherited types, as it may
    /// happen we need to manually add them to the inheritance chain.
    /// </remarks>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        foreach (var node in Syntax.BaseList!.Types)
        {
            if (node.Type is not GenericNameSyntax named) continue;
            if (named.Arity != 1) continue;
            if (named.Identifier.Text is not "IUpcast" and not "IUpcastEx") continue;

            var syntax = named.TypeArgumentList.Arguments[0];
            var info = SemanticModel.GetTypeInfo(syntax);
            var type = (INamedTypeSymbol?)info.Type;
            if (type == null)
            {
                throw new Exception("TO-DO");
                return;
            }

            var props = named.Identifier.Text == "IUpcastEx";
            var item = new UpcastType(type, props);
            UTypes.Add(item, raiseDuplicates: false);
        }

        // Base method will emit the inheritance chain and the captured contents...
        base.Emit(context, cb);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// We may need to add those not actually implemented.
    /// </remarks>
    protected override string GetTypeHeader()
    {
        var comparer = SymbolEqualityComparer.Default;
        var temps = UTypes.ToList();

        foreach (var node in Syntax.BaseList!.Types)
        {
            var info = SemanticModel.GetTypeInfo(node.Type);
            var type = (INamedTypeSymbol?)info.Type;
            if (type == null) continue;

            var temp = temps.Find(x => comparer.Equals(x.Symbol, type));
            if (temp != null) temps.Remove(temp);
        }

        var options = new EasyNameOptions(useGenerics: true);
        var name = Symbol.EasyName(options);
        
        var sb = new StringBuilder();
        sb.Append(name);

        if (temps.Count > 0)
        {
            sb.Append(" : "); for (int i = 0; i < temps.Count; i++)
            {
                if (i != 0) sb.Append(", ");

                name = temps[i].Symbol.EasyName(options);
                sb.Append(name);
            }
        }

        return sb.ToString();
    }
}