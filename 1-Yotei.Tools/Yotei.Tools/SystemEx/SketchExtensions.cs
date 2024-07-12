/*
namespace Yotei.Tools;

// ========================================================
public static class SketchExtensions
{
    /// <summary>
    /// Returns an alternate string representation of the given source, using the default settings.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string Sketch<T>(this T? source) => source.Sketch(SketchOptions.Default);

    /// <summary>
    /// Returns the C#-alike name of the given type, using the given settings.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string Sketch<T>(this T? source, SketchOptions options)
    {
        // Reflection cases using 'EasyNameExtensions'...
        switch (source)
        {
            case Type item: return item.EasyName(options.TypeOptions);
            case ConstructorInfo item: return item.EasyName(options.MemberOptions);
            case PropertyInfo item: return item.EasyName(options.MemberOptions);
            case FieldInfo item: return item.EasyName(options.MemberOptions);
        }

        // Capturing type...
        var type = source is null ? typeof(T) : source.GetType();
        var typename = options.UseSourceType != null ? type.EasyName(options.UseSourceType) : "";

        // Capturing ToString methods...
        MethodInfo? toString_Format_Provider = null;
        MethodInfo? toString_Provider = null;
        MethodInfo? toString_Format = null;
        MethodInfo? toString_Core = null;
        CaptureToStringMethods(type);

        // Trivial cases...
        if (source == null) return RoundedSourceType(options.NullStr);
        if (source is string str) return RoundedSourceType(str);
        if (source is char) return RoundedSourceType(InvokeToString()!);
        if (source is Guid) return RoundedSourceType(InvokeToString()!);
        if (source is Enum) return GetEnum();
        if (type.IsPrimitive) return RoundedSourceType(InvokeToString()!);

        // There might be a captured 'ToString()'...
        str = InvokeToString()!;
        if (str != null) return RoundedSourceType(str);

        // Special options for nested elements...
        var xoptions = options with { UseSourceType = null };
        if (source is IDictionary dict) return GetDictionary(dict);
        if (source is IEnumerable items) return GetEnumerable(items);

        // Shape might be requested...
        str = GetShape()!;
        if (str != null) return RoundedSourceType(str);

        // Finishing...
        return typename.Length > 0 ? typename : type.Name;

        // -------------------------------------------------

        /// <summary>
        /// Enum values...
        /// </summary>
        string GetEnum()
        {
            var str = InvokeToString()!;
            var names = str.Split([", "], StringSplitOptions.None);

            for (int i = 0; i < names.Length; i++) names[i] = DottedSourceType(names[i]);
            return string.Join(" | ", names);
        }

        // -------------------------------------------------

        /// <summary>
        /// Dictionary values...
        /// </summary>
        string GetDictionary(IDictionary source)
        {
            var sb = new StringBuilder();
            if (typename.Length > 0) sb.Append(RoundedSourceType(string.Empty));

            var ini = '{';
            var end = '}';
            var first = true;

            sb.Append(ini); foreach (DictionaryEntry kv in source)
            {
                var key = kv.Key.Sketch(xoptions);
                var value = kv.Value.Sketch(xoptions);

                if (first) first = false;
                else sb.Append(", ");

                sb.AppendFormat("{0} = {1}", key, value);
            }
            sb.Append(end);
            return sb.ToString();
        }

        // -------------------------------------------------

        /// <summary>
        /// Enumeration values...
        /// </summary>
        string GetEnumerable(IEnumerable source)
        {
            var sb = new StringBuilder();
            if (typename.Length > 0) sb.Append(RoundedSourceType(string.Empty));

            var isdynamic = source is IDynamicMetaObjectProvider;
            var ini = isdynamic ? '{' : '[';
            var end = isdynamic ? '}' : ']';
            var first = true;

            sb.Append(ini);
            var iter = source.GetEnumerator(); while (iter.MoveNext())
            {
                var value = iter.Current.Sketch(xoptions);

                if (first) { first = false; if (isdynamic) sb.Append(' '); }
                else sb.Append(", ");

                sb.Append(value);
            }

            if (isdynamic && !first) sb.Append(' ');
            sb.Append(end);
            return sb.ToString();
        }

        // -------------------------------------------------

        /// <summary>
        /// Using source shape, if possible...
        /// </summary>
        string? GetShape()
        {
            if (!options.UseShape &&
                !options.UsePrivateMembers &&
                !options.UseStaticMembers)
                return null;

            var list = new List<string>();
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (options.UsePrivateMembers) flags |= BindingFlags.NonPublic;
            if (options.UseStaticMembers) flags |= BindingFlags.Static;

            var props = type.GetProperties(flags);
            foreach (var prop in props)
            {
                if (prop.GetIndexParameters().Length != 0)
                {
                    var name = prop.EasyName();
                    list.Add(name);
                }
                else if (prop.CanRead)
                {
                    var name = prop.Name;
                    var value = prop.GetValue(source, null);
                    var str = value.Sketch(xoptions);
                    list.Add(string.Format("{0} = {1}", name, str));
                }
            }

            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                if (field.CustomAttributes.Any(
                    x => x.AttributeType == typeof(CompilerGeneratedAttribute)))
                    continue;

                var name = field.EasyName();
                var value = field.GetValue(source);
                var str = value.Sketch(xoptions);
                list.Add(string.Format("{0} = {1}", name, str));
            }

            if (list.Count == 0) return null;

            var sb = new StringBuilder();
            sb.Append("{ ");
            sb.Append(string.Join(", ", list));
            sb.Append(" }");
            return sb.ToString();
        }

        // -------------------------------------------------

        /// <summary>
        /// Use a rounded type header with the given string value, if the source type header is
        /// requested.
        /// </summary>
        string RoundedSourceType(string str)
        {
            return typename.Length > 0 ? $"({typename}) {str}" : str;
        }

        /// <summary>
        /// Use a dotted type header with the given string value, if the source type header is
        /// requested.
        /// </summary>
        string DottedSourceType(string str)
        {
            return typename.Length > 0 ? $"{typename}.{str}" : str;
        }

        // -------------------------------------------------

        /// <summary>
        /// Invokes the appropriate ToString method, if any was captured...
        /// </summary>
        string? InvokeToString()
        {
            if (toString_Format_Provider != null) return (string?)toString_Format_Provider.Invoke(source, new object[] { options.Format!, options.Provider! });
            if (toString_Provider != null) return (string?)toString_Provider.Invoke(source, new object[] { options.Provider! });
            if (toString_Format != null) return (string?)toString_Format.Invoke(source, new object[] { options.Format! });
            if (toString_Core != null) return (string?)toString_Core.Invoke(source, null);

            return null;
        }

        /// <summary>
        /// Captures the public 'ToString' methods of the given source...
        /// </summary>
        void CaptureToStringMethods(Type type)
        {
            if (type == typeof(object)) return;

            var flags = BindingFlags.Instance | BindingFlags.Public;
            var methods = type.GetMethods(flags).Where(x => x.Name == "ToString");

            if (options.Format != null && options.Provider != null)
            {
                foreach (var method in methods)
                {
                    var pars = method.GetParameters();
                    if (pars.Length != 2) continue;
                    if (pars[0].ParameterType != typeof(string)) continue;
                    if (!pars[1].ParameterType.IsAssignableFrom(typeof(IFormatProvider))) continue;

                    toString_Format_Provider = method;
                    return;
                }
            }

            if (options.Provider != null)
            {
                foreach (var method in methods)
                {
                    var pars = method.GetParameters();
                    if (pars.Length != 1) continue;
                    if (!pars[0].ParameterType.IsAssignableFrom(typeof(IFormatProvider))) continue;

                    toString_Provider = method;
                    return;
                }
            }

            if (options.Format != null)
            {
                foreach (var method in methods)
                {
                    var pars = method.GetParameters();
                    if (pars.Length != 1) continue;
                    if (pars[0].ParameterType != typeof(string)) continue;

                    toString_Format = method;
                    return;
                }
            }

            foreach (var method in methods)
            {
                if (method.DeclaringType == typeof(object)) continue;
                var pars = method.GetParameters();
                if (pars.Length != 0) continue;

                toString_Core = method;
                return;
            }

            if (type.BaseType != null) CaptureToStringMethods(type.BaseType);
        }
    }
}*/