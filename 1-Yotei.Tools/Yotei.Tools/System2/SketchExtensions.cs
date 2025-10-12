namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Maintains the '<c>Sketch(...)</c>methods, which return alternate string representations of
/// values using extended options over the standard '<c>ToString()</c>' method.
public static class SketchExtensions
{
    /// <summary>
    /// Returns an alternate string representation of this value, using default options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string Sketch<T>(this T? source) => source.Sketch(SketchOptions.Default);

    /// <summary>
    /// Returns an alternate string representation of this value, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// NOTE: Generic arguments must be captured upfront because when getting the type's host,
    /// the bound information is lost.
    public static string Sketch<T>(this T? source, SketchOptions options)
    {
        string? str;

        // Reflection cases...
        var xoptions = options.ReflectionOptions ?? EasyNameOptions.Default;
        switch (source)
        {
            case Type item: return item.EasyName(xoptions);
            case MethodInfo item: return item.EasyName(xoptions);
            case ConstructorInfo item: return item.EasyName(xoptions);
            case PropertyInfo item: return item.EasyName(xoptions);
            case FieldInfo item: return item.EasyName(xoptions);
        }

        // Capturing type and typename...
        var type = source is null ? typeof(T) : source.GetType();
        var typename = options.SourceTypeOptions == null
            ? null
            : type.EasyName(options.SourceTypeOptions);

        // Capturing ToString methods...
        MethodInfo? toString_Core = null;
        MethodInfo? toString_Format = null;
        MethodInfo? toString_Provider = null;
        MethodInfo? toString_Format_Provider = null;
        CaptureToStringMethods(type);

        // Trivial cases...
        switch (source)
        {
            case null: return TryRoundedHead(options.NullString);
            case string item: return TryRoundedHead(item);
            case char item: return TryRoundedHead(item.ToString());
            case Guid item: return TryRoundedHead(item.ToString());
            case Enum item: return GetEnum();
        }

        // There might be a captured string...
        str = InvokeToString();
        if (str != null) return TryRoundedHead(str);

        // Others...
        if (source is IDictionary dict) return TryRoundedHead(GetDictionary(dict));
        if (source is IEnumerable items) return TryRoundedHead(GetCollection(items));
        if ((str = GetShape()) != null) return TryRoundedHead(str);

        // Finishing...
        return typename ?? type.Name;

        /// <summary>
        /// Gets the value of the source when it is a enum-alike one.  We assume this result is
        /// the one returned, without preceeding with the typename, because if it were not null
        /// it was already used by 'TryDottedHead'.
        /// </summary>
        string GetEnum()
        {
            var str = InvokeToString()!;
            var names = str.Split(", ", StringSplitOptions.None);

            for (int i = 0; i < names.Length; i++) names[i] = TryDottedHead(names[i]);
            return string.Join(" | ", names);
        }

        /// <summary>
        /// Invoked to get the representation of dictionaty-alike values...
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

        /// <summary>
        /// Invoked to get the shape of the collection-alike values..,
        /// </summary>
        string GetCollection(IEnumerable source)
        {
            var sb = new StringBuilder();
            var isdyn = source is IDynamicMetaObjectProvider;
            var ini = isdyn ? '{' : '[';
            var end = isdyn ? '}' : ']';
            var first = true;

            sb.Append(ini); var iter = source.GetEnumerator(); while(iter.MoveNext())
            {
                var value = iter.Current.Sketch(options);

                if (first) { first = false; if (isdyn) sb.Append(' '); }
                else sb.Append(", ");

                sb.AppendFormat(value);
            }

            if (isdyn && !first) sb.Append(' ');
            sb.Append(end);
            return sb.ToString();
        }

        /// <summary>
        /// Invoked to get the shape of the source value.
        /// </summary>
        string? GetShape()
        {
            if (!options.UseShape && !options.UsePrivateMembers && !options.UsePrivateMembers)
                return null;

            var list = new List<string>();
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (options.UsePrivateMembers) flags |= BindingFlags.NonPublic;
            if (options.UseStaticMembers) flags |= BindingFlags.Static;

            var props = type.GetProperties(flags);
            foreach (var prop in props)
            {
                var pars = prop.GetIndexParameters();
                if (pars.Length != 0)
                {
                    // Indexers exist without a-priori values that we can obtain: they are
                    // generated using the arguments provided... that we have not at this
                    // moment because it has not been invoked!.

                    var name = prop.EasyName();
                    list.Add(name);
                }
                else if (prop.CanRead)
                {
                    var name = prop.EasyName();
                    var value = prop.GetValue(source);
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

                var name = field.EasyName();
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

        /// <summary>
        /// Tries to preceed the value with a rounded brackets type header, if any.
        /// </summary>
        string TryRoundedHead(string value) => typename is null ? value : $"({typename}) {value}";

        /// <summary>
        /// Tries to preceed the value with a dotted type header, if any.
        /// </summary>
        string TryDottedHead(string value) => typename is null ? value : $"{typename}.{value}";

        /// <summary>
        /// Invokes the appropriate 'ToString(...)' method, if any was captured.
        /// </summary>
        string? InvokeToString()
        {
            if (toString_Format_Provider != null) return (string?)toString_Format_Provider.Invoke(source, [options.Format!, options.Provider!]);
            if (toString_Provider != null) return (string?)toString_Provider.Invoke(source, [options.Provider!]);
            if (toString_Format != null) return (string?)toString_Format.Invoke(source, [options.Format!]);
            if (toString_Core != null) return (string?)toString_Core.Invoke(source, null);
            return null;
        }

        /// <summary>
        /// Captures the public 'ToString(...)' methods declared at the given type.
        /// </summary>
        void CaptureToStringMethods(Type type)
        {
            if (type == typeof(object)) return; // Exiting recursion...

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