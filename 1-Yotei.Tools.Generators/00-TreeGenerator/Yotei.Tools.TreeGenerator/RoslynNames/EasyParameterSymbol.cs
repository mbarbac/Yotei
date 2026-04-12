namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
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
            // The type itself...
            var xoptions = options.TypeOptions;
            var type = source.Type;
            var str = type.EasyName(xoptions);

            // Nullability...
            while (xoptions.NullableStyle != EasyNullableStyle.None)
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    type.IsNullableWrapper())
                    break;

                if (str.Length > 0 && str[^1] != '?')
                {
                    if (type.IsNullableAnnotatedOrAttribute()) { str += '?'; break; }
                    if (source.IsNullableAnnotatedOrAttribute()) { str += '?'; break; }
                }

                break;
            }

            // Adding...
            if (str.Length > 0) sb.Append(str); // no space here!
        }

        // Parameter name...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers...
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