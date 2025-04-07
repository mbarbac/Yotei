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

    // Factorizes common code.
    private static Diagnostic CreateSymbolNotFoundForSyntax(
        SyntaxNode syntax,
        string type,
        DiagnosticSeverity severity)
    {
        var id = "TreeGen02";
        var head = $"Cannot find a symbol associated to the given {type} syntax.";
        var desc = $"Cannot find a symbol associated to the given {type} syntax: '{syntax}'.";
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
    public static Diagnostic SymbolNotFoundForSyntax(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFoundForSyntax(syntax, "type", severity);

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFoundForSyntax(
        PropertyDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFoundForSyntax(syntax, "property", severity);

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFoundForSyntax(
        FieldDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFoundForSyntax(syntax, "field", severity);

    /// <summary>
    /// Cannot find a symbol associated to the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFoundForSyntax(
        MethodDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFoundForSyntax(syntax, "method", severity);

    // ----------------------------------------------------

    // Factorizes common code.
    private static Diagnostic CreateSymbolNotFound(
        ISymbol symbol,
        string type,
        DiagnosticSeverity severity)
    {
        var id = "TreeGen02";
        var head = $"{type} symbol not found.";
        var desc = $"{type} symbol not found: '{symbol.Name}'.";

        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFound(symbol, "Type", severity);

    /// <summary>
    /// Cannot find the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        IPropertySymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFound(symbol, "Property", severity);

    /// <summary>
    /// Cannot find the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        IFieldSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFound(symbol, "Field", severity);

    /// <summary>
    /// Cannot find the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        IMethodSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
        => CreateSymbolNotFound(symbol, "Method", severity);

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

    // ----------------------------------------------------

    /// <summary>
    /// Records are not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var text = syntax.GetText().ToString()
            .Replace('\r', ' ').Replace('\n', ' ')
            .Replace("  ", " ")
            .Trim();

        var id = "TreeGen05";
        var head = "Records are not supported.";
        var desc = $"Records are not supported: '{syntax.Kind()}', '{text}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Records are not supported.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Records are not supported.";
        var desc = $"Records not supported: '{type.Name}'.";
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
        var id = "TreeGen06";
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
        var id = "TreeGen07";
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
        var id = "TreeGen08";
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
        var id = "TreeGen09";
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
        var id = "TreeGen10";
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

    // ----------------------------------------------------

    /// <summary>
    /// Indexed properties are not supported.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic IndexerNotSupported(
        IPropertySymbol item,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen11";
        var head = "Indexed properties are not supported.";
        var desc = $"Indexed properties are not supported: '{item.Name}'.";
        var location =
            item.Locations.FirstOrDefault() ??
            item.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}