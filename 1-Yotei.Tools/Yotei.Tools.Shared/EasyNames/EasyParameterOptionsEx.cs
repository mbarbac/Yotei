namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given element, using default options.
    /// <br/> This method is just a best-effort one because there are syntax elements the compiler
    /// does not keep from the source code.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => source.EasyName(EasyParameterOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation for a given element, using the given options.
    /// <br/> This method is just a best-effort one because there are syntax elements the compiler
    /// does not keep from the source code.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyParameterOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // Type...
        if (options.TypeOptions != null)
        {
            var xoptions = options.TypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = source.ParameterType;
            var str = arg.EasyName(xoptions);

            if (str.Length > 0)
            {
                // Nullability...
                var pointer = str.EndsWith('*');
                if (pointer) str = str[..^1].NotNullNotEmpty(trim: false); // Temporary removal...

                while (str[^1] != '?' && xoptions.NullableStyle != EasyNullableStyle.None)
                {
                    if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                        arg.IsNullableWrapper() &&
                        !arg.IsArray &&
                        !arg.IsPointer)
                        break;

                    if (arg.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                    break;
                }
                if (pointer) str += '*'; // Restoring pointer...

                // Adding...
                sb.Append(str);
            }
        }

        // Name...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers (only if sb is not empty...)
        if (sb.Length > 0)
        {
            // This...
            if (options.UseThis && source.Name != null)
            {
                var method = source.Member as MethodBase;

                if (method != null &&
                    method.IsDefined(typeof(ExtensionAttribute), false) &&
                    source.Position == 0)
                    sb.Insert(0, "this ");
            }

            // Modifiers...
            if (options.UseModifiers)
            {
                // Traditional 'in' modifier...
                if (source.ParameterType.IsByRef &&
                    source.IsIn &&
                    source.HasReadOnlyAttribute())
                    sb.Insert(0, "in ");

                // Special case: 'ref readonly'...
                else if (source.ParameterType.IsByRef &&
                    source.GetCustomAttributes().Any(x => x.GetType().FullName == REQUIRES_LOCATION))
                    sb.Insert(0, "ref readonly ");

                // Other ref-alike cases...
                else if (source.IsIn) sb.Insert(0, "in ");
                else if (source.IsOut) sb.Insert(0, "out ");
                else if (source.ParameterType.IsByRef) sb.Insert(0, "ref ");

                // Params...
                if (source.IsDefined(typeof(ParamArrayAttribute), false)) sb.Insert(0, "params ");

                // Scoped...
                if (source
                    .GetCustomAttributes()
                    .Any(x => x.GetType().FullName == SCOPED_ATTRIBUTE))
                    sb.Insert(0, "scoped ");
            }
        }

        // Finishing...
        return sb.ToString();
    }
}