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
        [AllowNull] this T source) => source.Sketch(SketchOptions.Default);

    /// <summary>
    /// Obtains an alternate string representation of the given value, using the given options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string Sketch<T>([AllowNull] this T source, SketchOptions options)
    {
        // Reflection alike cases...
        switch (source)
        {
            case Type item: if (options.EasyTypeOptions != null) return item.EasyName(options.EasyTypeOptions); break;
            case ParameterInfo item: if (options.EasyParameterOptions != null) return item.EasyName(options.EasyParameterOptions); break;
            case MethodInfo item: if (options.EasyMethodOptions != null) return item.EasyName(options.EasyMethodOptions); break;
            case ConstructorInfo item: if (options.EasyMethodOptions != null) return item.EasyName(options.EasyMethodOptions); break;
            case PropertyInfo item: if (options.EasyPropertyOptions != null) return item.EasyName(options.EasyPropertyOptions); break;
            case FieldInfo item: if (options.EasyFieldOptions != null) return item.EasyName(options.EasyFieldOptions); break;
        }

        // Capturing...
        var type = source == null ? typeof(T) : source.GetType();
        var head = options.HeadOptions == null ? null : type.EasyName(options.HeadOptions);
        string? str;

        MethodInfo? tostring_core = null;
        MethodInfo? tostring_format = null;
        MethodInfo? tostring_provider = null;
        MethodInfo? tostring_format_provider = null;
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

        // Using captured ToString if any (also primitiva values as well)...
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
        /// Returns the representation of the enum-alike value.
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
        /// Returns the representation of the dictionary-alike value.
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
        /// Returns the representation of the non-dictionary collection-alike value.
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
        /// Returns the shape representation of the value, or null if any.
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

            // Properties...
            var props = type.GetProperties(flags);
            foreach (var prop in props)
            {
                var pars = prop.GetIndexParameters();
                var name = pars.Length > 0 ? "this" : prop.Name;

                // If cannot read, or it is an indexer (so we have no way to obtain its arguments'
                // values), just use its name...
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
                // Backing fields not included...
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
        /// Tries to prefix the given value with a rounded brackets header, if possible.
        /// </summary>
        string TryRoundedHead(string value) => head == null ? value : $"({head}) {value}";

        /// <summary>
        /// Tries to prefix the given value with a dotted header, if possible.
        /// </summary>
        string TryDottedHead(string value) => head == null ? value : $"{head}.{value}";

        // ------------------------------------------------

        /// <summary>
        /// Invokes a captured ToString method, if any, or returns null otherwise.
        /// </summary>
        string? InvokeToString()
        {
            if (tostring_format_provider != null) return (string?)tostring_format_provider.Invoke(source, [options.FormatString!, options.FormatProvider!]);
            if (tostring_provider != null) return (string?)tostring_provider.Invoke(source, [options.FormatProvider!]);
            if (tostring_format != null) return (string?)tostring_format.Invoke(source, [options.FormatString!]);
            if (tostring_core != null) return (string?)tostring_core.Invoke(source, null);
            return null;
        }

        /// <summary>
        /// Invoked to capture the relevant 'ToString' methods...
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

                    tostring_format_provider = method;
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

                    tostring_provider = method;
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

                    tostring_format = method;
                    return;
                }
            }

            foreach (var method in methods)
            {
                if (method.DeclaringType == typeof(object)) continue;
                var pars = method.GetParameters();
                if (pars.Length != 0) continue;

                tostring_core = method;
                return;
            }

            if (type.BaseType != null && type != typeof(object)) CaptureToString(type.BaseType);
        }
    }
}