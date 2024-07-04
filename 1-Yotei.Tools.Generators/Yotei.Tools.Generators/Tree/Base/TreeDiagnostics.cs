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

    // -----------------------------------------------------

    /// <summary>
    /// The syntax node's kind is not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        SyntaxNode syntax,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var item = syntax.GetText();
        var text = item.ToString().Replace('\r', ' ').Replace('\n', ' ').Trim();
        while (text.IndexOf("  ") > 0) text = text.Replace("  ", " ");

        var id = "TreeGen03";
        var head = "The syntax node kind is not supported.";
        var desc = $"The syntax node kind '{text}' is not supported.";
        var location = syntax.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    /// <summary>
    /// Type kind not supported.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic KindNotSupported(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "Type kind not supported.";
        var desc = $"Kind of type '{type.Name}' is not supported.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Type is not a partial.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic TypeIsNotPartial(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Name}' is not partial.";
        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}