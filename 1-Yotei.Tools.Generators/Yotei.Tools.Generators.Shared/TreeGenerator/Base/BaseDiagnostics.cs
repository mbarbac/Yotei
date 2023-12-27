namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class BaseDiagnostics
{
    /// <summary>
    /// Determines if the given type is a partial one or not.
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

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "TreeGen01";
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
    /// Determines if the type is of a supported kind, or not.
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

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "TreeGen02";
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
    /// Determines if the type is not a record, for scenarios where records are not supported.
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

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "TreeGen03";
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
    /// Determines that the property has a suitable getter, or not.
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

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "TreeGen04";
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
    /// Determines that the property has a suitable setter, or not.
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

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "TreeGen05";
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
    /// Determines if the field is a writtable one, or not.
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

        if (severity != DiagnosticSeverity.Hidden)
        {
            var id = "TreeGen06";
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
        var desc = $"Specifications '{specs}' are invalid for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when there is not a copy constructor for the given type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        ITypeSymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Type has no copy constructor.";
        var desc = $"The type '{symbol.Name}' has not a copy constructor.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }

    /// <summary>
    /// Reports a diagnostic when there is not a base method for the given symbol.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="severity"></param>
    public static void NoBaseMethod(
        this SourceProductionContext context,
        ISymbol symbol,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "No base method.";
        var desc = $"No base method found for symbol '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));
    }
}