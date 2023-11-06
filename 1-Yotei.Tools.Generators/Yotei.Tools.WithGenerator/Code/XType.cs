namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XType : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    public XType(TypeCandidate candidate) : base(candidate) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XType(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!context.TypeIsNotRecord(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        var properties = CaptureProperties();
        var fields = CaptureFields();
        var num = 0;

        foreach (var property in properties)
        {
            if (!property.Validate(context)) continue;

            if (num > 0) file.AppendLine();
            property.Print(context, file);
            num++;
        }

        foreach (var field in fields)
        {
            if (!field.Validate(context)) continue;

            if (num > 0) file.AppendLine();
            field.Print(context, file);
            num++;
        }
    }

    /// <summary>
    /// Captures the inherited properties for which there is no implementation.
    /// </summary>
    /// <returns></returns>
    List<XProperty> CaptureProperties()
    {
        var list = new List<XProperty>();
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);
        return list;

        // Recursive...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                // Already captured...
                var found = list.Find(x => x.Symbol.Name == member.Name);
                if (found != null) continue;

                // Already implemented...
                if (FindMethod(Symbol, member) != null) continue;

                // Already decorated...
                if (FindDecorated(Symbol, member) != null) continue;

                // Capturing...
                var node = new XProperty(member) { HostType = Symbol };
                list.Add(node);
            }
        }

        // Determines if the type implements a method for the given member...
        static IMethodSymbol? FindMethod(ITypeSymbol type, IPropertySymbol member)
        {
            var name = "With" + member.Name;
            return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == name &&
                x.Parameters.Length == 1 &&
                member.Type.IsAssignableTo(x.Parameters[0].Type));
        }

        // Determines if the type has a decorated member with the given name...
        static IPropertySymbol? FindDecorated(ITypeSymbol type, IPropertySymbol member)
        {
            return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == member.Name &&
                x.HasAttributes(WithGeneratorAttr.LongName));
        }
    }

    /// <summary>
    /// Captures the inherited properties for which there is no implementation.
    /// </summary>
    /// <returns></returns>
    List<XField> CaptureFields()
    {
        var list = new List<XField>();
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);
        return list;

        // Recursive...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                // Already captured...
                var found = list.Find(x => x.Symbol.Name == member.Name);
                if (found != null) continue;

                // Already implemented...
                if (FindMethod(Symbol, member) != null) continue;

                // Already decorated...
                if (FindDecorated(Symbol, member) != null) continue;

                // Capturing...
                var node = new XField(member) { HostType = Symbol };
                list.Add(node);
            }
        }

        // Determines if the type implements a method for the given member...
        static IMethodSymbol? FindMethod(ITypeSymbol type, IFieldSymbol member)
        {
            var name = "With" + member.Name;
            return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == name &&
                x.Parameters.Length == 1 &&
                member.Type.IsAssignableTo(x.Parameters[0].Type));
        }

        // Determines if the type has a decorated member with the given name...
        static IPropertySymbol? FindDecorated(ITypeSymbol type, IFieldSymbol member)
        {
            return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == member.Name &&
                x.HasAttributes(WithGeneratorAttr.LongName));
        }
    }
}