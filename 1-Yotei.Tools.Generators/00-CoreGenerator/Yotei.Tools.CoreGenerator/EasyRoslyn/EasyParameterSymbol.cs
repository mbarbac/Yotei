namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IParameterSymbol source) => source.EasyName(EasyParameterOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyParameterOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // Parameter type...
        if (options.TypeOptions != null)
        {
            var xoptions = options.TypeOptions.NoHideName();
            var str = source.Type.EasyName(xoptions);
            sb.Append(str);

            while (sb.Length > 0 && sb[^1] != '?' &&
                options.TypeOptions != null &&
                options.TypeOptions.NullableStyle != EasyNullableStyle.None)
            {
                if (options.TypeOptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    source.Type.IsNullableWrapper())
                    break;

                if (source.IsNullableByAnnotationOrAttribute()) sb.Append('?');
                break;
            }
        }

        // Parameter name...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers and alike...
        if (sb.Length > 0)
        {
            if (options.UseThis && source.IsThis) sb.Insert(0, "this ");

            if (options.UseModifiers)
            {
                var str = source.RefKind switch
                {
                    RefKind.In => "in ",
                    RefKind.Out => "out ",
                    RefKind.Ref => "ref ",
                    RefKind.RefReadOnlyParameter => "ref readonly ",
                    _ => null,
                };
                if (str != null) sb.Insert(0, str);

                if (source.IsParams) sb.Insert(0, "params ");
                if (source.ScopedKind == ScopedKind.ScopedValue) sb.Insert(0, "scoped ");
            }
        }
        
        // Finishing...
        return sb.ToString();
    }
}