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
        this PropertyInfo source) => source.EasyName(EasyPropertyOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation for a given element, using the given options.
    /// <br/> This method is just a best-effort one because there are syntax elements the compiler
    /// does not keep from the source code.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyPropertyOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;
        var method = source.GetMethod ?? source.SetMethod;

        // Accessibility...
        AddAccessibility();
        void AddAccessibility()
        {
            if (!options.UseAccessibility) return;
            if (method == null) return;

            if (method.IsPublic && !iface) sb.Append("public ");
            if (method.IsPrivate) sb.Append("private ");
            if (method.IsFamily) sb.Append("protected ");
            if (method.IsAssembly) sb.Append("internal ");
            if (method.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (method.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        AddModifiers();
        void AddModifiers()
        {
            if (!options.UseModifiers) return;
            if (method == null) return;

            if (method.IsStatic) sb.Append("static ");
            if (method.IsAbstract && !iface) sb.Append("abstract ");
            if (IsSealed()) sb.Append("sealed ");

            if (IsOverride()) sb.Append("override ");
            else if (IsVirtual() && !iface && !method.IsAbstract) sb.Append("virtual ");
            else if (IsNew()) sb.Append("new ");
        }

        // Member type...
        AddMemberType();
        void AddMemberType()
        {
            if (options.MemberTypeOptions == null) return;
            if (method == null) return;

            var xoptions = options.MemberTypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = method.ReturnType;
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

                // Modifiers...
                if (options.UseModifiers && arg.IsByRef)
                {
                    var ronly =
                        method.ReturnTypeCustomAttributes.HasReadOnlyAttribute() ||
                        arg.HasReadOnlyAttribute();

                    sb.Append(ronly ? "ref readonly " : "ref ");
                }

                // Adding...
                sb.Append(str).Append(' ');
            }
        }

        // Host type...
        AddHostType();
        void AddHostType()
        {
            if (options.HostTypeOptions == null) return;
            if (host == null) return;

            var xoptions = options.HostTypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            // No top-level nullable annotations, but wrappers are ok...
            if (xoptions.NullableStyle == EasyNullableStyle.UseAnnotations)
                xoptions.NullableStyle = EasyNullableStyle.KeepWrappers;

            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !options.UseTechName) name = "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given source is a 'virtual' one.
        /// </summary>
        bool IsVirtual() => method != null && method.IsVirtual && !method.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'sealed' one.
        /// </summary>
        bool IsSealed() => method != null && method.IsVirtual && method.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'override' one.
        /// </summary>
        bool IsOverride() =>
            method != null &&
            method.IsVirtual &&
            method.GetBaseDefinition().DeclaringType != method.DeclaringType;

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
}