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
}