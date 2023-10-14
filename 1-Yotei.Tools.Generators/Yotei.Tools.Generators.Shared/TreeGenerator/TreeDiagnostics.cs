namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class TreeDiagnostics
{
    /// <summary>
    /// Reports an error when the type is not a partial one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorTypeNotPartial(
        this SourceProductionContext context,
        ITypeSymbol symbol)
    {
        var id = "TreeGen01";
        var head = "Type is not partial.";
        var desc = $"The type '{symbol.Name}' is not a partial one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports an error when the type is not of a supported kind.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorTypeNotSupported(
        this SourceProductionContext context,
        ITypeSymbol symbol)
    {
        var id = "TreeGen02";
        var head = "Kind of type is not supported.";
        var desc = $"The kind of type '{symbol.Name}' is not supported in this context.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports an error when the type is a record.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorTypeIsRecord(
        this SourceProductionContext context,
        ITypeSymbol symbol)
    {
        var id = "TreeGen03";
        var head = "Type is a record.";
        var desc = $"The type '{symbol.Name}' is a record, which is not supported in this context.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}