namespace Yotei.Tools;

// ========================================================
public static class EasyNameExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given type.
    /// <para>Using the UseNameSpace activates the UseFullTypeName one.</para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string EasyName(this Type type) => type.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given type.
    /// <para>Using the UseNameSpace activates the UseFullTypeName one.</para>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type type, EasyNameOptions options)
    {
        type = type.ThrowIfNull();

        var args = type.GetGenericArguments();
        return EasyName(type, options, args);
    }

    static string EasyName(Type type, EasyNameOptions options, Type[] args)
    {
        var sb = new StringBuilder();
        var used = 0;

        // Generic types...
        if (type.IsGenericParameter)
        {
            return options.PreventGenericTypeNames
                ? string.Empty
                : type.Name;
        }

        // Nested types...
        if (type.DeclaringType != null)
        {
            var host = type.DeclaringType;

            if (options.UseFullTypeName || options.UseNameSpace)
            {
                var temp = EasyName(host, options, args);
                sb.Append($"{temp}.");
            }

            while (host != null)
            {
                var temp = host.GetGenericArguments().Length;
                used += temp;
                host = host.DeclaringType!;
            }
        }

        // Not-nested types...
        else
        {
            if (options.UseNameSpace &&
                type.Namespace != null)
                sb.Append($"{type.Namespace}.");
        }

        // Names with generics...
        var i = type.Name.IndexOf('`');
        if (i >= 0)
        {
            sb.Append(type.Name.AsSpan(0, i));
            sb.Append('<');

            bool first = true;
            var types = type.GetGenericArguments();
            var typen = 0;

            while (used < args.Length && typen < types.Length)
            {
                if (!first) sb.Append(',');

                var temp = args[used].EasyName(options);
                if (!first && temp.Length > 0) sb.Append(' ');
                sb.Append(temp);

                first = false;
                typen++;
                used++;
            }

            sb.Append('>');
        }

        // Names without generics...
        else
        {
            sb.Append(type.Name);
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given constructor.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo info) => info.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given constructor.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo info, EasyNameOptions options)
    {
        info = info.ThrowIfNull();

        var sb = new StringBuilder();

        var type = info.DeclaringType;
        if (type != null) sb.Append(type.EasyName(options));
        else sb.Append("<void>");

        if (!info.Name.StartsWith('.')) sb.Append('.');
        sb.Append(info.Name);

        if (!options.PreventArguments)
        {
            sb.Append('(');
            var pars = info.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                if (i != 0) sb.Append(',');

                var temp = pars[i].ParameterType.EasyName(options);
                if (i != 0 && temp.Length > 0) sb.Append(' ');
                sb.Append(temp);

                sb.Append(' ');
                sb.Append(pars[i].Name);
            }
            sb.Append(')');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given method.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo info) => info.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given method.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo info, EasyNameOptions options)
    {
        info = info.ThrowIfNull();

        var sb = new StringBuilder();

        if (!options.PreventReturnType)
        {
            sb.Append(info.ReturnType.EasyName(options));
            sb.Append(' ');
        }

        if (options.UseNameSpace || options.UseTypeName || options.UseFullTypeName)
        {
            var type = info.DeclaringType;
            if (type != null) sb.Append(type.EasyName(options));
            else sb.Append("<void>");

            sb.Append('.');
        }
        sb.Append(info.Name);

        var gens = info.GetGenericArguments();
        if (gens.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < gens.Length; i++)
            {
                if (i != 0) sb.Append(',');

                var temp = gens[i].EasyName(options);
                if (i != 0 && temp.Length > 0) sb.Append(' ');
                sb.Append(temp);
            }
            sb.Append('>');
        }

        if (!options.PreventArguments)
        {
            sb.Append('(');
            var pars = info.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                if (i != 0) sb.Append(',');

                var temp = pars[i].ParameterType.EasyName(options);
                if (i != 0 && temp.Length > 0) sb.Append(' ');
                sb.Append(temp);

                sb.Append(' ');
                sb.Append(pars[i].Name);
            }
            sb.Append(')');
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given property.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo info) => info.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given property.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo info, EasyNameOptions options)
    {
        info = info.ThrowIfNull();

        var sb = new StringBuilder();

        if (!options.PreventReturnType)
        {
            sb.Append(info.PropertyType.EasyName(options));
            sb.Append(' ');
        }

        if (options.UseNameSpace || options.UseTypeName || options.UseFullTypeName)
        {
            var type = info.DeclaringType;
            if (type != null) sb.Append(type.EasyName(options));
            else sb.Append("<void>");

            sb.Append('.');
        }

        var pars = info.GetIndexParameters();
        if (pars.Length > 0)
        {
            var name = info.Name == "Item" ? "this" : info.Name;
            sb.Append(name);

            sb.Append('['); for (int i = 0; i < pars.Length; i++)
            {
                if (i != 0) sb.Append(',');

                var temp = pars[i].ParameterType.EasyName(options);
                if (i != 0 && temp.Length > 0) sb.Append(' ');
                sb.Append(temp);

                sb.Append(' ');
                sb.Append(pars[i].Name);
            }
            sb.Append(']');
        }
        else sb.Append(info.Name);

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given field.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo info) => info.EasyName(EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given field.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo info, EasyNameOptions options)
    {
        info = info.ThrowIfNull();

        var sb = new StringBuilder();

        if (!options.PreventReturnType)
        {
            sb.Append(info.FieldType.EasyName(options));
            sb.Append(' ');
        }

        if (options.UseNameSpace || options.UseTypeName || options.UseFullTypeName)
        {
            var type = info.DeclaringType;
            if (type != null) sb.Append(type.EasyName(options));
            else sb.Append("<void>");

            sb.Append('.');
        }
        sb.Append(info.Name);

        return sb.ToString();
    }
}