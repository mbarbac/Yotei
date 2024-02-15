namespace Yotei.Tools.InheritGenerator;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// The type to inherit from is null.
    /// </summary>
    public static void TypeArgumentNull(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Inherit01";
        var head = "Type to inherit from is null";
        var desc = $"Type to inherit from is null in '{type.Name}'";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// There are not enough generic names.
    /// </summary>
    public static void NotEnoughGenericNames(
        this SourceProductionContext context,
        ITypeSymbol type,
        ITypeSymbol inherit,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Inherit02";
        var head = "Not enough generic names";
        var desc = $"Not enough generic names for inherited type '{inherit.Name}'";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));
    }
}