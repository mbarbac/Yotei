namespace Yotei.Tools.InheritGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    string YOTEIGENERATED => "[Yotei.Tools.InheritGenerator.YoteiGenerated]";
    ImmutableArray<TypeElement> TypeElements { get; set; } = ImmutableArray<TypeElement>.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;
        
        TypeElements = Symbol.GetAttributes(InheritAttr.LongName)
            .Select(x => GetTypeElement(x, context))
            .ToImmutableArray();

        return true;
    }

    TypeElement GetTypeElement(AttributeData attr, SourceProductionContext context)
    {
        var tparg = attr.ConstructorArguments[0];
        if (tparg.IsNull)
        {
            context.TypeArgumentNull(Symbol);
            throw new ArgumentNullException("Type to inherit from is null.");
        }

        var type = (ITypeSymbol)tparg.Value!;
        var change = InheritAttr.GetChangeProperties(attr, out var props) && props;
        var names = InheritAttr.GetGenericNames(attr, out var gnames) ? gnames : [];

        var args = new List<TypeArgument>();
        var gens = 0;

        if (type is INamedTypeSymbol named && named.Arity > 0)
        {
            for (int i = 0; i < named.TypeArguments.Length; i++)
            {
                var symbol = named.TypeArguments[i];
                var generic = false;
                var name = "";

                if (symbol is IErrorTypeSymbol temp)
                {
                    if (gens >= names.Length)
                    {
                        context.NotEnoughGenericNames(Symbol, type);
                        throw new ArgumentException("Not enough generic types.");
                    }

                    generic = true;
                    name = names[gens];
                    gens++;
                }

                args.Add(new(symbol, generic, name));
            }

            return new(type, args.ToImmutableArray(), change);
        }
        else
        {
            return new(type, ImmutableArray<TypeArgument>.Empty, change);
        }
    }

    // ----------------------------------------------------

    protected override string GetTypeName()
    {
        var names = new List<string>();

        foreach (var item in TypeElements)
        {
            var found = false;
            foreach (var type in Symbol.AllBaseTypes()) if (item.SameAs(type)) { found = true; break; }
            foreach (var type in Symbol.Interfaces) if (item.SameAs(type)) { found = true; break; }
            if (!found)
            {
                if (item.Type.IsInterface()) names.Add(item.Reduced);
                else names.Insert(0, item.Reduced);
            }
        }

        var sb = new StringBuilder();
        sb.Append(base.GetTypeName());

        if (names.Count > 0)
        {
            sb.Append(" : ");
            sb.Append(string.Join(", ", names));
        }

        return sb.ToString();
    }
}