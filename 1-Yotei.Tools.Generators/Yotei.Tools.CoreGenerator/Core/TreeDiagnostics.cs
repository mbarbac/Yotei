namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TreeDiagnostics
{
    /// <summary>
    /// Syntax is not not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SyntaxNotSupported(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Syntax not supported";
        var desc = $"Syntax not supported: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The symbol that correspond to the given syntax was not found.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var type = syntax switch
        {
            TypeDeclarationSyntax => "type",
            PropertyDeclarationSyntax => "property",
            FieldDeclarationSyntax => "field",
            MethodDeclarationSyntax => "method",
            _ => "syntax"
        };

        var id = "TreeGen02";
        var head = "Symbol not found";
        var desc = $"Symbol not found for {type}: '{syntax}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}