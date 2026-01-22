namespace Yotei.Tools;

// ========================================================
public static class SketchExtensions
{
    /// <summary>
    /// Obtains an alternate string representation of the given value, using default options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string Sketch<T>(
        this T? source) => source.Sketch(SketchOptions.Default);

    /// <summary>
    /// Obtains an alternate string representation of the given value, using the given options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string Sketch<T>(this T? source, SketchOptions options)
    {
        // Reflection-alike cases...
        var xoptions = options.EasyNameOptions ?? EasyNameOptions.Empty;
        switch (source)
        {
            case Type item: return item.EasyName(xoptions);
            case ParameterInfo item: return item.EasyName(xoptions);
            case MethodInfo item: return item.EasyName(xoptions);
            case ConstructorInfo item: return item.EasyName(xoptions);
            case PropertyInfo item: return item.EasyName(xoptions);
            case FieldInfo item: return item.EasyName(xoptions);
        }

        // Capturing...
        string? str;
        var type = source == null ? typeof(T) : source.GetType();
        var head = !options.UseTypeHead
            ? null
            : type.EasyName(options.EasyNameOptions ?? EasyNameOptions.Default);

        MethodInfo? toString_Core = null;
        MethodInfo? toString_Format = null;
        MethodInfo? toString_Provider = null;
        MethodInfo? toString_Format_Provider = null;
        CaptureToString(type);

        // Trivial cases...
        switch (source)
        {
            case null: return TryRoundedHead(options.NullString ?? string.Empty);
            case string item: return TryRoundedHead(item);
            case char item: return TryRoundedHead(item.ToString());
            case Guid item: return TryRoundedHead(item.ToString());
            case Enum item: return GetEnum();
        }

        // Using captured method, if any (covers primitive values as well)...
        if (!options.PreventToString)
        {
            str = InvokeToString();
            if (str != null) return TryRoundedHead(str);
        }

        // Others...
        if (source is IDictionary dict) return TryRoundedHead(GetDictionary(dict));
        if (source is IEnumerable items) return TryRoundedHead(GetCollection(items));
        if ((str = GetShape()) != null) return TryRoundedHead(str);

        // Finishing...
        return head ?? type.Name;

        // ------------------------------------------------

        /// <summary>
        /// Invoked to obtain the representation of enum-alike values.
        /// </summary>
        string GetEnum()
        {
            var str = InvokeToString()!;
            var names = str.Split(", ", StringSplitOptions.None);

            for (int i = 0; i < names.Length; i++) names[i] = TryDottedHead(names[i]);
            return string.Join(" | ", names);
        }

        // ------------------------------------------------

        /// <summary>
        /// Invoked to obtain the representation of dictionary-alike values.
        /// </summary>
        string GetDictionary(IDictionary source)
        {
            var sb = new StringBuilder();
            var ini = '{';
            var end = '}';
            var first = true;

            sb.Append(ini); foreach (DictionaryEntry kv in source)
            {
                var key = kv.Key.Sketch(options);
                var value = kv.Value.Sketch(options);

                if (first) first = false; else sb.Append(", ");
                sb.AppendFormat("{0} = {1}", key, value);
            }
            sb.Append(end);
            return sb.ToString();
        }

        // ------------------------------------------------

        /// <summary>
        /// Invoked to obtain the representation of not-dictionary collection values.
        /// </summary>
        string GetCollection(IEnumerable source)
        {
            var sb = new StringBuilder();
            var dyn = source is IDynamicMetaObjectProvider;
            var ini = dyn ? '{' : '[';
            var end = dyn ? '}' : ']';
            var first = true;

            sb.Append(ini); var iter = source.GetEnumerator(); while (iter.MoveNext())
            {
                var value = iter.Current.Sketch(options);
                if (first) { first = false; if (dyn) sb.Append(' '); } else sb.Append(", ");
                sb.Append(value);
            }
            if (dyn && !first) sb.Append(' ');
            sb.Append(end);
            return sb.ToString();
        }

        // ------------------------------------------------

        /// <summary>
        /// Invoked to obtain the shape representation of the value, or null if any.
        /// </summary>
        string? GetShape()
        {
            if (!options.UseShape && !options.UsePrivateMembers && !options.UseStaticMembers)
                return null;

            var list = new List<string>();
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (options.UsePrivateMembers) flags |= BindingFlags.NonPublic;
            if (options.UseStaticMembers) flags |= BindingFlags.Static;

            // Properties...
            var props = type.GetProperties(flags);
            foreach (var prop in props)
            {
                var pars = prop.GetIndexParameters();
                var name = pars.Length > 0 ? "this" : prop.Name;

                if (pars.Length > 0 || !prop.CanRead) list.Add(name);
                else
                {
                    var value = prop.GetValue(source);
                    var str = value.Sketch(options);
                    list.Add(string.Format("{0} = {1}", name, str));
                }
            }

            // Fields...
            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                if (field.CustomAttributes.Any(
                    static x => x.AttributeType == typeof(CompilerGeneratedAttribute)))
                    continue;

                var name = field.Name;
                var value = field.GetValue(source);
                var str = value.Sketch(options);
                list.Add(string.Format("{0} = {1}", name, str));
            }

            // Only if we've captured something...
            if (list.Count == 0) return null;
            var sb = new StringBuilder();
            sb.Append("{ "); sb.Append(string.Join(", ", list)); sb.Append(" }");
            return sb.ToString();
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to prefix the value with a rounded brackets type header, if any.
        /// </summary>
        string TryRoundedHead(string value) => head is null ? value : $"({head}) {value}";

        /// <summary>
        /// Tries to prefix the value with a dotted type header, if any.
        /// </summary>
        string TryDottedHead(string value) => head is null ? value : $"{head}.{value}";

        // ------------------------------------------------

        /// <summary>
        /// Invokes the captured 'ToString' method, if any, or returns a null string.
        /// </summary>
        string? InvokeToString()
        {
            if (toString_Format_Provider != null) return (string?)toString_Format_Provider.Invoke(source, [options.FormatString!, options.FormatProvider!]);
            if (toString_Provider != null) return (string?)toString_Provider.Invoke(source, [options.FormatProvider!]);
            if (toString_Format != null) return (string?)toString_Format.Invoke(source, [options.FormatString!]);
            if (toString_Core != null) return (string?)toString_Core.Invoke(source, null);
            return null;
        }

        /// <summary>
        /// Invoked to capture the relevant 'ToString' method at the type's level.
        /// </summary>
        void CaptureToString(Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            var methods = type.GetMethods(flags).Where(static x => x.Name == "ToString");

            if (options.FormatString != null && options.FormatProvider != null)
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

            if (options.FormatProvider != null)
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

            if (options.FormatString != null)
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

            if (type.BaseType != null && type != typeof(object)) CaptureToString(type.BaseType);
        }
    }
}