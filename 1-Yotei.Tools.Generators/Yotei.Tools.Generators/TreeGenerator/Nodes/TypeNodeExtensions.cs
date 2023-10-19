namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class TypeNodeExtensions
{
    /// <summary>
    /// Validates that the type defined by the given candidate is not a partial one. Reports
    /// an error if not.
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="context"></param>
    public static bool ValidateIsPartial(this TypeCandidate candidate, SourceProductionContext context)
    {
        candidate = candidate.ThrowWhenNull(nameof(candidate));

        var syntax = candidate.Syntax;
        var done = syntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));

        if (!done) context.TypeNotPartial(candidate.Symbol, DiagnosticSeverity.Error);
        return done;
    }

    /// <summary>
    /// Validates that the given type is a partial one. Reports an error if not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool ValidateIsPartial(this ITypeSymbol symbol, SourceProductionContext context)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var nodes = symbol.GetSyntaxNodes();
        var done = false;

        foreach (var node in nodes)
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) done = true;

        if (!done) context.TypeNotPartial(symbol, DiagnosticSeverity.Error);
        return done;
    }

    /// <summary>
    /// Validates that the given type is of a supported kind (interface, class, or struct).
    /// Reports an error if not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool ValidateIsSupportedKind(this ITypeSymbol symbol, SourceProductionContext context)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        if (symbol.TypeKind
            is not TypeKind.Class
            and not TypeKind.Struct
            and not TypeKind.Interface)
        {
            context.TypeNotSupported(symbol, DiagnosticSeverity.Error);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validates that the given type is not a record. Reports an error otherwise.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool ValidateNotRecord(this ITypeSymbol symbol, SourceProductionContext context)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        if (symbol.IsRecord)
        {
            context.TypeIsRecord(symbol, DiagnosticSeverity.Error);
            return false;
        }
        return true;
    }
}