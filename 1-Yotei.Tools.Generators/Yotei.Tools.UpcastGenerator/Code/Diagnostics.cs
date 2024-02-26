namespace Yotei.Tools.UpcastGenerator;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// Reports a diagnostic when there are no elements to inherit from.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    public static void NoInheritedElements(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Upcast01";
        var head = "No elements to inherit from.";
        var desc = $"No elements to inherit from for type '{type.Identifier.Text}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (severity == DiagnosticSeverity.Error) throw new ArgumentException(desc);
    }
}