namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given method-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol source) => source.EasyName(new EasyMethodOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given method-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var iface = host != null && host.IsInterface;
        bool constructor = source.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor;

        // Accessibility...
        if (options.UseAccessibility)
        {
            var str = source.DeclaredAccessibility.ToAccessibilityString(iface);
            if (str != null) sb.Append(str).Append(' ');
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsNew) sb.Append("new ");
            if (source.IsStatic) sb.Append("static ");
            if (source.IsSealed) sb.Append("sealed ");
            if (!iface && source.IsAbstract) sb.Append("abstract ");

            if (source.IsOverride) sb.Append("override ");
            else if (source.IsVirtual && !iface && !source.IsAbstract) sb.Append("virtual ");

            if (source.IsPartialDefinition ||
                source.PartialDefinitionPart is not null ||
                source.PartialImplementationPart is not null) sb.Append("partial ");
        }

        // Return type...
        if (options.ReturnTypeOptions != null && !constructor)
        {
            var xoptions = options.ReturnTypeOptions;
            var arg = source.ReturnType;
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

                // Ref-alike types...
                if (options.UseModifiers)
                {
                    var temp = source.RefKind switch
                    {
                        RefKind.Ref => "ref",
                        RefKind.Out => "out",
                        RefKind.In => "ref readonly",
                        _ => null
                    };
                    if (temp != null) sb.Append(temp).Append(' ');
                }

                // Adding...
                sb.Append(str).Append(' ');
            }
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions;
            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (constructor) // Constructor-alike methods...
        {
            if (options.HostTypeOptions == null) // Otherwise it has been already captured!
            {
                var str = host?.EasyName() ?? "new";
                sb.Append(str);
            }
            if (options.UseTechName) // Adding the CLR name if requested
            {
                if (!source.Name.StartsWith('.')) sb.Append('.');
                sb.Append(source.Name);
            }
        }
        else // Regular methods...
        {
            sb.Append(source.Name);
        }

        // Generic arguments (regular methods only)...
        if (options.GenericListOptions != null && !constructor)
        {
            var xoptions = options.GenericListOptions;
            var args = source.TypeArguments;

            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
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
                var xoptions = options.ParameterOptions;
                var args = source.Parameters;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }
}