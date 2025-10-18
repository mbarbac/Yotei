namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TreeDiagnostics
{
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
    /// A candidate is not consistent with an existing node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="candidate"></param>
    /// <param name="location"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidHierarchy(
        INode node,
        IValidCandidate candidate,
        Location location,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen00";
        var head = "A candidate is not consistent with an existing node";
        var desc = $"Candidate '{candidate}' is not consistent with node '{node}'.";

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Syntax is not not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SyntaxNotSupported(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax not supported";
        var desc = $"Syntax not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The symbol that correspond to the given syntax was not found.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Symbol not found";
        var desc = $"Symbol not found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// No valid attributes found decorating the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoAttributes(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "No attributes found";
        var desc = $"No attributes found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// No valid attributes found decorating the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoAttributes(
        ISymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "No attributes found";
        var desc = $"No attributes found for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Too many attributes found decorating the given element.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TooManyAttributes(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Too many attributes found";
        var desc = $"Too many attributes found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Too many attributes found decorating the given element.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TooManyAttributes(
        ISymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Too many attributes found";
        var desc = $"Too many attributes found for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// An attribute that decorates the given element is invalid.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidAttribute(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Invalid attribute";
        var desc = $"Invalid attribute found for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// An attribute that decorates the given element is invalid.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidAttribute(
        ISymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Invalid attribute";
        var desc = $"Invalid attribute found for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Element kind is not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Element kind is not supported";
        var desc = $"Element kind is not supported for {syntax.ToKindString()}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Element kind is not supported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        ISymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "Element kind is not supported";
        var desc = $"Element kind is not supported for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Record types are not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Record types are not supported";
        var desc = $"Record types are not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Record types are not supported
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Record types are not supported";
        var desc = $"Record types are not supported for: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Type is not a partial one.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TypeNotPartial(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Type is not a partial one";
        var desc = $"Type is not a partial one: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Record types are not supported
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TypeNotPartial(
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Type is not a partial one";
        var desc = $"Type is not a partial one: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Type has not a suitable copy constructor.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoCopyConstructor(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Type has not a suitable copy constructor";
        var desc = $"Type has not a suitable copy constructor: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Record types are not supported
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoCopyConstructor(
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Type has not a suitable copy constructor";
        var desc = $"Type has not a suitable copy constructor: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen10";
        var head = "Property has not a suitable getter";
        var desc = $"Property has not a suitable getter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen10";
        var head = "Property has not a suitable getter";
        var desc = $"Property has not a suitable getter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen11";
        var head = "Property has an invalid getter";
        var desc = $"Property has an invalid getter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen11";
        var head = "Property has an invalid getter";
        var desc = $"Property has an invalid getter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen12";
        var head = "Property has not a suitable setter";
        var desc = $"Property has not a suitable setter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen12";
        var head = "Property has not a suitable setter";
        var desc = $"Property has not a suitable setter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen13";
        var head = "Property has an invalid setter";
        var desc = $"Property has an invalid setter: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen13";
        var head = "Property has an invalid setter";
        var desc = $"Property has an invalid setter: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen14";
        var head = "Indexed properties are not supported";
        var desc = $"Indexed properties are not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen14";
        var head = "Indexed properties are not supported";
        var desc = $"Indexed properties are not supported: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen15";
        var head = "Field is not writtable";
        var desc = $"Field is not writtable: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
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
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen15";
        var head = "Field is not writtable";
        var desc = $"Field is not writtable: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Method is an invalid one.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidMethod(
        FieldDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen16";
        var head = "Method is an invalid one";
        var desc = $"Method is an invalid one: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Method is an invalid one.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidMethod(
        IFieldSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen16";
        var head = "Method is an invalid one";
        var desc = $"Method is an invalid one: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Return type is an invalid one.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidReturnType(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen17";
        var head = "Return type is an invalid one";
        var desc = $"Return type is an invalid one: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Record types are not supported
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidReturnType(
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen17";
        var head = "Return type is an invalid one";
        var desc = $"Return type is an invalid one: '{symbol.Name}'.";
        var location =
            symbol.Locations.FirstOrDefault() ??
            symbol.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}