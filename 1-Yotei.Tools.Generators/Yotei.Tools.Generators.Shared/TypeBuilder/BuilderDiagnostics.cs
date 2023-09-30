namespace Yotei.Tools.Generators;

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
        var desc = "The symbol for type '{0}' is a namespace.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault(),
            new object[] { symbol.Name }));
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
        var desc = "The type '{0}' is not a named one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault(),
            new object[] { symbol.Name }));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports an error when the type is unnamed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void WarningCannotGenerateCode(
        this SourceProductionContext context,
        ISymbol symbol)
    {
        var id = "TypeGen03";
        var head = "Cannot generate the requested code.";
        var desc = "Cannot generate code for '{0}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Warning, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault(),
            new object[] { symbol.Name }));
    }
}