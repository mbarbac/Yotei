namespace Yotei.Tools;

// ========================================================
public static class SketchExtensions
{
    /// <summary>
    /// Returns an alternate string representation of the given source.
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
    public static string Sketch<T>(
        this T? source, SketchOptions options) => new Worker<T>(options, source).Execute();

    /// <summary>
    /// Represents the worker of the Sketch extension method.
    /// </summary>
    class Worker<T>
    {
        readonly SketchOptions Options;
        readonly T? Source;
        readonly Type Type;
        readonly string TypeName;

        MethodInfo? ToString_Format_Provider = null;
        MethodInfo? ToString_Provider = null;
        MethodInfo? ToString_Format = null;
        MethodInfo? ToString_Core = null;

        /// <summary>
        /// Intializes a new worker.
        /// </summary>
        internal Worker(SketchOptions options, T? source)
        {
            Options = options;
            Source = source;
            Type = source is null ? typeof(T) : source.GetType();
            TypeName = options.UseNameSpace || options.UseFullTypeName || options.UseTypeName
                ? Type.EasyName(options.EasyNameOptions)
                : string.Empty;
        }

        /// <summary>
        /// Returns the result of the Sketch method.
        /// </summary>
        internal string Execute()
        {
            if (Source is null) return Rounded(Options.NullStr);

            if (Source is Type _type) return _type.EasyName(Options.EasyNameOptions);
            if (Source is ConstructorInfo _constr) return _constr.EasyName(Options.EasyNameOptions);
            if (Source is MethodInfo _method) return _method.EasyName(Options.EasyNameOptions);
            if (Source is ConstructorInfo _prop) return _prop.EasyName(Options.EasyNameOptions);
            if (Source is ConstructorInfo _field) return _field.EasyName(Options.EasyNameOptions);

            PopulateToStringMethods(Type);

            if (Source is char) return Rounded(InvokeToString()!);
            if (Source is string) return Rounded(InvokeToString()!);
            if (Source is Guid) return Rounded(InvokeToString()!);
            if (Type.IsPrimitive) return Rounded(InvokeToString()!);

            if (Source is Enum) return GetEnumString();
            if (Source is IDictionary dict) return GetDictionaryString(dict);
            if (Source is IEnumerable items) return GetEnumerableString(items);

            var str = InvokeToString();
            if (str != null) return Rounded(str);

            str = GetShape();
            if (str != null) return Rounded(str);

            return TypeName.Length == 0 ? $"<{Type.Name}>" : $"({TypeName})";
        }

        /// <summary>
        /// Uses a rounded header.
        /// </summary>
        string Rounded(string str)
            => Options.UseNameSpace || Options.UseFullTypeName || Options.UseTypeName
            ? $"({TypeName}) {str}"
            : str;

        /// <summary>
        /// Uses a dotted header.
        /// </summary>
        string Dotted(string str)
            => Options.UseNameSpace || Options.UseFullTypeName || Options.UseTypeName
            ? $"{TypeName}.{str}"
            : str;

        /// <summary>
        /// Gets a sketch for an enum source.
        /// </summary>
        string GetEnumString()
        {
            var str = InvokeToString()!;
            var names = str.Split(", ");

            for (int i = 0; i < names.Length; i++) names[i] = Dotted(names[i]);
            return string.Join(" | ", names);
        }

