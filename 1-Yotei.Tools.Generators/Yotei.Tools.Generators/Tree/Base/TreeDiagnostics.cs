namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class TreeDiagnostics
{
    /// <summary>
    /// Cannot find a symbol associated with the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        TypeDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var text = syntax.Identifier.Text;
        var id = "TreeGen01";
        var head = "Cannot find a symbol associated with the given syntax.";
        var desc = $"Cannot find a symbol associated with the syntax: '{text}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find a symbol associated with the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        PropertyDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var text = syntax.Identifier.Text;
        var id = "TreeGen01";
        var head = "Cannot find a symbol associated with the given syntax.";
        var desc = $"Cannot find a symbol associated with the syntax: '{text}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find a symbol associated with the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        FieldDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var text = syntax.Declaration.ToString();
        var id = "TreeGen01";
        var head = "Cannot find a symbol associated with the given syntax.";
        var desc = $"Cannot find a symbol associated with the syntax: '{text}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Cannot find a symbol associated with the given syntax.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic SymbolNotFound(
        MethodDeclarationSyntax syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var text = syntax.Identifier.Text;
        var id = "TreeGen01";
        var head = "Cannot find a symbol associated with the given syntax.";
        var desc = $"Cannot find a symbol associated with the syntax: '{text}'.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// A candidate is not consistent with an existing node in the hierarchy.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="candidate"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InconsistentHierarchy(
        INode node,
        INodeCandidate candidate,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "A candidate is not consistent with an existing node in the hierarchy.";
        var desc = $"Candidate '{candidate}' is not consistent with an existing node: '{node}'.";
        var location = candidate.Syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}