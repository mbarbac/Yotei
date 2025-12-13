namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given source element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return EasyName(source, options, types);
    }

    /// <summary>
    /// Invoked after the closed generic arguments are captured. Otherwise, when using recursion,
    /// the actual information about the closed types is lost, and only the generic remains.
    /// </summary>
    static string EasyName(this Type source, EasyNameOptions options, Type[] types)
    {
        var sb = new StringBuilder();
        var isgen = source.FullName == null;
        var host = source.DeclaringType;

        // Namespace...
        if (options.TypeUseNamespace && !isgen && host is null)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) sb.Append($"{str}.");
        }

        // Host...
        if ((options.TypeUseHost || options.TypeUseNamespace) &&
            !isgen &&
            host is not null)
        {
            var str = host.EasyName(options, types);
            sb.Append($"{str}.");
        }

        // Name...
        var name = string.Empty;
        if (options.TypeUseName || options.TypeUseHost || options.TypeUseNamespace)
        {
            name = GetTypeName(source);
            sb.Append(name);
        }
        static string GetTypeName(Type source)
        {
            var name = source.Name;
            var index = name.IndexOf('`');
            if (index >= 0) name = name[..index];

            var type = source.ReflectedType;
            if (type != null)
            {
                var at = (NullableAttribute)Attribute.GetCustomAttribute(type, typeof(NullableAttribute))!;
                if (at is not null)
                {
                    //var nums = type.GetGenericArguments().Select((x, i) => x == source ? i : -1);
                    //index = nums.Where(static x => x > 0).FirstOrDefault();

                    index = type.GetGenericArguments().IndexOf(x => x == source);
                    if (index >= 0)
                    {
                        var value = at.NullableFlags[index + 1];
                        if (value == 2) name += "?";
                    }
                }
            }

            return name;
        }

        // Generic arguments...
        if (options.TypeGenericArgumentOptions is not null)
        {
            var args = source.GetGenericArguments().Length;
            if (args > 0)
            {
                var used = host == null ? 0 : host.GetGenericArguments().Length;
                var need = args - used;
                if (need > 0)
                {
                    if (name.Length == 0) sb.Append(GetTypeName(source));

                    sb.Append('<'); for (int i = 0; i < need; i++)
                    {
                        var arg = types[i + used];
                        var str = arg.EasyName(options.TypeGenericArgumentOptions);

                        if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                        sb.Append(str);
                    }
                    sb.Append('>');
                }
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given source element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given source element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given source element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given source element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given source element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyNameOptions options)
    {
        throw null;
    }
}