namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class DiagnosticExtensions
{
    extension(Diagnostic diagnostic)
    {
        /// <summary>
        /// Reports this diagnostic in the given source production context.
        /// </summary>
        /// <param name="context"></param>
        public void Report(
            SourceProductionContext context) => context.ReportDiagnostic(diagnostic);
    }
}