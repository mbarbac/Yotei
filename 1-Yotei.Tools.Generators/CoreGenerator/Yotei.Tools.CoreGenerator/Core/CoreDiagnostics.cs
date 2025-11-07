namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class CoreDiagnostics
{
    /// <summary>
    /// Gets the syntax kind descriptor for error reporting purposes.
    /// </summary>
    static string ToKindString(this SyntaxNode syntax) => syntax switch
    {
        TypeDeclarationSyntax => "type",
        PropertyDeclarationSyntax => "property",
        FieldDeclarationSyntax => "field",
        MethodDeclarationSyntax => "method",
        _ => "syntax"
    };

    // ----------------------------------------------------

    /// <summary>
    /// Candidate node is not consistent with captured hierarchy.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidHierarchy(
        IValidCandidate node,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen00";
        var head = "Candidate node is not consistent with captured hierarchy.";
        message ??= $"Candidate node '{node}' is not consistent with captured hierarchy.";
        var location =
            node.Syntax?.GetLocation() ??
            node.Symbol.Locations.FirstOrDefault() ??
            node.Symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }// ----------------------------------------------------

    /// <summary>
    /// Syntax not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SyntaxNotSupported(
        SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax not supported.";
        message ??= $"Syntax not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Syntax not supported for symbol.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SyntaxNotSupported(
        ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax not supported for symbol.";
        message ??= $"Syntax not supported for symbol: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// Symbol not found for syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Symbol not found for syntax.";
        message ??= $"Symbol not found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// No attributes found.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoAttributes(
        SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "No attributes found.";
        message ??= $"No attributes found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// No attributes found.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoAttributes(
        ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "No attributes found.";
        message ??= $"No attributes found for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Too many attributes found.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TooManyAttributes(
        SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Too many attributes found.";
        message ??= $"Too many attributes found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Too many attributes found.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TooManyAttributes(
        ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Too many attributes found.";
        message ??= $"Too many attributes found for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invalid attribute.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidAttribute(
        SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Invalid attribute.";
        message ??= $"Invalid attribute found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Invalid attribute.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidAttribute(
        ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Invalid attribute.";
        message ??= $"Invalid attribute found for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Kind is not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        SyntaxNode syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Kind is not supported.";
        message ??= $"Kind is not supported for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Kind is not supported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        ISymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Kind is not supported.";
        message ??= $"Kind is not supported for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Records not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        TypeDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Records not supported.";
        message ??= $"Records not supported for: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Records not supported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        ITypeSymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Records not supported.";
        message ??= $"Records not supported for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Type is not a partial.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TypeNotPartial(
        TypeDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Type is not a partial.";
        message ??= $"Type is not a partial: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Type is not a partial.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TypeNotPartial(
        ITypeSymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Type is not a partial.";
        message ??= $"Type is not a partial: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Type has not a copy constructor.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoCopyConstructor(
        TypeDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Type has not a copy constructor.";
        message ??= $"Type has not a copy constructor: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Type has not a copy constructor.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoCopyConstructor(
        ITypeSymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Type has not a copy constructor.";
        message ??= $"Type has not a copy constructor: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Property has not a suitable getter.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoGetter(
        PropertyDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen10";
        var head = "Property has not a suitable getter.";
        message ??= $"Property has not a suitable getter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Property has not a suitable getter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoGetter(
        IPropertySymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen10";
        var head = "Property has not a suitable getter.";
        message ??= $"Property has not a suitable getter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Property has an invalid getter.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidGetter(
        PropertyDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen11";
        var head = "Property has an invalid getter.";
        message ??= $"Property has an invalid getter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Property has an invalid getter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidGetter(
        IPropertySymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen11";
        var head = "Property has an invalid getter.";
        message ??= $"Property has an invalid getter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Property has not a suitable setter.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoSetter(
        PropertyDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen12";
        var head = "Property has not a suitable setter.";
        message ??= $"Property has not a suitable setter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Property has not a suitable setter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoSetter(
        IPropertySymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen12";
        var head = "Property has not a suitable setter.";
        message ??= $"Property has not a suitable setter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Property has an invalid setter.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidSetter(
        PropertyDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen13";
        var head = "Property has an invalid setter.";
        message ??= $"Property has an invalid setter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Property has an invalid setter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidSetter(
        IPropertySymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen13";
        var head = "Property has an invalid setter.";
        message ??= $"Property has an invalid setter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Indexed properties are not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic IndexerNotSupported(
        PropertyDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen14";
        var head = "Indexed properties are not supported.";
        message ??= $"Indexed properties are not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Indexed properties are not supported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic IndexerNotSupported(
        IPropertySymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen14";
        var head = "Indexed properties are not supported.";
        message ??= $"Indexed properties are not supported: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Field is not writtable.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NotWrittable(
        FieldDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen15";
        var head = "Field is not writtable.";
        message ??= $"Field is not writtable: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Field is not writtable.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NotWrittable(
        IFieldSymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen15";
        var head = "Field is not writtable.";
        message ??= $"Field is not writtable: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Method is invalid.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidMethod(
        FieldDeclarationSyntax syntax,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen16";
        var head = "Method is invalid.";
        message ??= $"Method is invalid: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Method is invalid.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidMethod(
        IFieldSymbol symbol,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen16";
        var head = "Method is invalid.";
        message ??= $"Method is invalid: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Return type is invalid.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidReturnType(
        ISymbol symbol,
        ITypeSymbol type,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen17";
        var head = "Return type is invalid.";
        message ??= $"Symbol '{symbol.Name}' has an invalid return type: '{type.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}