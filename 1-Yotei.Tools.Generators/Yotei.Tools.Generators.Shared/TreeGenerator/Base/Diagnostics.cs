namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// Determines if the given type is a partial one, or reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsPartial(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var nodes = symbol.GetSyntaxNodes();
        foreach (var node in nodes)
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;

        var id = "BaseGen01";
        var head = "Type is not partial.";
        var desc = $"The type '{symbol.Name}' is not a partial one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        return false;
    }

    /// <summary>
    /// Determines if the type is of a supported kind, or reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsSupported(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (symbol.TypeKind
            is TypeKind.Class or TypeKind.Struct or TypeKind.Interface) return true;

        var id = "BaseGen02";
        var head = "Type is not of a supported kind.";
        var desc = $"The type '{symbol.Name}' is not a class, struct or interface.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        return false;
    }

    /// <summary>
    /// Determines if the type is not a record, or reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsNotRecord(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (!symbol.IsRecord) return true;

        var id = "BaseGen03";
        var head = "Type is a record.";
        var desc = $"The type '{symbol.Name}' is a record.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the property has a suitable getter, or reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool PropertyHasGetter(
        this SourceProductionContext context,
        IPropertySymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (symbol.GetMethod != null) return true;

        var id = "BaseGen04";
        var head = "Property has no getter.";
        var desc = $"The property '{symbol.Name}' has no getter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        return false;
    }

    /// <summary>
    /// Determines if the property has a suitable setter, or reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool PropertyHasSetter(
        this SourceProductionContext context,
        IPropertySymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (symbol.SetMethod != null) return true;

        var id = "BaseGen05";
        var head = "Property has no setter.";
        var desc = $"The property '{symbol.Name}' has no setter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the field is a writtable one, or reports a diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool FieldIsWrittable(
        this SourceProductionContext context,
        IFieldSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (!symbol.IsConst && !symbol.IsReadOnly && !symbol.HasConstantValue) return true;

        var id = "BaseGen06";
        var head = "Field is not writtable.";
        var desc = $"The field '{symbol.Name}' is not writtable.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when the specifications are invalid.
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
        var id = "BaseGen07";
        var head = "Invalid specifications.";
        var desc = $"Specifications '{specs}' are invalid for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic if no base method is found.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void NoBaseFound(
        this SourceProductionContext context,
        ISymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "BaseGen08";
        var head = "No base method found.";
        var desc = $"No base method found for symbol '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic if no copy constructor is found.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "BaseGen09";
        var head = "No copy constructor found.";
        var desc = $"No copy constructor found for type '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}