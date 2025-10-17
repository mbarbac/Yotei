namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class DiagnosticExtensions
{
    /// <summary>
    /// Reports the given diagnostic in the given production context.
    /// </summary>
    /// <param name="diagnostic"></param>
    /// <param name="context"></param>
    public static void Report(
        this Diagnostic diagnostic,
        SourceProductionContext context) => context.ReportDiagnostic(diagnostic.ThrowWhenNull());
}