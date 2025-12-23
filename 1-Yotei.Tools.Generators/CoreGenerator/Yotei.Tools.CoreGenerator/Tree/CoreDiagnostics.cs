namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class CoreDiagnostics
{
    /// <summary>
    /// Syntax not supported
    /// </summary>
    public static Diagnostic SyntaxNotSupported(
        this SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax not supported";
        message ??= $"Syntax not supported: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Syntax not supported
    /// </summary>
    public static Diagnostic SyntaxNotSupported(
        this ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax not supported";
        message ??= $"Syntax not supported: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    // TreeGen02: SymbolNotFound
    // TreeGen03: SytaxNotFound

    // ----------------------------------------------------

    /// <summary>
    /// No attributes found
    /// </summary>
    public static Diagnostic NoAttributes(
        this SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "No attributes found";
        message ??= $"No attributes found for: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// No attributes found
    /// </summary>
    public static Diagnostic NoAttributes(
        this ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "No attributes found";
        message ??= $"No attributes found for: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Too many attributes
    /// </summary>
    public static Diagnostic TooManyAttributes(
        this SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Too many attributes found";
        message ??= $"Too many attributes found for: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Too many attributes
    /// </summary>
    public static Diagnostic TooManyAttributes(
        this ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Too many attributes found";
        message ??= $"Too many attributes found for: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invalid attribute
    /// </summary>
    public static Diagnostic InvalidAttribute(
        this SyntaxNode syntax,
        AttributeSyntax attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Invalid attribute";
        message ??= $"Invalid attribute '{attribute.Name}' at: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Invalid attribute
    /// </summary>
    public static Diagnostic InvalidAttribute(
        this ISymbol symbol,
        AttributeData attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Invalid attribute";
        message ??= $"Invalid attribute '{attribute.AttributeClass?.Name}' at: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Kind not supported
    /// </summary>
    public static Diagnostic KindNotSupported(
        this SyntaxNode syntax,
        AttributeSyntax attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Kind not supported";
        message ??= $"Kind not supported for: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Kind not supported
    /// </summary>
    public static Diagnostic KindNotSupported(
        this ISymbol symbol,
        AttributeData attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Kind not supported";
        message ??= $"Kind not supported for: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Records not supported
    /// </summary>
    public static Diagnostic RecordsNotSupported(
        this SyntaxNode syntax,
        AttributeSyntax attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Records not supported";
        message ??= $"Records not supported: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Records not supported
    /// </summary>
    public static Diagnostic RecordsNotSupported(
        this ISymbol symbol,
        AttributeData attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Records not supported";
        message ??= $"Records not supported: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Type is not partial
    /// </summary>
    public static Diagnostic TypeNotPartial(
        this SyntaxNode syntax,
        AttributeSyntax attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Type is not partial";
        message ??= $"Type is not partial: {syntax}";

        var location = syntax.GetLocation();
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Type is not partial
    /// </summary>
    public static Diagnostic TypeNotPartial(
        this ISymbol symbol,
        AttributeData attribute,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Type is not partial";
        message ??= $"Type is not partial: {symbol.Name}";

        var location = symbol.FirstLocation;
        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}