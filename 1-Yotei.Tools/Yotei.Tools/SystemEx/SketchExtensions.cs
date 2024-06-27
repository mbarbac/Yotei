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
        // Capturing type...
        var type = source is null ? typeof(T) : source.GetType();
        var typename = type.EasyName(options.TypeOptions);

        // Null values and inherited cases from 'EasyName'...
        string? str = source switch
        {
            Type item => type.EasyName(),
            ConstructorInfo item => item.EasyName(),
            MethodInfo item => item.EasyName(),
            PropertyInfo item => item.EasyName(),
            FieldInfo item => item.EasyName(),

            null => RoundedTypeName(options.NullStr), // Uses 'typename', hence why we have captured it...
            _ => null
        };
        
        if (str is not null) return str;

        // Capturing ToString methods...
        MethodInfo? toString_Format_Provider = null;
        MethodInfo? toString_Provider = null;
        MethodInfo? toString_Format = null;
        MethodInfo? toString_Core = null;
        CaptureToStringMethods(type);

        // Preliminary trivial cases...
        str = source switch
        {
            char item => RoundedTypeName(InvokeToString()!),
            string item => RoundedTypeName(InvokeToString()!),
            Guid item => RoundedTypeName(InvokeToString()!),
            _ => null
        };
        if (str is not null) return str;
        if (type.IsPrimitive) return RoundedTypeName(InvokeToString()!);
        if (source is Enum) return GetEnum();

        // String must come first to prevent it to be enumerated...
        str = InvokeToString();
        if (str is not null) return RoundedTypeName(str);

        // Other known types...
        if (source is IDictionary dict) return GetDictionary(dict);
        if (source is IEnumerable items) return GetEnumerable(items);

        // We may be authorized to compute the shape...
        str = GetShape();
        if (str is not null) return RoundedTypeName(str);

        // Finalizing...
        return typename.Length > 0 ? typename : type.EasyName();
        
        // -------------------------------------------------

        /// <summary>
        /// Enum values...
        /// </summary>
        string GetEnum()
        {
            var str = InvokeToString()!;
            var names = str.Split([", "], StringSplitOptions.None);

            for (int i = 0; i < names.Length; i++) names[i] = DottedTypeName(names[i]);
            return string.Join(" | ", names);
        }

        // -------------------------------------------------

        /// <summary>
        /// Enumeration values...
        /// </summary>
        string GetEnumerable(IEnumerable source)
        {
            var sb = new StringBuilder();
            if (typename.Length > 0) sb.Append(RoundedTypeName(string.Empty));

            var isdynamic = source is IDynamicMetaObjectProvider;
            var ini = isdynamic ? '{' : '[';
            var end = isdynamic ? '}' : ']';
            var first = true;

            var xoptions = options with { TypeOptions = EasyTypeOptions.Default with { UseName = false } };
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
        /// Dictionary values...
        /// </summary>
        string GetDictionary(IDictionary source)
        {
            var sb = new StringBuilder();
            if (typename.Length > 0) sb.Append(RoundedTypeName(string.Empty));

            var xoptions = options with { TypeOptions = EasyTypeOptions.Default with { UseName = false } };
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
        /// Using source shape, if possible...
        /// </summary>
        string? GetShape()
        {
            if (options.PreventShape) return null;
            var xoptions = options with { TypeOptions = EasyTypeOptions.Default with { UseName = false } };

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
                    list.Add(string.Format("{0}='{1}'", name, str));
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
                list.Add(string.Format("{0}='{1}'", name, str));
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
        /// Use a rounded header.
        /// </summary>
        string RoundedTypeName(string str) => typename!.Length > 0 ? $"({typename}) {str}" : str;

        /// <summary>
        /// Use a dotted header.
        /// </summary>
        string DottedTypeName(string str) => typename!.Length > 0 ? $"{typename}.{str}" : str;

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
}