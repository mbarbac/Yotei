namespace Yotei.Tools.Generators;

// ========================================================
partial class TypeNode
{
    /// <summary>
    /// Obtains the not-normalized file name of the element carried by this instance. It needs
    /// normalization before it can be used as an actual file name.
    /// </summary>
    public string FileName
    {
        get
        {
            var options = new EasyTypeOptions
            {
                NamespaceStyle = EasyNamespaceStyle.Default,
                UseHost = true,
                UseSpecialNames = true,
                RemoveAttributeSuffix = false,
                NullableStyle = EasyNullableStyle.UseAnnotations,
                GenericListStyle = EasyGenericListStyle.UseNames
            };
            return Symbol.EasyName(options);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> This method is INFRASTRUCTURE and not intended for inheritors usage.
    /// <br/> Inheritors can modify behavior by using the <see cref="GetBaseList"/>, the
    /// <see cref="OnValidate(SourceProductionContext)"/>, the <see cref="IsSupportedKind"/>, the
    /// <see cref="OnEmitCore(ref TreeContext, CodeBuilder)"/> and the
    /// <see cref="OnEmitChilds(ref TreeContext, CodeBuilder)"/> methods.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    public bool Emit(ref TreeContext context, CodeBuilder cb)
    {
        context.Context.CancellationToken.ThrowIfCancellationRequested();

        // Parent elements, returning how many levels were opened...
        var num = OnEmitParents(ref context, cb);

        // This type...
        var head = GetTypeHeader(Symbol);
        var str = GetBaseList();
        if ((str = str.NullWhenEmpty(trim: true)) != null) head += $" : {str}";

        cb.AppendLine(head);
        cb.AppendLine("{");
        cb.IndentLevel++;

        var old = cb.Length;
        var ret = OnEmitCore(ref context, cb);
        if (ret)
        {
            var len = cb.Length;
            if (len != old) cb.AppendLine();

            if (!OnEmitChilds(ref context, cb)) ret = false;
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        // Closing he parent levels...
        for (var i = 0; i < num; i++)
        {
            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Returning...
        return ret;
    }

    // ----------------------------------------------------

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
            NamespaceStyle = EasyNamespaceStyle.None,
            UseHost = false,
            UseSpecialNames = true,
            RemoveAttributeSuffix = false,
            NullableStyle = EasyNullableStyle.UseAnnotations,
            GenericListStyle = EasyGenericListStyle.UseNames,
        });

        // Always a partial one...
        return $"partial {rec}{kind} {name}";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the host parents of this instance. Returns the number of opened levels
    /// that must be closed afterwards.
    /// </summary>
    int OnEmitParents(ref TreeContext context, CodeBuilder cb)
    {
        throw null;
    }
}