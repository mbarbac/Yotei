namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class BaseDiagnostics
{
    /// <summary>
    /// Determines if the given type is a partial one.
    /// If not, and the severity is not 'hidden', then a diagnostic is reported.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsPartial(
        this ITypeSymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var nodes = symbol.GetSyntaxNodes();
        foreach (var node in nodes)
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "BaseGen01";
            var head = "Type is not partial.";
            var desc = $"The type '{symbol.Name}' is not a partial one.";

            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                id, head, desc,
                "Yotei", severity, isEnabledByDefault: true),
                symbol.Locations.FirstOrDefault()));
        }
        return false;
    }

    /// <summary>
    /// Determines if the given type is of a suported kind.
    /// If not, and the severity is not 'hidden', then a diagnostic is reported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsSupported(
        this ITypeSymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (symbol.TypeKind
            is TypeKind.Class or TypeKind.Struct or TypeKind.Interface) return true;

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "BaseGen02";
            var head = "Type is not of a supported kind.";
            var desc = $"The type '{symbol.Name}' is not a class, struct or interface.";

            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                id, head, desc,
                "Yotei", severity, isEnabledByDefault: true),
                symbol.Locations.FirstOrDefault()));
        }
        return false;
    }

    /// <summary>
    /// Determines if the given type is not a record.
    /// If not, and the severity is not 'hidden', then a diagnostic is reported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool TypeIsNotRecord(
        this ITypeSymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (!symbol.IsRecord) return true;

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "BaseGen03";
            var head = "Type is a record.";
            var desc = $"The type '{symbol.Name}' is a record.";

            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                id, head, desc,
                "Yotei", severity, isEnabledByDefault: true),
                symbol.Locations.FirstOrDefault()));
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given property has a suitable getter.
    /// If not, and the severity is not 'hidden', then a diagnostic is reported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool PropertyHasGetter(
        this IPropertySymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (symbol.GetMethod != null) return true;

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "BaseGen04";
            var head = "Property has no getter.";
            var desc = $"The property '{symbol.Name}' has no getter.";

            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                id, head, desc,
                "Yotei", severity, isEnabledByDefault: true),
                symbol.Locations.FirstOrDefault()));
        }
        return false;
    }

    /// <summary>
    /// Determines if the given property has a suitable setter.
    /// If not, and the severity is not 'hidden', then a diagnostic is reported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool PropertyHasSetter(
        this IPropertySymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (symbol.SetMethod != null) return true;

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "BaseGen05";
            var head = "Property has no setter.";
            var desc = $"The property '{symbol.Name}' has no setter.";

            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                id, head, desc,
                "Yotei", severity, isEnabledByDefault: true),
                symbol.Locations.FirstOrDefault()));
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given field is writtable.
    /// If not, and the severity is not 'hidden', then a diagnostic is reported.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static bool FieldIsWrittable(
        this IFieldSymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        if (!symbol.IsConst && !symbol.IsReadOnly && !symbol.HasConstantValue) return true;

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "BaseGen06";
            var head = "Field is not writtable.";
            var desc = $"The field '{symbol.Name}' is not writtable.";

            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                id, head, desc,
                "Yotei", severity, isEnabledByDefault: true),
                symbol.Locations.FirstOrDefault()));
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports a diagnostic when a copy constructor is not found.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this ITypeSymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "BaseGen07";
        var head = "Type is not partial.";
        var desc = $"The type '{symbol.Name}' is not a partial one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when the specifications are invalid.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="specs"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    public static void InvalidSpecs(
        this ISymbol symbol,
        string? specs,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "BaseGen08";
        var head = "Invalid specifications.";
        var desc = $"Specifications '{specs}' are invalid for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when a base method was not found.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <param name="severity"></param>
    public static void NoBaseMethod(
        this ISymbol symbol,
        SourceProductionContext context,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "BaseGen09";
        var head = "No base method found.";
        var desc = $"No base method found for symbol '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}