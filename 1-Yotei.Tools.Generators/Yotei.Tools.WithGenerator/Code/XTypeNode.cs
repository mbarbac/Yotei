using Microsoft.CodeAnalysis.FindSymbols;

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    public XTypeNode(TypeCandidate candidate) : base(candidate) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!Symbol.TypeIsNotRecord(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// Just before type is emitted, we capture the inherited members whose implementation is
    /// delegated to this host type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        var num = 0;

        var properties = CaptureProperties();
        foreach (var property in properties)
        {
            if (!property.Validate(context)) continue;

            if (num > 0) file.AppendLine();
            property.Print(context, file);
            num++;
        }

        var fields = CaptureFields();
        foreach (var field in fields)
        {
            if (!field.Validate(context)) continue;

            if (num > 0) file.AppendLine();
            field.Print(context, file);
            num++;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the inherited properties for which there are no explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<XPropertyNode> CaptureProperties()
    {
        var list = new List<XPropertyNode>();

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);
        return list;
        
        // Recursive...
        void Capture(ITypeSymbol type)
        {
            var members = type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var done = list.Find(x => x.Symbol.Name == member.Name);
                if (done != null) continue;

                if (FindDecorated(Symbol, member) != null) continue;
                if (FindMethod(Symbol, member) != null) continue;

                var node = new XPropertyNode(member) { HostType = Symbol };
                list.Add(node);
            }
        }

        // Finds the decorated memeber associated with the member, or null...
        static IPropertySymbol? FindDecorated(ITypeSymbol type, IPropertySymbol member)
        {
            return type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(x =>
                    x.Name == member.Name &&
                    x.HasAttributes(WithGeneratorAttr.LongName));
        }

        // Finds the method associated with the member, or null...
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
    /// Captures the inherited fields for which there are no explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<XFieldNode> CaptureFields()
    {
        var list = new List<XFieldNode>();

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);
        return list;

        // Recursive...
        void Capture(ITypeSymbol type)
        {
            var members = type
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var done = list.Find(x => x.Symbol.Name == member.Name);
                if (done != null) continue;

                if (FindDecorated(Symbol, member) != null) continue;
                if (FindMethod(Symbol, member) != null) continue;

                var node = new XFieldNode(member) { HostType = Symbol };
                list.Add(node);
            }
        }

        // Finds the decorated memeber associated with the member, or null...
        static IFieldSymbol? FindDecorated(ITypeSymbol type, IFieldSymbol member)
        {
            return type
                .GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(x =>
                    x.Name == member.Name &&
                    x.HasAttributes(WithGeneratorAttr.LongName));
        }

        // Finds the method associated with the member, or null...
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