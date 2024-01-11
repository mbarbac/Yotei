namespace Yotei.Tools;

// ========================================================
public static class SketchExtensions
{
    /// <summary>
    /// Returns an alternate string representation of the given source value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string Sketch<T>(this T? source) => source.Sketch(SketchOptions.Default);

    /// <summary>
    /// Returns an alternate string representation of the given source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string Sketch<T>(this T? source, SketchOptions options)
    {
        var type = source is null ? typeof(T) : source.GetType();
        var typeName = options.UseNameSpace || options.UseFullTypeName || options.UseTypeName
            ? type.EasyName(options)
            : string.Empty;

        string? str = source switch
        {
            Type item => item.EasyName(options),
            ConstructorInfo item => item.EasyName(options),
            MethodInfo item => item.EasyName(options),
            PropertyInfo item => item.EasyName(options),
            FieldInfo item => item.EasyName(options),
            null => RoundedTypeName(options.NullStr),
            _ => null
        };
        if (str is not null) return str;

        MethodInfo? toString_Format_Provider = null;
        MethodInfo? toString_Provider = null;
        MethodInfo? toString_Format = null;
        MethodInfo? toString_Core = null;
        CaptureToStringMethods(type);

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

        // This need to come before IDictionary or IEnumerable, as those will actually enumerate
        // which means it may have side effects...
        str = InvokeToString();
        if (str is not null) return RoundedTypeName(str);

        if (source is IDictionary dict) return GetDictionary(dict);
        if (source is IEnumerable items) return GetEnumerable(items);

        str = GetShape();
        if (str is not null) return RoundedTypeName(str);

        return typeName.Length > 0 ? typeName : type.EasyName(options);

        /// <summary>
        /// Use a rounded header.
        /// </summary>
        string RoundedTypeName(string str) => typeName!.Length > 0 ? $"({typeName}) {str}" : str;

        /// <summary>
        /// Use a dotted header.
        /// </summary>
        string DottedTypeName(string str) => typeName!.Length > 0 ? $"{typeName}.{str}" : str;

        /// <summary>
        /// Captures the public <c>ToString</c> methods of the given source.
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

        /// <summary>
        /// Invokes the appropriate <c>ToString</c> method, if any was captured.
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
        /// Enum values...
        /// </summary>
        string GetEnum()
        {
            var str = InvokeToString()!;
            var names = str.Split([", "], StringSplitOptions.None);

            for (int i = 0; i < names.Length; i++) names[i] = DottedTypeName(names[i]);
            return string.Join(" | ", names);
        }

        /// <summary>
        /// Dictionary values...
        /// </summary>
        string GetDictionary(IDictionary source)
        {
            var sb = new StringBuilder();

            if (options.UseNameSpace || options.UseFullTypeName || options.UseTypeName)
                sb.Append(RoundedTypeName(string.Empty));

            var temps = options with
            {
                UseNameSpace = false,
                UseFullTypeName = false,
                UseTypeName = false,
            };
            var ini = '{';
            var end = '}';
            var first = true;

            sb.Append(ini); foreach (DictionaryEntry kv in source)
            {
                var key = kv.Key.Sketch(temps);
                var value = kv.Value.Sketch(temps);

                if (first) first = false;
                else sb.Append(", ");

                sb.AppendFormat("{0} = {1}", key, value);
            }
            sb.Append(end);
            return sb.ToString();
        }

        /// <summary>
        /// Enumeration values...
        /// </summary>
        string GetEnumerable(IEnumerable source)
        {
            var sb = new StringBuilder();

            if (options.UseNameSpace || options.UseFullTypeName || options.UseTypeName)
                sb.Append(RoundedTypeName(string.Empty));

            var temps = options with
            {
                UseNameSpace = false,
                UseFullTypeName = false,
                UseTypeName = false,
            };
            var isdynamic = source is IDynamicMetaObjectProvider;
            var ini = isdynamic ? '{' : '[';
            var end = isdynamic ? '}' : ']';
            var first = true;

            sb.Append(ini);
            var iter = source.GetEnumerator(); while (iter.MoveNext())
            {
                var value = iter.Current.Sketch(temps);

                if (first) { first = false; if (isdynamic) sb.Append(' '); }
                else sb.Append(", ");

                sb.Append(value);
            }

            if (isdynamic && !first) sb.Append(' ');
            sb.Append(end);
            return sb.ToString();
        }

        /// <summary>
        /// Using source shape, if possible...
        /// </summary>
        string? GetShape()
        {
            if (options.PreventShape) return null;

            var list = new List<string>();
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (options.UsePrivateMembers) flags |= BindingFlags.NonPublic;
            if (options.UseStaticMembers) flags |= BindingFlags.Static;

            var props = type.GetProperties(flags);
            foreach (var prop in props)
            {
                if (prop.GetIndexParameters().Length != 0)
                {
                    var name = prop.EasyName(options);
                    list.Add(name);
                }
                else if (prop.CanRead)
                {
                    var name = prop.Name;
                    var value = prop.GetValue(source, null);
                    var str = value.Sketch(options);
                    list.Add(string.Format("{0} = {1}", name, str));
                }
            }

            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                if (field.CustomAttributes.Any(
                    x => x.AttributeType == typeof(CompilerGeneratedAttribute)))
                    continue;

                var name = field.Name;
                var value = field.GetValue(source);
                var str = value.Sketch(options);
                list.Add(string.Format("{0} = {1}", name, str));
            }

            if (list.Count == 0) return null;

            var sb = new StringBuilder();
            sb.Append("{ ");
            sb.Append(string.Join(", ", list));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}