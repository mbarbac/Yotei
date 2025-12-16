using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

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
    /// <br/>
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
        var isgen = source.FullName == null;
        var host = source.DeclaringType;

        var args = source.GetGenericArguments();
        var used = host == null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;        

        // Shortcut hide name...
        if (options.TypeHideName &&
            !options.TypeUseHost && !options.TypeUseNamespace &&
            (need > 0 || options.TypeGenericArgumentOptions is null))
            return string.Empty;

        // Shortcut nullable value types...
        if (source.Name.StartsWith("Nullable"))
        {
            var type = source.GetGenericArguments()[0];
            var str = type.EasyName(options);
            str += "?";
            return str;
        }

        // TODO: Obtain nullable annotation of generic types.
        // TODO: Obtain nullable annotation of reference types.
        // https://devblogs.microsoft.com/dotnet/announcing-net-6-preview-7/#libraries-reflection-apis-for-nullability-information
        // https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md

        // Other cases...
        var sb = new StringBuilder();

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
        if (!options.TypeHideName ||
            options.TypeUseHost || options.TypeUseNamespace ||
            (args.Length > 0 && options.TypeGenericArgumentOptions is not null))
        {
            name = GetTypeName(source);
            sb.Append(name);
        }        
        static string GetTypeName(Type source)
        {
            var name = source.Name;
            var index = name.IndexOf('`');
            if (index >= 0) name = name[..index];
            return name;
        }

        // Generic arguments...
        if (need > 0 && options.TypeGenericArgumentOptions is not null)
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

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    static string EasyName(this ParameterInfo source, EasyNameOptions options)
    {
        var sb = new StringBuilder();

        if (options.ParameterTypeOptions is not null)
        {
            var str = source.ParameterType.EasyName(options.ParameterTypeOptions);
            sb.Append(str);
        }

        if (options.ParameterUseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

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
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.ReturnType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        sb.Append(source.Name);

        // Generic arguments...
        if (options.MemberGenericArgumentOptions is not null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.MemberGenericArgumentOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member parameters...
        if (options.MemberUseParameters ||
            options.ParameterTypeOptions is not null || options.ParameterUseName)
        {
            sb.Append('(');
            var pars = source.GetParameters();
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = par.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(')');
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
        this ConstructorInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Host type...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = !options.ConstructorTechName ? "new" : source.Name;
        sb.Append(name);

        // Member parameters...
        if (options.MemberUseParameters ||
            options.ParameterTypeOptions is not null || options.ParameterUseName)
        {
            sb.Append('(');
            var pars = source.GetParameters();
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = par.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(')');
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
        this PropertyInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this PropertyInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.PropertyType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        var pars = source.GetIndexParameters();
        if (pars.Length > 0 && !options.IndexerTechName) name = "this";
        sb.Append(name);

        // Member parameters...
        if (pars.Length > 0 && (
            options.MemberUseParameters ||
            options.ParameterTypeOptions is not null || options.ParameterUseName))
        {
            sb.Append('[');
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];
                var str = par.EasyName(options);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                if (str.Length > 0) sb.Append(str);
            }
            sb.Append(']');
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
        this FieldInfo source) => EasyName(source, EasyNameOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given source element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyNameOptions options)
    {
        source.ThrowWhenNull();
        options.ThrowWhenNull();

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Return type...
        if (options.MemberReturnTypeOptions is not null)
        {
            var str = source.FieldType.EasyName(options.MemberReturnTypeOptions);
            if (str.Length > 0) sb.Append($"{str} ");
        }

        // Host type...
        if (options.MemberHostTypeOptions is not null && host is not null)
        {
            var str = host.EasyName(options.MemberHostTypeOptions);
            if (str.Length > 0) sb.Append($"{str}.");
        }

        // Name...
        var name = source.Name;
        sb.Append(name);

        // Finishing...
        return sb.ToString();
    }
}