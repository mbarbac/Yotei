namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class TypeBuilderDiagnostics
{
    /// <summary>
    /// Reports a diagnostic when the given type symbol is a namespace.
    /// This can be an error because it depends only on the generator developer.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void TypeIsNamespace(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen01";
        var head = "Type is a namespace.";
        var desc = $"The symbol for type '{symbol.Name}' is a namespace.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when the type is unnamed.
    /// This can be an error because it depends only on the generator developer.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void TypeNotNamed(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen02";
        var head = "Type is not named.";
        var desc = $"The type '{symbol.MetadataName}' is not a named one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when the code for the associated symbol cannot be generated for the
    /// given specs.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void CannotGenerateCode(
        this SourceProductionContext context,
        ISymbol symbol,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen03";
        var head = "Cannot generate code.";
        var desc = $"Cannot generate code for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when no builders are found for the given specs.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void NoBuildersFound(
        this SourceProductionContext context,
        ISymbol symbol,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen04";
        var head = "No builders found.";
        var desc = $"No builders found for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when there is no enforced element
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void NoEnforcedElement(
        this SourceProductionContext context,
        ISymbol symbol,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen05";
        var head = "No enforced member.";
        var desc = $"No enforced member given for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when the enforced element is not used.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="enforced"></param>
    /// <param name="severity"></param>
    public static void EnforcedNotUsed(
        this SourceProductionContext context,
        ISymbol symbol,
        ISymbol enforced,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen06";
        var head = "Enforced member not used.";
        var desc = $"Enforced member '{enforced.Name}' not used at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when there is an ambiguous match for a required element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    /// <param name="severity"></param>
    public static void AmbiguousMatch(
        this SourceProductionContext context,
        ISymbol symbol,
        string name,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen07";
        var head = "Ambiguous match.";
        var desc = $"Ambiguous match for '{name}' at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when there is no match for a required element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    public static void NoMatch(
        this SourceProductionContext context,
        ISymbol symbol,
        string name,
        DiagnosticSeverity severity)
    {
        var id = "TypeGen08";
        var head = "No match.";
        var desc = $"No match for '{name}' at '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}