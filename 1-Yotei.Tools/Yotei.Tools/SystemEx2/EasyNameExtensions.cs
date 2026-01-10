namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
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
    /// Invoked to obtain a C#-alike representation of the given type element, once the details
    /// of its closed generic arguments have been captured, to prevent loosing them in recursive
    /// calls.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyNameOptions options)
    {
        var isgen =
            (!source.IsGenericType && source.FullName == null) ||
            source.IsGenericParameter ||
            source.IsGenericTypeParameter ||
            source.IsGenericMethodParameter;

        var host = source.DeclaringType;
        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        if (options.HideTypeName) return string.Empty;

        // Shortcut wrapped nullability...
        if (options.TypeNullableStyle is not EasyNullableStyle.KeepWrappers && (
            (need == 1 && source.Name.StartsWith("Nullable`1")) ||
            (need == 1 && source.Name.StartsWith("IsNullable`1"))))
        {
            var type = types[used];
            var str = type.EasyName(options);

            if (options.TypeNullableStyle is EasyNullableStyle.UseAnnotations &&
                !str.EndsWith('?'))
                str += '?';

            return str;
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask...
        if (options.UseTypeVariance && source.IsGenericParameter) // or otherwise an error!
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (options.UseTypeNamespace && host == null && !isgen)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if (options.UseHost && host != null && !isgen)
        {
            var str = host.EasyName(types, options);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name (if 'name&' is a 'ref' type, such is handled elsewhere)...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.Length > 0 && name[^1] == '&') name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (options.UseGenericArguments && need > 0)
        {
            sb.Append('<'); for (var i = 0; i < need; i++)
            {
                var arg = types[used + i];
                var str = arg.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullability...
        if (options.TypeNullableStyle is not EasyNullableStyle.None &&
            sb.Length > 0 &&
            sb[^1] != '?')
        {
            if (options.TypeNullableStyle is EasyNullableStyle.KeepWrappers && (
                source.Name.StartsWith("Nullable`1") ||
                source.Name.StartsWith("IsNullable`1"))) goto END;

            // Nullable attribute...
            var nullability = source.GetCustomAttribute<NullableAttribute>();
            if (nullability != null && nullability.NullableFlags.Length > 0)
            {
                var flag = nullability.NullableFlags[0];
                if (flag == 2) { sb.Append('?'); goto END; }
            }

            // IsNullable attribute...
            if (source.GetCustomAttribute<IsNullableAttribute>() != null)
            { sb.Append('?'); goto END; }
        }

        // Finishing...
        END:
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
        if (options.UseArgumentType)
        {
            if (options.TypeNullableStyle != options.ArgumentNullableStyle)
                options = options with
                { TypeNullableStyle = options.ArgumentNullableStyle, HideTypeName = false };

            if (options.HideTypeName) options = options with { HideTypeName = false };

            var str = source.ParameterType.EasyName(options);
            if (str.Length > 0)
            {
                sb.Append(str);

                // Validating nullability...
                while (options.ArgumentNullableStyle is not EasyNullableStyle.None &&
                    sb.Length > 0 &&
                    sb[^1] != '?')
                {
                    if (options.ArgumentNullableStyle is EasyNullableStyle.KeepWrappers && (
                        source.ParameterType.Name.StartsWith("Nullable`1") ||
                        source.ParameterType.Name.StartsWith("IsNullable´1")))
                        break;

                    // Nullability API...
                    var nic = new NullabilityInfoContext();
                    var info = nic.Create(source);

                    if (info.ReadState == NullabilityState.Nullable ||
                        info.WriteState == NullabilityState.Nullable)
                    {
                        sb.Append('?');
                        break;
                    }

                    // Nullable attribute...
                    var nullability = source.GetCustomAttribute<NullableAttribute>();
                    if (nullability != null && nullability.NullableFlags.Length > 0)
                    {
                        var flag = nullability.NullableFlags[0];
                        if (flag == 2) { sb.Append('?'); break; }
                    }

                    // IsNullable attribute...
                    if (source.GetCustomAttribute<IsNullableAttribute>() != null)
                    { sb.Append('?'); break; }

                    // End nullability...
                    break;
                }
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
            else if (source.ParameterType.IsByRef) prefix = "ref ";

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
        if (options.UseReturnType)
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = source.ReturnType.EasyName(options);
            if (str.Length > 0) { sb.Append(str); sb.Append(' '); }
        }

        // Host type...
        if (options.UseReturnType && host != null)
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = host.EasyName(options);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (options.UseGenericArguments)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Parameters...
        if (options.UseBrackets || options.UseArgumentType || options.UseArgumentName)
        {
            sb.Append('('); if (options.UseArgumentType || options.UseArgumentName)
            {
                var args = source.GetParameters();
                if (args.Length > 0)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        var str = arg.EasyName(options);
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

        // Special case...
        if (host is null)
            sb.Append(options.UseConstructorTechName ? source.Name : "new");

        // Standard case...
        else
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = host.EasyName(options);
            sb.Append(str);

            if (options.UseConstructorTechName)
            {
                if (sb.Length > 0) sb.Append('.');
                sb.Append(source.Name);
            }
        }

        // Parameters...
        if (options.UseBrackets || options.UseArgumentType || options.UseArgumentName)
        {
            sb.Append('('); if (options.UseArgumentType || options.UseArgumentName)
            {
                var args = source.GetParameters();
                if (args.Length > 0)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        var str = arg.EasyName(options);
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
        if (options.UseReturnType)
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = source.PropertyType.EasyName(options);
            if (str.Length > 0)
            {
                sb.Append(str);

                // Validating nullability...
                while (options.ArgumentNullableStyle is not EasyNullableStyle.None &&
                    sb.Length > 0 &&
                    sb[^1] != '?')
                {
                    if (options.ArgumentNullableStyle is EasyNullableStyle.KeepWrappers && (
                        source.PropertyType.Name.StartsWith("Nullable`1") ||
                        source.PropertyType.Name.StartsWith("IsNullable´1")))
                        break;

                    // Nullability API...
                    var nic = new NullabilityInfoContext();
                    var info = nic.Create(source);

                    if (info.ReadState == NullabilityState.Nullable ||
                        info.WriteState == NullabilityState.Nullable)
                    {
                        sb.Append('?');
                        break;
                    }

                    // Nullable attribute...
                    var nullability = source.GetCustomAttribute<NullableAttribute>();
                    if (nullability != null && nullability.NullableFlags.Length > 0)
                    {
                        var flag = nullability.NullableFlags[0];
                        if (flag == 2) { sb.Append('?'); break; }
                    }

                    // IsNullable attribute...
                    if (source.GetCustomAttribute<IsNullableAttribute>() != null)
                    { sb.Append('?'); break; }

                    // End nullability...
                    break;
                }

                // Separator...
                sb.Append(' ');
            }
        }

        // Host type...
        if (options.UseHost && host != null)
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = host.EasyName(options);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !options.UseIndexedTechName) name = "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (
            options.UseBrackets || options.UseArgumentType || options.UseArgumentName))
        {
            sb.Append('['); if (options.UseArgumentType || options.UseArgumentName)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options);
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
        if (options.UseReturnType)
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = source.FieldType.EasyName(options);
            if (str.Length > 0)
            {
                sb.Append(str);

                // Validating nullability...
                while (options.ArgumentNullableStyle is not EasyNullableStyle.None &&
                    sb.Length > 0 &&
                    sb[^1] != '?')
                {
                    if (options.ArgumentNullableStyle is EasyNullableStyle.KeepWrappers && (
                        source.FieldType.Name.StartsWith("Nullable`1") ||
                        source.FieldType.Name.StartsWith("IsNullable´1")))
                        break;

                    // Nullability API...
                    var nic = new NullabilityInfoContext();
                    var info = nic.Create(source);

                    if (info.ReadState == NullabilityState.Nullable ||
                        info.WriteState == NullabilityState.Nullable)
                    {
                        sb.Append('?');
                        break;
                    }

                    // Nullable attribute...
                    var nullability = source.GetCustomAttribute<NullableAttribute>();
                    if (nullability != null && nullability.NullableFlags.Length > 0)
                    {
                        var flag = nullability.NullableFlags[0];
                        if (flag == 2) { sb.Append('?'); break; }
                    }

                    // IsNullable attribute...
                    if (source.GetCustomAttribute<IsNullableAttribute>() != null)
                    { sb.Append('?'); break; }

                    // End nullability...
                    break;
                }

                // Separator...
                sb.Append(' ');
            }
        }

        // Host type...
        if (options.UseHost && host != null)
        {
            if (options.HideTypeName) options = options with { HideTypeName = false };
            var str = host.EasyName(options);
            if (str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}