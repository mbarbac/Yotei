namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class BuilderDiagnostics
{
    /// <summary>
    /// Reports an error when the given type symbol is a namespace.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorTypeIsNamespace(
        this SourceProductionContext context,
        ITypeSymbol symbol)
    {
        var id = "TypeGen01";
        var head = "Type is a namespace.";
        var desc = $"The symbol for type '{symbol.Name}' is a namespace.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports an error when the type is unnamed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorTypeNotNamed(
        this SourceProductionContext context,
        ITypeSymbol symbol)
    {
        var id = "TypeGen02";
        var head = "Type is not named.";
        var desc = $"The type '{symbol.MetadataName}' is not a named one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a warning when the code for the associated symbol cannot be generated.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void WarningCannotGenerateCode(
        this SourceProductionContext context,
        ISymbol symbol)
    {
        var id = "TypeGen03";
        var head = "Cannot generate code.";
        var desc = $"Cannot generate code for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Warning, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports an error when there no builders can be found.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    public static void ErrorNoBuilders(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        string? name)
    {
        name ??= "<null>";

        var id = "TypeGen04";
        var head = "No builders found..";
        var desc = $"No builders found for name:'{name}' at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports an error when there is not a match for a required element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    public static void ErrorNoMatch(
        this SourceProductionContext context,
        ISymbol symbol,
        string name)
    {
        var id = "TypeGen05";
        var head = "No match found.";
        var desc = $"No match for '{name}' at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports an error when there is an ambiguous match for a required element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    public static void ErrorAmbiguousMatch(
        this SourceProductionContext context,
        ISymbol symbol,
        string name)
    {
        var id = "TypeGen06";
        var head = "Ambiguous match.";
        var desc = $"Ambiguous match for '{name}' at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a warning when there is an ambiguous match for a required element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    public static void WarningAmbiguousMatch(
        this SourceProductionContext context,
        ISymbol symbol,
        string name)
    {
        var id = "TypeGen07";
        var head = "Ambiguous match.";
        var desc = $"Ambiguous match for '{name}' at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Warning, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports an error when the enforced element was not used.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="typeSymbol"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    public static void ErrorEnforcedNotUsed(
        this SourceProductionContext context,
        ITypeSymbol typeSymbol,
        ISymbol symbol)
    {
        var id = "TypeGen08";
        var head = "Enforced not used.";
        var desc = $"Enforced '{symbol.Name}' not used at '{typeSymbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            typeSymbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports an error when the property is init-only.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorInitOnly(
        this SourceProductionContext context,
        ISymbol symbol)
    {
        var id = "TypeGen09";
        var head = "Property is init only.";
        var desc = $"The property '{symbol.Name}' is init only.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}