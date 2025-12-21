namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class DiagnosticExtensions
{
    /// <summary>
    /// Reports this diagnostic on the given context.
    /// </summary>
    /// <param name="diagnostic"></param>
    /// <param name="context"></param>
    public static void Report(
        this Diagnostic diagnostic, SourceProductionContext context)
        => context.ReportDiagnostic(diagnostic.ThrowWhenNull());
}