namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class EasyMemberExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given member, using the default settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo item) => item.EasyName(EasyMemberOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given member, using the given settings.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo item, EasyMemberOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Member type or return type...
        if (options.UseReturnType != null)
        {
            var type = item.ReturnType;
            var s = type.EasyName(options.UseReturnType);
            if (s.Length > 0) sb.Append($"{s} ");
        }

        // Host type...
        if (options.UseHostType != null && host != null)
        {
            var s = host.EasyName(options.UseHostType);
            if (s.Length > 0) sb.Append($"{s}.");
        }

        // Member name...
        if (options.UseName) sb.Append(item.Name);

        // Member generic type arguments...
        if (options.UseTypeArguments != null)
        {
            var gens = item.GetGenericArguments();
            if (gens.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < gens.Length; i++)
                {
                    var gen = gens[i];
                    var s = gen.EasyName(options.UseTypeArguments);

                    if (i > 0) sb.Append(s.Length > 0 ? ", " : ",");
                    sb.Append(s);
                }
                sb.Append('>');
            }
        }

        // Member arguments...
        if (options.UseArguments || options.UseArgumentsTypes != null || options.UseArgumentsNames)
        {
            sb.Append('(');
            var pars = item.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var stype = options.UseArgumentsTypes == null ? "" : par.ParameterType.EasyName(options.UseArgumentsTypes);
                var sname = options.UseArgumentsNames ? (par.Name ?? "") : "";

                if (i > 0) sb.Append((stype.Length > 0 || sname.Length > 0) ? ", " : ",");
                if (stype.Length > 0)
                {
                    sb.Append(stype);
                    if (sname.Length > 0) sb.Append(' ');
                }
                if (sname.Length > 0) sb.Append(sname);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given member, using the default settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo item) => item.EasyName(EasyMemberOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given member, using the given settings.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo item, EasyMemberOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Member type or return type...
        if (options.UseReturnType != null && host != null)
        {
            var s = host.EasyName(options.UseReturnType);
            if (s.Length > 0) sb.Append($"{s} ");
        }

        // Host type...
        if (options.UseHostType != null && host != null)
        {
            var s = host.EasyName(options.UseHostType);
            if (s.Length > 0) sb.Append($"{s}.");
        }

        // Member name...
        if (options.UseName) sb.Append(item.Name);

        // Member arguments...
        if (options.UseArguments || options.UseArgumentsTypes != null || options.UseArgumentsNames)
        {
            sb.Append('(');
            var pars = item.GetParameters(); for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var stype = options.UseArgumentsTypes == null ? "" : par.ParameterType.EasyName(options.UseArgumentsTypes);
                var sname = options.UseArgumentsNames ? (par.Name ?? "") : "";

                if (i > 0) sb.Append((stype.Length > 0 || sname.Length > 0) ? ", " : ",");
                if (stype.Length > 0)
                {
                    sb.Append(stype);
                    if (sname.Length > 0) sb.Append(' ');
                }
                if (sname.Length > 0) sb.Append(sname);
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }

    // ---------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given member, using the default settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo item) => item.EasyName(EasyMemberOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given member, using the given settings.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo item, EasyMemberOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;
        var pars = item.GetIndexParameters();

        // Member type or return type...
        if (options.UseReturnType != null)
        {
            var type = item.PropertyType;
            var s = type.EasyName(options.UseReturnType);
            if (s.Length > 0) sb.Append($"{s} ");
        }

        // Host type...
        if (options.UseHostType != null && host != null)
        {
            var s = host.EasyName(options.UseHostType);
            if (s.Length > 0) sb.Append($"{s}.");
        }

        // Member name...
        if (options.UseName) sb.Append(pars.Length == 0 ? item.Name : "this");

        // Member arguments...
        if (options.UseArguments || options.UseArgumentsTypes != null || options.UseArgumentsNames)
        {
            if (pars.Length > 0)
            {
                sb.Append('[');
                for (int i = 0; i < pars.Length; i++)
                {
                    var par = pars[i];
                    var stype = options.UseArgumentsTypes == null ? "" : par.ParameterType.EasyName(options.UseArgumentsTypes);
                    var sname = options.UseArgumentsNames ? (par.Name ?? "") : "";

                    if (i > 0) sb.Append((stype.Length > 0 || sname.Length > 0) ? ", " : ",");
                    if (stype.Length > 0)
                    {
                        sb.Append(stype);
                        if (sname.Length > 0) sb.Append(' ');
                    }
                    if (sname.Length > 0) sb.Append(sname);
                }
                sb.Append(']');
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ---------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given member, using the default settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo item) => item.EasyName(EasyMemberOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given member, using the given settings.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo item, EasyMemberOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = item.DeclaringType;

        // Member type or return type...
        if (options.UseReturnType != null)
        {
            var type = item.FieldType;
            var s = type.EasyName(options.UseReturnType);
            if (s.Length > 0) sb.Append($"{s} ");
        }

        // Host type...
        if (options.UseHostType != null && host != null)
        {
            var s = host.EasyName(options.UseHostType);
            if (s.Length > 0) sb.Append($"{s}.");
        }

        // Member name...
        if (options.UseName) sb.Append(item.Name);

        // Finishing...
        return sb.ToString();
    }
}