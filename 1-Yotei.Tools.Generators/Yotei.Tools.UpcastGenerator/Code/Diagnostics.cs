namespace Yotei.Tools.UpcastGenerator;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// Reports that the syntax is not associated with an inheritance list.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoInheritanceList(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Upcast01";
        var head = "No inheritance list.";
        var desc = $"Type '{type.Identifier.Text}' has not an inheritance list.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }

    /// <summary>
    /// Reports that no inherited types could be captured.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoInheritedTypes(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Upcast01";
        var head = "No inherited types captured.";
        var desc = $"No inherited types could be found for '{type.Identifier.Text}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }
}