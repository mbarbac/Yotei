namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given parameter-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IParameterSymbol source) => source.EasyName(new EasyParameterOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given parameter-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyParameterOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // Parameter type (only if requested)...
        if (options.TypeOptions != null)
        {
            var xoptions = options.TypeOptions;
            var arg = source.Type;
            var str = arg.EasyName(xoptions);

            if (str.Length > 0)
            {
                // Nullability...
                var pointer = str.EndsWith('*');
                if (pointer) str = str[..^1].NotNullNotEmpty(trim: false);

                while (str[^1] != '?' && xoptions.NullableStyle != EasyNullableStyle.None)
                {
                    if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                        arg.IsNullableWrapper() &&
                        arg is not IArrayTypeSymbol &&
                        arg is not IPointerTypeSymbol) break;

                    if (arg.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                    break;
                }
                if (pointer) str += '*'; // Don't forget to restore pointer!

                // Adding...
                sb.Append(str); // No additional space!
            }
        }

        // Parameter name (only if requested)...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers (only if 'sb' is not an empty one)...
        if (sb.Length > 0)
        {
            // Special case for 'this'...
            if (options.UseThis && source.IsThis) sb.Insert(0, "this ");

            // Other modifiers...
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