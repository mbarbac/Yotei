namespace Yotei.Tools.InheritGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    InheritElement[] Elements { get; set; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        Elements = InheritAttr.GetElements(Symbol).ToArray();
        base.Print(context, cb);
    }

    /// <inheritdoc/>
    protected override string GetTypeName()
    {
        var names = new List<string>();

        foreach (var item in Elements)
        {
            var expanded = item.ToString(true);
            var reduced = item.ToString();
            var found = false;

            foreach (var type in Symbol.AllBaseTypes())
            {
                var name = type.ToString();
                if (name == expanded) found = true;
            }

            foreach (var type in Symbol.Interfaces)
            {
                var name = type.ToString();
                if (name == expanded) found = true;
            }

            if (!found)
            {
                if (item.Symbol.IsInterface()) names.Add(reduced);
                else names.Insert(0, reduced);
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

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        foreach (var item in Elements)
        {
            var expanded = item.ToString(expanded: true);
            var reduced = item.ToString();
            var num = 0;
        }
    }
}