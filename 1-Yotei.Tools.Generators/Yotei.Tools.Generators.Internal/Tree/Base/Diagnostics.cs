namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class Diagnostics
{
    /// <summary>
    /// Reports a duplicated type found in the hierarchy.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void DuplicatedHierarchyType(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Duplicated type found in the hierarchy.";
        var desc = $"Type '{type.Identifier.Text}' is found duplicated in the hierarchy.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }

    /// <summary>
    /// Reports a duplicated type found in the hierarchy.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void DuplicatedHierarchyType(
        this SourceProductionContext context,
        ITypeSymbol type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen01";
        var head = "Duplicated type found in the hierarchy.";
        var desc = $"Type '{type.Name}' is found duplicated in the hierarchy.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given type is not a partial one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void TypeIsNotPartial(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Identifier.Text}' is not partial.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }

    /// <summary>
    /// Reports that the given type is not a partial one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void TypeIsNotPartial(
        this SourceProductionContext context,
        ITypeSymbol type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen02";
        var head = "Type is not partial.";
        var desc = $"Type '{type.Name}' is not partial.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the kind of given type is not supported.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void TypeKindNotSupported(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "Type kind is not supported.";
        var desc = $"The kind of the type '{type.Identifier.Text}' is not supported.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }

    /// <summary>
    /// Reports that the kind of given type is not supported.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void TypeKindNotSupported(
        this SourceProductionContext context,
        ITypeSymbol type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen03";
        var head = "Type kind is not supported.";
        var desc = $"The kind of the type '{type.Name}' is not supported.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given type is a record.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void TypeIsRecord(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Type is a record, not supported in this context.";
        var desc = $"Type '{type.Identifier.Text}' is a record, not supported in this context.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }

    /// <summary>
    /// Reports that the given type is a record.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void TypeIsRecord(
        this SourceProductionContext context,
        ITypeSymbol type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen04";
        var head = "Type is a record, not supported in this context.";
        var desc = $"Type '{type.Name}' is a record, not supported in this context.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given type has not a suitable copy constructor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        TypeDeclarationSyntax type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Type has no copy constructor.";
        var desc = $"Type '{type.Identifier.Text}' has not copy constructor.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.GetLocation()));

        if (raiseException) throw new ArgumentException(desc);
    }

    /// <summary>
    /// Reports that the given type has not a suitable copy constructor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoCopyConstructor(
        this SourceProductionContext context,
        ITypeSymbol type,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen05";
        var head = "Type has not copy constructor.";
        var desc = $"Type '{type.Name}' has not copy constructor.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            type.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that no base method is found for the given element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoBaseMethod(
        this SourceProductionContext context,
        ISymbol symbol,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen06";
        var head = "No base method found.";
        var desc = $"No base method found for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given specifications are invalid ones.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="specs"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void InvalidSpecs(
        this SourceProductionContext context,
        ISymbol symbol,
        string? specs,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen07";
        var head = "Invalid specifications.";
        var desc = $"Specifications '{specs}' are invalid for '{symbol.Name}'.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given property has no suitable getter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="specs"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoGetter(
        this SourceProductionContext context,
        IPropertySymbol symbol,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen08";
        var head = "Property has no getter.";
        var desc = $"Propery '{symbol.Name}' has no suitable getter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given property has no suitable setter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="specs"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NoSetter(
        this SourceProductionContext context,
        IPropertySymbol symbol,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen09";
        var head = "Property has no setter.";
        var desc = $"Propery '{symbol.Name}' has no suitable setter.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reports that the given field is not a writable one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="specs"></param>
    /// <param name="raiseException"></param>
    /// <param name="severity"></param>
    public static void NotWrittable(
        this SourceProductionContext context,
        IFieldSymbol symbol,
        bool raiseException = false,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "TreeGen10";
        var head = "Field is not writtable.";
        var desc = $"Field '{symbol.Name}' is not a writtable one.";

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc,
            "Yotei", severity, isEnabledByDefault: true),
            symbol.Locations.FirstOrDefault()));

        if (raiseException) throw new ArgumentException(desc);
    }
}