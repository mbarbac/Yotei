namespace Yotei.Tools.Generators;

// ========================================================
public static class TreeErrorExtensions
{
    /// <summary>
    /// Returns a diagnostic for the given error associated with the given syntax node. If no
    /// message is given, then the default error one is used.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="syntax"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic Create(
        this TreeError error,
        SyntaxNode syntax, string? message, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = $"Yotei{error.Value:D2}";
        var loc = syntax.GetLocation();
        var name = syntax.ToNodeName();
        message ??= $"{error.Description}: {name}";

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, error.Description, message, "Yotei",
            severity, isEnabledByDefault: true),
            loc);
    }

    /// <summary>
    /// Returns a diagnostic for the given error associated with the given syntax node. If no
    /// location is given, then a default one for the symbol is used. Similarly, if no message
    /// is given, then the default error one is used.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="symbol"></param>
    /// <param name="location"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic Create(
        this TreeError error,
        ISymbol symbol, Location? location = null,
        string? message = null, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = $"Yotei{error.Value:D2}";
        location ??= symbol.FirstLocation;
        message ??= $"{error.Description}: {symbol.Name}";

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, error.Description, message, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}