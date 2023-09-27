namespace Yotei.Tools.Generators;

// ========================================================
internal partial class TypeNode
{
    /// <summary>
    /// Validates the type defined by the given candidate is a partial one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public static bool ValidateIsPartial(SourceProductionContext context, TypeCandidate candidate)
    {
        candidate = candidate.ThrowWhenNull(nameof(candidate));

        var syntax = candidate.Syntax;
        var done = syntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));

        if (!done) context.ErrorTypeNotPartial(candidate.Symbol);
        return done;
    }

    /// <summary>
    /// Validates the type defined by the given symbol is a partial one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public static bool ValidateIsPartial(SourceProductionContext context, ITypeSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var nodes = symbol.GetSyntaxNodes();
        var done = false;

        foreach (var node in nodes)
            if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) done = true;

        if (!done) context.ErrorTypeNotPartial(symbol);
        return done;
    }

    /// <summary>
    /// Validates the type is of a supported kind, class, struct or interface.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public static bool ValidateIsSupported(SourceProductionContext context, ITypeSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        if (symbol.TypeKind
            is not TypeKind.Class
            and not TypeKind.Struct
            and not TypeKind.Interface)
        {
            context.ErrorTypeNotSupported(symbol);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validates the type is not a record.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public static bool ValidateNotRecord(SourceProductionContext context, ITypeSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        if (symbol.IsRecord)
        {
            context.ErrorTypeIsRecord(symbol);
            return false;
        }
        return true;
    }
}