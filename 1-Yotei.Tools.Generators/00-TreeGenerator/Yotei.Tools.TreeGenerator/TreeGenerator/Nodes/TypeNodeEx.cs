namespace Yotei.Tools.Generators;

// ========================================================
partial class TypeNode
{
    /// <summary>
    /// Obtains the appropriate header for the given type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    static string GetTypeHeader(INamedTypeSymbol symbol)
    {
        var rec = symbol.IsRecord ? "record " : string.Empty;
        var kind = symbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new ArgumentException("Type kind not supported.").WithData(symbol)
        };

        // We assume the type "is in range", so a simplified name is enough...
        var name = symbol.EasyName(new()
        {
            UseSpecialNames = true,
            NullableStyle = EasyNullableStyle.UseAnnotations,
            GenericListStyle = EasyGenericListStyle.UseNames,
        });

        // Always a partial one...
        return $"partial {rec}{kind} {name}";
    }

    // ----------------------------------------------------

    int OnEmitParents(CodeBuilder cb) => 0;
}