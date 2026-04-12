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
        this IMethodSymbol source) => source.EasyName(EasyMethodOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
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
        string? str;

        // Accessibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            str = source.DeclaredAccessibility.ToAccessibilityString(iface);
            if (str != null) sb.Append(str).Append(' ');
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsStatic) sb.Append("static ");
        }
    }
}
/*


            if (source.IsAbstract && !iface) sb.Append("abstract ");
            if (IsSealed()) sb.Append("sealed ");

            if (IsOverride()) sb.Append("override ");
            else if (IsVirtual() && !iface && !source.IsAbstract) sb.Append("virtual ");
            else if (IsNew()) sb.Append("new ");
        }

        // Return type (regular methods only)...
        if (options.ReturnTypeOptions != null && method != null)
        {
            // 'ref'-alike return types...
            if (options.UseModifiers && method.ReturnType.IsByRef)
            {
                var ronly = method.ReturnTypeCustomAttributes.HasReadOnlyAttribute();
                sb.Append(ronly ? "ref readonly " : "ref ");
            }

            // The type itself...
            var xoptions = options.ReturnTypeOptions;
            var type = method.ReturnType;
            var str = type.EasyName(xoptions);

            // Nullability...
            while (xoptions.NullableStyle != EasyNullableStyle.None && str.Length > 0)
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    type.IsNullableWrapper())
                    break;

                if (str.Length > 0 && str[^1] != '?')
                {
                    if (type.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                }

                break;
            }

            // Adding...
            if (str.Length > 0) sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null)
        {
            var type = method?.DeclaringType ?? constructor?.DeclaringType;
            if (type != null)
            {
                var xoptions = options.HostTypeOptions;
                var str = type.EasyName(xoptions);
                if (str.Length > 0)
                {
                    sb.Append(str);
                    if (method != null) sb.Append('.');
                }
            }
        }

        // Name...
        if (method != null) sb.Append(source.Name);
        if (constructor != null)
        {
            if (options.HostTypeOptions == null) // Otherwise is already captured...
            {
                var str = host?.EasyName(EasyTypeOptions.Empty) ?? "new";
                sb.Append(str);
            }
            if (options.UseTechName)
            {
                if (!source.Name.StartsWith('.')) sb.Append('.');
                sb.Append(source.Name);
            }
        }

        // Generic arguments (regular methods only)...
        if (options.GenericArgumentOptions != null && method != null)
        {
            var xoptions = options.GenericArgumentOptions;
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = xoptions.GenericListStyle == EasyGenericListStyle.PlaceHolders
                        ? string.Empty
                        : arg.EasyName(xoptions);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member parameters....
        if (options.UseBrackets || options.ParameterOptions != null)
        {
            sb.Append('('); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;
                var args = source.GetParameters();
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

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given source is a 'virtual' one.
        /// </summary>
        bool IsVirtual() => source.IsVirtual && !source.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'sealed' one.
        /// </summary>
        bool IsSealed() => source.IsVirtual && source.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'override' one.
        /// </summary>
        bool IsOverride() =>
            method != null &&
            method.IsVirtual &&
            method.GetBaseDefinition().DeclaringType != source.DeclaringType;

        /// <summary>
        /// Determines if the given source method (not constructor) is a 'new' one.
        /// </summary>
        bool IsNew()
        {
            if (method == null) return false;
            return iface
                ? (method.IsVirtual && FindBaseMethod(host, true) != null)
                : (!method.IsVirtual && FindBaseMethod(host) != null);
        }

        MethodInfo? FindBaseMethod(Type? host, bool ifaces = false)
        {
            if (host != null)
            {
                // First, the host's base types...
                var parent = host.BaseType;
                while (parent != null)
                {
                    var temp = FindMethodAt(parent);
                    if (temp != null) return temp;
                    parent = parent.BaseType;
                }

                // Then the host's interfaces, if requested...
                if (ifaces)
                {
                    foreach (var iface in host.GetInterfaces())
                    {
                        var temp = FindMethodAt(iface);
                        if (temp != null) return temp;
                    }
                }
            }
            return null;

            MethodInfo? FindMethodAt(Type type) => type.GetMethod(
                method.Name,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static,
                null,
                [.. method.GetParameters().Select(p => p.ParameterType)],
                null);
        }
    }
 */