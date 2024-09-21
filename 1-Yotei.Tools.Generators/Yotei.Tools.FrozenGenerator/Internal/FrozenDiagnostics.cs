namespace Yotei.Tools.FrozenGenerator;

// =========================================================
internal static class FrozenDiagnostics
{
    /// <summary>
    /// Reports that no 'IFrozen' attribute is found.
    /// Returns false.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool NoAttributes(
        this SourceProductionContext context,
        ITypeSymbol type)
    {
        var id = "Frozen01";
        var head = "No 'Frozen' attribute found.";
        var desc = $"No 'Frozen' attribute applied to: '{type.Name}'.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            DiagnosticSeverity.Error, isEnabledByDefault: true),
            location);

        context.ReportDiagnostic(diagnostic);
        return false;
    }

    /// <summary>
    /// Reports that more than one 'IFrozen' attribute found.
    /// Returns false.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool ManyAttributes(
        this SourceProductionContext context,
        ITypeSymbol type)
    {
        var id = "Frozen02";
        var head = "More than one 'Frozen' attribute found.";
        var desc = $"More than one 'Frozen' attribute applied to: '{type.Name}'.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            DiagnosticSeverity.Error, isEnabledByDefault: true),
            location);

        context.ReportDiagnostic(diagnostic);
        return false;
    }

    /// <summary>
    /// Reports that the 'IFrozen' attribute found is an invalid one.
    /// Returns false.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool InvalidAttribute(
        this SourceProductionContext context,
        ITypeSymbol type)
    {
        var id = "Frozen03";
        var head = "The 'Frozen' attribute found is an invalid one..";
        var desc = $"The 'Frozen' attribute applied to: '{type.Name}' is an invalid one.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            DiagnosticSeverity.Error, isEnabledByDefault: true),
            location);

        context.ReportDiagnostic(diagnostic);
        return false;
    }
}