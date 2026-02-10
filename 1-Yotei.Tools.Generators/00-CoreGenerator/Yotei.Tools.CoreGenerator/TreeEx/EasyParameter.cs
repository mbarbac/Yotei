namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyParameter
{
    /// <summary>
    /// Include the 'this' keyword before the first parameter of an extension method, in C#.
    /// </summary>
    public bool UseThis { get; set; }

    /// <summary>
    /// Include the 'params', 'scoped', 'ref', 'in', 'out', 'ByRef', and 'ByVal' keywords before
    /// the parameter, if possible.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to include the parameter's type. If null, it is ignored.
    /// </summary>
    public EasyType? TypeOptions { get; set; }

    /// <summary>
    /// Include the name of the parameter.
    /// </summary>
    public bool UseName { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyParameter Default => new()
    {
        UseThis = true,
        UseModifiers = true,
        TypeOptions = EasyType.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyParameter Full => new()
    {
        UseThis = true,
        UseModifiers = true,
        TypeOptions = EasyType.Full,
        UseName = true,
    };
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyParameter options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // With parameter type...
        if (options.TypeOptions != null)
        {
            var xoptions = options.TypeOptions with { HideName = false };
            var str = source.Type.EasyName(xoptions);
            sb.Append(str);

            if (!str.EndsWith('?') &&
                xoptions.NullableStyle == IsNullableStyle.UseAnnotations &&
                source.Type.IsNullableDecorated() &&
                !source.Type.IsNullableWrapper())
                sb.Append('?');
        }


        // With parameter name...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers and alike...
        if (sb.Length > 0)
        {
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
            if (options.UseThis && source.IsThis) sb.Insert(0, "this ");
        }

        // Finishing...
        return sb.ToString();
    }
}