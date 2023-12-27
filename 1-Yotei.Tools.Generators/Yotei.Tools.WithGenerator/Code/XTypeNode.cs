namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;
        if (!context.TypeIsNotRecord(Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> We use the fact this override is only invoked for types that are themselves
    /// decorated, and then we capture the inherited properties and fields for which there is no
    /// decoration in this type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder builder)
    {
        CaptureProperties();
        CaptureFields();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the inherited properties for which there is no decoration in this type.
    /// </summary>
    /// <returns></returns>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        void Capture(ITypeSymbol type)
        {
            var members = type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var index = PropertyChildren.IndexOf(x => x.Symbol.Name == member.Name);
                if (index >= 0) continue;
                if (FindDecorated(Symbol, member) != null) continue;
                if (FindMethod(Symbol, member) != null) continue;

                var node = new XPropertyNode(this, member);
                PropertyChildren.Add(node);
            }
        }

        static IPropertySymbol? FindDecorated(ITypeSymbol type, IPropertySymbol member)
        {
            return type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(x =>
                    x.Name == member.Name &&
                    x.HasAttributes(WithGeneratorAttr.LongName));
        }

        static IMethodSymbol? FindMethod(ITypeSymbol type, IPropertySymbol member)
        {
            var name = "With" + member.Name;

            return type
                .GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(x =>
                    x.Name == name &&
                    x.Parameters.Length == 1 &&
                    member.Type.IsAssignableTo(x.Parameters[0].Type));
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the inherited fields for which there is no decoration in this type.
    /// </summary>
    /// <returns></returns>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        void Capture(ITypeSymbol type)
        {
            var members = type
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var index = FieldChildren.IndexOf(x => x.Symbol.Name == member.Name);
                if (index >= 0) continue;
                if (FindDecorated(Symbol, member) != null) continue;
                if (FindMethod(Symbol, member) != null) continue;

                var node = new XFieldNode(this, member);
                FieldChildren.Add(node);
            }
        }

        static IFieldSymbol? FindDecorated(ITypeSymbol type, IFieldSymbol member)
        {
            return type
                .GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(x =>
                    x.Name == member.Name &&
                    x.HasAttributes(WithGeneratorAttr.LongName));
        }

        static IMethodSymbol? FindMethod(ITypeSymbol type, IFieldSymbol member)
        {
            var name = "With" + member.Name;

            return type
                .GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(x =>
                    x.Name == name &&
                    x.Parameters.Length == 1 &&
                    member.Type.IsAssignableTo(x.Parameters[0].Type));
        }
    }
}