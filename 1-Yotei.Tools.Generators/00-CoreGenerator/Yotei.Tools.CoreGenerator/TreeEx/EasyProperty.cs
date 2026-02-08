using System.Reflection.Metadata.Ecma335;

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyProperty
{
    /// <summary>
    /// Include the accessibility modifiers of the member (ie: public).
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// Include the modifiers of the member (ie: static readonly).
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to include the type of the property. If null, it is ignored.
    /// </summary>
    public EasyType? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the host type of the member. If null, it is ignored.
    /// </summary>
    public EasyType? HostTypeOptions { get; set; }

    /// <summary>
    /// If enabled, include member brackets, even if <see cref="ParameterOptions"/> is null. This
    /// setting is only used with indexed properties.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, the options to include the parameters of the member. This setting is only used
    /// with indexed properties. If null, they are ignored.
    /// </summary>
    public EasyParameter? ParameterOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyProperty Default => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyType.Default,
        UseBrackets = true,
        ParameterOptions = EasyParameter.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyProperty Full => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyType.Full,
        HostTypeOptions = EasyType.Full,
        UseBrackets = true,
        ParameterOptions = EasyParameter.Full,
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
    public static string EasyName(this IPropertySymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyProperty options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var format = ToDisplayFormat(options);
        var head = source.ToDisplayString(format);

        return head;
    }

    /*

            // Modifiers...
            if (options.UseModifiers && options.ReturnTypeOptions != null)
            {
                if (source.IsSealed) sb.Append("sealed ");
                if (source.IsStatic) sb.Append("static ");

                var str = source.RefKind switch
                {
                    RefKind.Ref => "ref",
                    RefKind.Out => "out",
                    RefKind.In => "ref readonly",
                    _ => null
                };
                if (str != null) sb.Append(str).Append(' ');
            }

            // Return type...
            if (options.ReturnTypeOptions != null)
            {
                var xoptions = options.ReturnTypeOptions with { HideName = false };
                var str = source.ReturnType.EasyName(xoptions);
                sb.Append(str).Append(' ');
            }

            // Host type...
            var host = source.ContainingType;
            if (options.HostTypeOptions != null && host != null)
            {
                var xoptions = options.HostTypeOptions with { HideName = false };
                var str = host.EasyName(xoptions);
                sb.Append(str).Append('.');
            }

            // Name...
            sb.Append(source.Name);
        }



        // Generic arguments...
        if (options.GenericOptions != null)
        {
            var args = source.TypeArguments;
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var xoptions = options.GenericOptions with { GenericOptions = options.GenericOptions };
                    var arg = args[i];
                    var str = EasyName(arg, xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Parameters...
        if (options.UseBrackets || options.ParameterOptions != null)
        {
            sb.Append('('); if (options.ParameterOptions != null)
            {
                var args = source.Parameters;
                if (args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        var str = EasyName(arg, options.ParameterOptions);
                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();

        /// <summary>
        /// Invoked when the method is a constructor.
        /// </summary>
        static void DoConstructor(StringBuilder sb, IMethodSymbol source, EasyMethod options)
        {
            if (source.MethodKind is MethodKind.Constructor && // Regular constructor only!
                options.UseAccessibility)
            {
                var temp = source.DeclaredAccessibility.ToAccesibilityString();
                if (temp != null) sb.Append(temp).Append(' ');
            }

            // Modifiers...
            if (options.UseModifiers && (
                source.MethodKind == MethodKind.StaticConstructor || source.IsStatic))
                sb.Append("static ");

            // Name...
            var xoptions = options.HostTypeOptions ?? options.ReturnTypeOptions ?? new();
            xoptions = xoptions with { HideName = false };

            var host = source.ContainingType;
            var str = EasyName(host, xoptions);
            sb.Append(str);
            if (options.UseTechName) sb.Append(source.Name);
        }
     
     */
}