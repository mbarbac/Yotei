namespace Yotei.Tools.TreeGenerator;

// ========================================================
internal static class DiagnosticExtensions
{
    extension(Diagnostic source)
    {
        /// <summary>
        /// Reports this diagnostic in the given source production context.
        /// </summary>
        /// <param name="context"></param>
        public void Report(
            SourceProductionContext context) => context.ReportDiagnostic(source);
    }
}