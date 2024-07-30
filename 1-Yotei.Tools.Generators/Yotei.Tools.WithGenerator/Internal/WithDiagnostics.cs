namespace Yotei.Tools.WithGenerator;

// =========================================================
internal static class WithDiagnostics
{
    /// <summary>
    /// Records not supported.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic RecordsNotSupported(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "WithGen01";
        var head = "Records not supported.";
        var desc = $"Type '{type.Name}' is a record, which are not supported.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Indexer not supported.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic IndexerNotSupported(
        IPropertySymbol item,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "WithGen02";
        var head = "Indexers are not supported.";
        var desc = $"Property '{item.Name}' is an indexer, which are not supported.";
        var location =
            item.Locations.FirstOrDefault() ??
            item.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Member not found in interface.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="iface"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic MemberNotFoundInInterface(
        IPropertySymbol item,
        ITypeSymbol iface,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "WithGen03";
        var head = "Member not found in interface.";
        var desc = $"Member '{item.Name}' is not found in interface '{iface.Name}'.";
        var location =
            item.Locations.FirstOrDefault() ??
            item.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}