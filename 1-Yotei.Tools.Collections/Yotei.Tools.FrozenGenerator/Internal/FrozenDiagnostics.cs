namespace Yotei.Tools.FrozenGenerator;

// =========================================================
internal static class FrozenDiagnostics
{
    /// <summary>
    /// No 'Frozen' attribute found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic NoAttributes(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Frozen01";
        var head = "No 'Frozen' attribute found.";
        var desc = $"No 'Frozen' attribute found in: '{type.Name}'.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// More than one 'Frozen' attribute found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic ManyAttributes(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Frozen02";
        var head = "More than one 'Frozen' attribute found.";
        var desc = $"More than one 'Frozen' attribute applied to: '{type.Name}'.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// The 'Frozen' attribute found is an invalid one.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidAttribute(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Frozen03";
        var head = "The 'Frozen' attribute found is an invalid one.";
        var desc = $"The 'Frozen' attribute found in: '{type.Name}' is an invalid one.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}