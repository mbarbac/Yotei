namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class TreeDiagnostics
{
    /// <summary>
    /// Validates that the given type is a partial one.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsPartial(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var nodes = type.GetSyntaxNodes();
        foreach (var node in nodes)
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;

        var id = "TreeGen01";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Name}' is not a partial one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        return false;
    }

    /// <summary>
    /// Validates that the given type is a partial one.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
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

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given type is of a supported kind.
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
    /// Validates that the given property has a getter.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="property"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool PropertyHasGetter(
        this SourceProductionContext context,
        IPropertySymbol property,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (property.GetMethod != null) return true;

        var id = "TreeGen04";
        var head = "Property has no getter.";
        var desc = $"Property '{property.Name}' has no getter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            property.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given property has a setter.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="property"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool PropertyHasSetter(
        this SourceProductionContext context,
        IPropertySymbol property,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (property.SetMethod != null) return true;

        var id = "TreeGen05";
        var head = "Property has no setter.";
        var desc = $"Property '{property.Name}' has no setter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            property.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given field is a writtable one.
    /// <br/> If not, reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="field"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool FieldIsWrittable(
        this SourceProductionContext context,
        IFieldSymbol field,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (!field.IsConst && !field.IsReadOnly && !field.HasConstantValue) return true;

        var id = "TreeGen06";
        var head = "Field is not writtable.";
        var desc = $"Field '{field.Name}' is not writtable.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            field.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when the specifications for the given symbol are invalid.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="specs"></param>
    /// <param name="severity"></param>
    public static void InvalidSpecs(
        this SourceProductionContext context,
        ISymbol symbol,
        string? specs,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Invalid specifications.";
        var desc = $"Specifications '{specs}' are invalid for symbol '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when no copy constructor is found for the given type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "No copy constructor.";
        var desc = $"Type '{type.Name}' has not a copy constructor.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when no base method is found for the given symbol.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="source"></param>
    /// <param name="severity"></param>
    public static void NoBaseMethod(
        this SourceProductionContext context,
        ISymbol symbol,
        string source,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "No base method found.";
        var desc = $"No base method found for symbol '{symbol.Name}' on source '{source}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}