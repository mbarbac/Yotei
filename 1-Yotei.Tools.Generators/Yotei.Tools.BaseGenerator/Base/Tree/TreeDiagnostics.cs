namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class TreeDiagnostics
{
    /// <summary>
    /// Reports the given diagnostic in the given production context.
    /// </summary>
    /// <param name="diagnostic"></param>
    /// <param name="context"></param>
    public static void Report(this Diagnostic diagnostic, SourceProductionContext context)
    {
        diagnostic.ThrowWhenNull();
        context.ThrowWhenNull();

        context.ReportDiagnostic(diagnostic);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The syntax is not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic UnknownSyntax(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax is not supported.";
        var desc = $"Syntax is not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Cannot find a symbol associated to the given type syntax.";
        var desc = $"Cannot find a symbol associated to the given type syntax: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        PropertyDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Cannot find a symbol associated to the given property syntax.";
        var desc = $"Cannot find a symbol associated to the given property syntax: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        FieldDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Cannot find a symbol associated to the given field syntax.";
        var desc = $"Cannot find a symbol associated to the given field syntax: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        MethodDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Cannot find a symbol associated to the given method syntax.";
        var desc = $"Cannot find a symbol associated to the given method syntax: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// A candidate is not consistent with an existing node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="candidate"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InconsistentHierarchy(
        INode node,
        IValidCandidate candidate,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "A candidate is not consistent with an existing node.";
        var desc = $"The candidate '{candidate}' is not consistent with the node '{node}'.";
        var location = candidate.Syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Syntax kind is not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var text = syntax.GetText().ToString()
            .Replace('\r', ' ').Replace('\n', ' ')
            .Replace("  ", " ")
            .Trim();

        var id = "TreeGen04";
        var head = "Syntax kind is not supported.";
        var desc = $"Syntax kind is not supported: '{syntax.Kind()}', '{text}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Type kind is not supported.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Type kind is not supported.";
        var desc = $"Type kind is not supported: '{type.Name}'.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Type is not a partial.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TypeIsNotPartial(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Name}' is not partial.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Type has not a copy constructor.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoCopyConstructor(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Type has not a copy constructor.";
        var desc = $"Type '{type.Name}' has not a suitable copy constructor.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Property has no getter.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoGetter(
        IPropertySymbol item,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Property has no getter.";
        var desc = $"Property '{item.Name}' has no getter.";
        var location =
            item.Locations.FirstOrDefault() ??
            item.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Property has no setter.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoSetter(
        IPropertySymbol item,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Property has no setter.";
        var desc = $"Property '{item.Name}' has no setter.";
        var location =
            item.Locations.FirstOrDefault() ??
            item.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Field is not writtable.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NotWrittable(
        IFieldSymbol item,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Field is not writtable.";
        var desc = $"Field '{item.Name}' is not writtable.";
        var location =
            item.Locations.FirstOrDefault() ??
            item.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}