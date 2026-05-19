namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Tree-related errors for diagnostics.
/// </summary>
/// <param name="Value"></param>
/// <param name="Description"></param>
public record TreeError(int Value, string Description)
{
    public static readonly TreeError SyntaxNotSupported = new(1, "Syntax not supported");
    public static readonly TreeError SymbolNotFound = new(2, "Symbol not found");
    public static readonly TreeError SyntaxNotFound = new(3, "Syntax not found");
    public static readonly TreeError NoAttributes = new(4, "No attributes");
    public static readonly TreeError TooManyAttributes = new(5, "Too many attributes");
    public static readonly TreeError InvalidAttribute = new(6, "Invalid attribute");
    public static readonly TreeError KindNotSupported = new(7, "Kind not supported");
    public static readonly TreeError RecordsNotSupported = new(8, "Records not supported");
    public static readonly TreeError TypeNotPartial = new(9, "Type not partial");
    public static readonly TreeError NoCopyConstructor = new(10, "No copy constructor");
    public static readonly TreeError NoGetter = new(11, "No suitable getter");
    public static readonly TreeError NoSetter = new(12, "No suitable setter");
    public static readonly TreeError InvalidGetter = new(13, "Invalid getter");
    public static readonly TreeError InvalidSetter = new(14, "Invalid setter");
    public static readonly TreeError IndexerNotFound = new(15, "Indexer not found");
    public static readonly TreeError IndexerNotSupported = new(16, "Indexer not supported");
    public static readonly TreeError NotWrittable = new(17, "Not writtable");
    public static readonly TreeError InvalidMethod = new(18, "Invalid method");
    public static readonly TreeError InvalidReturnType = new(19, "Invalid return type");
    public static readonly TreeError NoParentNode = new(20, "No parent node");
}

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
        this TreeError error, SyntaxNode syntax,
        string? message = null, DiagnosticSeverity severity = DiagnosticSeverity.Error)
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
    /// Reports in the given context a diagnostic for the given error, associated with the given
    /// syntax node. If no message is given, then the default error one is used.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="syntax"></param>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    public static void Report(
        this TreeError error, SyntaxNode syntax, SourceProductionContext context,
        string? message = null, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var diag = error.Create(syntax, message, severity);
        diag.Report(context);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a diagnostic for the given error associated with the given symbol. If no location
    /// is given, then a default one for the symbol is used. Similarly, if no message is given,
    /// then the default error one is used.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="symbol"></param>
    /// <param name="location"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic Create(
        this TreeError error, ISymbol symbol,
        Location? location = null,
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

    /// <summary>
    /// Reports in the given context a diagnostic for the given error associated with the given
    /// symbol. If no location is given, then a default one for the symbol is used. Similarly,
    /// if no message is given, then the default error one is used.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="location"></param>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    public static void Report(
        this TreeError error, ISymbol symbol, SourceProductionContext context,
        Location? location = null,
        string? message = null, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var diag = error.Create(symbol, location, message, severity);
        diag.Report(context);
    }
}