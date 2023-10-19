namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// Reports an error when the property has no getter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorPropertyNoGetter(
        this SourceProductionContext context,
        IPropertySymbol symbol)
    {
        var id = "WithGen01";
        var head = "Property has no getter.";
        var desc = "The property '{0}' has no getter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault(),
            new object[] { symbol.Name }));
    }

    /// <summary>
    /// Reports an error when the property has no setter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorPropertyNoSetter(
        this SourceProductionContext context,
        IPropertySymbol symbol)
    {
        var id = "WithGen02";
        var head = "Property has no setter.";
        var desc = "The property '{0}' has no setter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault(),
            new object[] { symbol.Name }));
    }

    /// <summary>
    /// Reports an error when the field is not writable.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    public static void ErrorFieldNotWritable(
        this SourceProductionContext context,
        IFieldSymbol symbol)
    {
        var id = "WithGen03";
        var head = "Field is not writable.";
        var desc = "The field '{0}' is not writable.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", DiagnosticSeverity.Error, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault(),
            new object[] { symbol.Name }));
    }
}