        /// <summary>
        /// Gets a sketch for a dictionary source.
        /// </summary>
        string GetDictionaryString(IDictionary source)
        {
            var sb = new StringBuilder();

            if (Options.UseNameSpace || Options.UseFullTypeName || Options.UseTypeName)
                sb.Append(Rounded(string.Empty));

            var temp = Options with { UseNameSpace = false, UseTypeName = false, UseFullTypeName = false };
            var ini = '{';
            var end = '}';
            sb.Append(ini);

            var first = true; foreach (DictionaryEntry kv in source)
            {
                if (first) first = false; else sb.Append(", ");

                var key = kv.Key.Sketch(temp);
                var value = kv.Value.Sketch(temp);
                sb.AppendFormat("{0} = {1}", key, value);
            }

            sb.Append(end);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a sketch for an enumerable source.
        /// </summary>
        string GetEnumerableString(IEnumerable source)
        {
            var sb = new StringBuilder();

            if (Options.UseNameSpace || Options.UseFullTypeName || Options.UseTypeName)
                sb.Append(Rounded(string.Empty));

            var temp = Options with { UseNameSpace = false, UseTypeName = false, UseFullTypeName = false };
            var isDynamic = source is IDynamicMetaObjectProvider;
            var ini = isDynamic ? '{' : '[';
            var end = isDynamic ? '}' : ']';

            sb.Append(ini);
            var first = true;
            var iter = source.GetEnumerator(); while (iter.MoveNext())
            {
                if (first)
                {
                    first = false;
                    if (isDynamic) sb.Append(' ');
                }
                else sb.Append(", ");

                var value = iter.Current.Sketch(temp);
                sb.Append(value);
            }

            if (isDynamic && !first) sb.Append(' ');
            sb.Append(end);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a sketch using the source's shape, if possible.
        /// </summary>
        string? GetShape()
        {
            if (Options.PreventShape) return null;

            var list = new List<string>();
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (Options.UsePrivateMembers) flags |= BindingFlags.NonPublic;
            if (Options.UseStaticMembers) flags |= BindingFlags.Static;

            var props = Type.GetProperties(flags);
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
                    var value = prop.GetValue(Source, null);
                    var str = value.Sketch(Options);
                    list.Add(string.Format("{0} = {1}", name, str));
                }
            }

            var fields = Type.GetFields(flags);
            foreach (var field in fields)
            {
                if (field.CustomAttributes.Any(
                    x => x.AttributeType == typeof(CompilerGeneratedAttribute)))
                    continue;

                var name = field.Name;
                var value = field.GetValue(Source);
                var str = value.Sketch(Options);
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
        /// Populates the appropriate ToString method, if any is suitable.
        /// </summary>
        void PopulateToStringMethods(Type type)
        {
            if (type == typeof(object)) return;

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = type.GetMethods(flags).Where(x => x.Name == "ToString");

            if (Options.Format != null && Options.Provider != null)
            {
                foreach (var method in methods)
                {
                    var pars = method.GetParameters();
                    if (pars.Length != 2) continue;
                    if (pars[0].ParameterType != typeof(string)) continue;
                    if (!pars[1].ParameterType.IsAssignableFrom(typeof(IFormatProvider))) continue;

                    ToString_Format_Provider = method;
                    return;
                }
            }

            if (Options.Provider != null)
            {
                foreach (var method in methods)
                {
                    var pars = method.GetParameters();
                    if (pars.Length != 1) continue;
                    if (!pars[0].ParameterType.IsAssignableFrom(typeof(IFormatProvider))) continue;

                    ToString_Provider = method;
                    return;
                }
            }

            if (Options.Format != null)
            {
                foreach (var method in methods)
                {
                    var pars = method.GetParameters();
                    if (pars.Length != 1) continue;
                    if (pars[0].ParameterType != typeof(string)) continue;

                    ToString_Format = method;
                    return;
                }
            }

            foreach (var method in methods)
            {
                var pars = method.GetParameters();
                if (pars.Length != 0) continue;
                if (method.DeclaringType == typeof(object)) continue;

                ToString_Core = method;
                return;
            }

            if (type.BaseType is not null) PopulateToStringMethods(type.BaseType);
        }

        /// <summary>
        /// Invokes the appropriate ToString method.
        /// </summary>
        string? InvokeToString()
        {
            object? item = null;

            if (ToString_Format_Provider != null) item = ToString_Format_Provider.Invoke(Source, new object[] { Options.Format!, Options.Provider! });
            else if (ToString_Provider != null) item = ToString_Provider.Invoke(Source, new object[] { Options.Provider! });
            else if (ToString_Format != null) item = ToString_Format.Invoke(Source, new object[] { Options.Format! });
            else if (ToString_Core != null) item = ToString_Core.Invoke(Source, null);

            return item is null ? null : (string)item;
        }
    }
}