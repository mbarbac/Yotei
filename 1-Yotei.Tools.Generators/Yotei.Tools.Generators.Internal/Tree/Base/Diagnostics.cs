namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// Validates that the given type is a partial one.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    public static bool TypeIsPartial(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (type.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;

        var id = "TreeGen01";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Identifier.Text}' is not a partial one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        return false;
    }

    /// <summary>
    /// Validates that the given type is a partial one.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    public static bool TypeIsPartial(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var nodes = type.GetSyntaxNodes();
        foreach (var node in nodes)
        {
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;
        }

        var id = "TreeGen01";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Name}' is not a partial one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given type is of a supported kind: interface, struct, or class.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeKindIsSupported(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (type.Kind() is
            SyntaxKind.InterfaceDeclaration or
            SyntaxKind.ClassDeclaration or
            SyntaxKind.StructDeclaration or
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration)
            return true;

        var id = "TreeGen02";
        var head = "Type kind is not supported.";
        var desc = $"Type '{type.Identifier.Text}' is not of a supported kind.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        return false;
    }

    /// <summary>
    /// Validates that the given type is of a supported kind: interface, struct, or class.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeKindIsSupported(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (type.TypeKind
            is TypeKind.Class or TypeKind.Struct or TypeKind.Interface) return true;

        var id = "TreeGen02";
        var head = "Type kind is not supported.";
        var desc = $"Type '{type.Name}' is not of a supported kind.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given type is not a record.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsNotRecord(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var isrec = type.Kind() is
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration;

        if (!isrec) return true;

        var id = "TreeGen03";
        var head = "Records are not supported in this context.";
        var desc = $"Type '{type.Identifier.Text}' is a record, which is not supported in this context.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        return false;
    }

    /// <summary>
    /// Validates that the given type is not a record.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsNotRecord(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (!type.IsRecord) return true;

        var id = "TreeGen03";
        var head = "Records are not supported in this context.";
        var desc = $"Type '{type.Name}' is a record, which is not supported in this context.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when the type has not a suitable copy constructor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Type has no copy constructor.";
        var desc = $"Type '{type.Identifier.Text}' has no copy constructor.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));
    }

    /// <summary>
    /// Reports a diagnostic when the type has not a suitable copy constructor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Type has no copy constructor.";
        var desc = $"Type '{type.Name}' has no copy constructor.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));
    }
}