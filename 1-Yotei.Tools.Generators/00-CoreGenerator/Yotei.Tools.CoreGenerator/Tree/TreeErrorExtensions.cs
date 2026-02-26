namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TreeErrorExtensions
{
    /// <summary>
    /// Creates a diagnostic for the given error at the location associated with the given element.
    /// If no message is provided, the default one for the error is used.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="error"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic CreateError(
        this SyntaxNode syntax,
        TreeError error,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = $"Yotei{error.Value:D2}";
        var loc = syntax.GetLocation();
        message ??= $"{error.Description}: {syntax}";

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, error.Description, message, "Yotei",
            severity, isEnabledByDefault: true),
            loc);
    }

    /// <summary>
    /// Reports a diagnostic created for the given error at the location associated with the given
    /// element, on the given source production context. If no message is provided, the default one
    /// for the error is used.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="error"></param>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic ReportError(
        this SyntaxNode syntax,
        TreeError error,
        SourceProductionContext context,
        string? message = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var diag = syntax.CreateError(error, message, severity);
        context.ReportDiagnostic(diag);
        return diag;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Creates a diagnostic for the given error at the first location associated with the given
    /// element, or at the given one. If no message is provided, the default one for the error is
    /// used.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="error"></param>
    /// <param name="message"></param>
    /// <param name="location"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic CreateError(
        this ISymbol symbol,
        TreeError error,
        string? message = null,
        Location? location = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = $"Yotei{error.Value:D2}";
        location ??= symbol.FirstLocation;
        message ??= $"{error.Description}: {symbol.Name}";

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, error.Description, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Reports a diagnostic created for the given error at the first location associated with the
    /// given element, or at the given one, on the given source production context. If no message
    /// is provided, the default one for the error is used.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="error"></param>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="location"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic ReportError(
        this ISymbol symbol,
        TreeError error,
        SourceProductionContext context,
        string? message = null,
        Location? location = null,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var diag = symbol.CreateError(error, message, location, severity);
        context.ReportDiagnostic(diag);
        return diag;
    }
}