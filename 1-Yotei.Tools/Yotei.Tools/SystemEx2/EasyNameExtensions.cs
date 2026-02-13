namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return EasyName(source, types, options);
    }

    /// <summary>
    /// Invoked to obtain the representation of the given type once the details of its closed
    /// generic arguments have been obtained. Otherwise, these details are lost after recursive
    /// calls.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyNameOptions options)
    {
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;
        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        if (options.HideTypeName) return string.Empty;

        // Shortcut wrapped nullability...
        if (options.TypeNullableStyle != IsNullableStyle.KeepWrappers &&
            need == 1 &&
            source.IsNullableWrapper())
        {
            // If 'None' we just remove the wrapper...
            var type = types[used];
            var str = type.EasyName(options);

            // If 'UseAnnotation', we add the nullable annotation...
            if (options.TypeNullableStyle == IsNullableStyle.UseAnnotations &&
                !str.EndsWith('?'))
                str += '?';

            return str;
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask only for generic parameters...
        if (options.UseTypeVariance && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace..
        if (options.UseTypeNamespace && host == null && !isgen)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if ((options.UseTypeHost || options.UseTypeNamespace) && host != null && !isgen)
        {
            var xoptions = options.WithHideTypeName(false);
            var str = host.EasyName(types, xoptions);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.Length > 0 && name[^1] == '&') name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (options.TypeGenericArgumentOptions != null && need > 0)
        {
            sb.Append('<'); for (var i = 0; i < need; i++)
            {
                var arg = types[used + i];
                var str = arg.EasyName(options.TypeGenericArgumentOptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullability...
        while (options.TypeNullableStyle != IsNullableStyle.None && sb.Length > 0 && sb[^1] != '?')
        {
            if (options.TypeNullableStyle == IsNullableStyle.KeepWrappers &&
                source.IsNullableWrapper())
                break;

            if (source.ByNullableAttribute()) { sb.Append('?'); break; }
            if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }
            break;
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();

        // Type...
        if (options.ArgumentTypeOptions != null)
        {
            var type = source.ParameterType;
            var xoptions = options.ArgumentTypeOptions.WithHideTypeName(false);
            var str = type.EasyName(xoptions);
            sb.Append(str);

            // Nullability...
            while (xoptions.TypeNullableStyle != IsNullableStyle.None &&
                sb.Length > 0 &&
                sb[^1] != '?')
            {
                if (xoptions.TypeNullableStyle == IsNullableStyle.KeepWrappers &&
                    type.IsNullableWrapper())
                    break;

                if (source.ByNullabilityApi()) { sb.Append('?'); break; }
                if (source.ByNullableAttribute()) { sb.Append('?'); break; }
                if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }
                break;
            }
        }

        // Name...
        if (options.UseArgumentName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers...
        if (options.UseArgumentModifiers && sb.Length > 0)
        {
            string? prefix = null;
            if (source.IsIn) prefix = "in ";
            else if (source.IsOut) prefix = "out ";
            else if (source.ParameterType.IsByRef)
            {
                var ronly = source.GetCustomAttributes().Any(x => x.GetType().FullName == "System.Runtime.CompilerServices.IsReadOnlyAttribute");
                prefix = ronly ? "ref readonly " : "ref ";
            }

            if (prefix is not null) sb.Insert(0, prefix);
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var type = source.ReturnType;
            var xoptions = options.MemberReturnTypeOptions.WithHideTypeName(false);
            var str = type.EasyName(xoptions);
            if (str.Length > 0)
            {
                sb.Append(str);

                // Nullable attribute on member itself...
                while (xoptions.TypeNullableStyle != IsNullableStyle.None &&
                   sb.Length > 0 &&
                   sb[^1] != '?')
                {
                    if (xoptions.TypeNullableStyle == IsNullableStyle.KeepWrappers &&
                        type.IsNullableWrapper())
                        break;

                    if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }

                    break;
                }

                // Separator...
                sb.Append(' ');

                // Modifiers...
                if (xoptions.UseMemberModifiers && sb.Length > 0)
                {
                    var name = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
                    var hasatr = source.ReturnTypeCustomAttributes
                        .GetCustomAttributes(false).Any(x => x.GetType().FullName == name);

                    if (source.ReturnType.IsByRef && hasatr) sb.Insert(0, "ref readonly ");
                    else if (source.ReturnType.IsByRef) sb.Insert(0, "ref ");
                }
            }
        }

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
        {
            var xoptions = options.MemberHostTypeOptions.WithHideTypeName(false);
            var str = host.EasyName(xoptions);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (options.MemberGenericArgumentOptions != null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.MemberGenericArgumentOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.UseMemberBrackets || options.MemberArgumentOptions != null)
        {
            sb.Append('('); if (options.MemberArgumentOptions != null)
            {
                var args = source.GetParameters();
                if (args.Length > 0)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        var str = arg.EasyName(options.MemberArgumentOptions);
                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Name is built from the host...
        var xoptions = options.MemberHostTypeOptions ?? options;
        xoptions = xoptions.WithHideTypeName(false);

        var name = host!.EasyName(xoptions);
        sb.Append(name);
        if (name.Length == 0) sb.Append("new");
        else if (options.ConstructorTechName) { sb.Append('.'); sb.Append(source.Name); }

        // Member arguments...
        if (options.UseMemberBrackets || options.MemberArgumentOptions != null)
        {
            sb.Append('('); if (options.MemberArgumentOptions != null)
            {
                var args = source.GetParameters();
                if (args.Length > 0)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        var str = arg.EasyName(options.MemberArgumentOptions);
                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var type = source.PropertyType;
            var xoptions = options.MemberReturnTypeOptions.WithHideTypeName(false);
            var str = type.EasyName(xoptions);
            if (str.Length > 0)
            {
                sb.Append(str);

                // Nullability...
                while (xoptions.TypeNullableStyle != IsNullableStyle.None &&
                    sb.Length > 0 &&
                    sb[^1] != '?')
                {
                    if (xoptions.TypeNullableStyle == IsNullableStyle.KeepWrappers &&
                        type.IsNullableWrapper())
                        break;

                    if (source.ByNullabilityApi()) { sb.Append('?'); break; }
                    if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }
                    if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }
                    break;
                }

                // Separator...
                sb.Append(' ');

                // Modifiers (on original options)...
                if (options.UseMemberModifiers && sb.Length > 0)
                {
                    var getter = source.GetGetMethod();
                    if (getter != null && getter.ReturnType.IsByRef)
                    {
                        var temp = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
                        var at = getter.ReturnParameter
                            .GetCustomAttributes(false).Any(x => x.GetType().FullName == temp);

                        sb.Insert(0, at ? "ref readonly " : "ref ");
                    }
                }
            }
        }

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
        {
            var xoptions = options.MemberHostTypeOptions.WithHideTypeName(false);
            var str = host.EasyName(xoptions);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !options.IndexedTechName) name = "this";
        sb.Append(name);

        // Member arguments...
        if (args.Length > 0 &&
            (options.UseMemberBrackets || options.MemberArgumentOptions != null))
        {
            sb.Append('['); if (options.MemberArgumentOptions != null)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.MemberArgumentOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => source.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var host = source.DeclaringType;
        var sb = new StringBuilder();

        // Return type...
        if (options.MemberReturnTypeOptions != null)
        {
            var type = source.FieldType;
            var xoptions = options.MemberReturnTypeOptions.WithHideTypeName(false);
            var str = type.EasyName(xoptions);
            if (str.Length > 0)
            {
                sb.Append(str);

                // Nullability...
                while (xoptions.TypeNullableStyle != IsNullableStyle.None &&
                    sb.Length > 0 &&
                    sb[^1] != '?')
                {
                    if (xoptions.TypeNullableStyle == IsNullableStyle.KeepWrappers &&
                        type.IsNullableWrapper())
                        break;

                    if (source.ByNullabilityApi()) { sb.Append('?'); break; }
                    if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }
                    if (source.ByEasyNullableAttribute()) { sb.Append('?'); break; }
                    break;
                }

                // Separator...
                sb.Append(' ');

                // Modifiers (on original options)...
                if (options.UseMemberModifiers && sb.Length > 0 && type.IsByRef)
                {
                    var temp = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
                    var at = source.GetCustomAttributes(false).Any(x => x.GetType().FullName == temp);
                    sb.Insert(0, at ? "ref readonly " : "ref ");
                }
            }
        }

        // Host type...
        if (options.MemberHostTypeOptions != null && host != null)
        {
            var xoptions = options.MemberHostTypeOptions.WithHideTypeName(false);
            var str = host.EasyName(xoptions);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}