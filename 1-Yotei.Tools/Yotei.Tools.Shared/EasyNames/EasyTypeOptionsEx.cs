namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var types = source.GetGenericArguments();
        return source.EasyName(types, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the details of the original closed/bounded generic arguments, if any, have
    /// been captured. Otherwise, through recursion, those details are lost.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    static string EasyName(this Type source, Type[] types, EasyTypeOptions options)
    {
        // Shortcuts...
        if (options.UsePlaceHolder) return string.Empty;
        if (source.IsArray) return WhenArray(source, options);
        if (source.IsPointer) return WhenPointer(source, options);
        if (source.IsNullableWrapper()) return WhenWrapper(source, options);

        // Standard case...
        var sb = new StringBuilder();
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        AddVariance(sb, source, options);
        AddAccessibility(sb, source, options);
        AddModifiers(sb, source, options);
        AddKind(sb, source, options);
        AddNamespace(sb, source, options, host, isgen, xname);
        AddHost(sb, options, host, isgen, xname, types);
        AddName(sb, source, options, xname);
        AddGenerics(sb, source, options, host, xname, types);
        AddNullability(sb, source, options);
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Invoked when the source is an array one.
        /// </summary>
        static string WhenArray(Type source, EasyTypeOptions options)
        {
            var xoptions = options.WithPrefixes(true, false, false, false, false);
            var arg = source.GetElementType()!;
            var str = arg.EasyName(xoptions);
            if (str.Length == 0) return string.Empty;

            var rank = source.GetArrayRank();
            str = $"{str}[{new string(',', rank - 1)}]";

            var sb = new StringBuilder();
            AddVariance(sb, source, options);
            AddAccessibility(sb, source, options);
            AddModifiers(sb, source, options);
            AddKind(sb, source, options);
            if (sb.Length > 0 && sb[^1] != ' ') sb.Append(' ');

            sb.Append(str);
            AddNullability(sb, source, options);
            return sb.ToString();
        }

        /// <summary>
        /// Invoked when the source is a pointer one.
        /// Note that pointers do not allow nullable annotations (such as 'int*?), but yes they
        /// can point to a nullable type ('int?*').
        /// </summary>
        static string WhenPointer(Type source, EasyTypeOptions options)
        {
            var xoptions = options.WithPrefixes(true, false, false, false, false);
            var arg = source.GetElementType()!;
            var str = arg.EasyName(xoptions);
            if (str.Length == 0) return string.Empty;

            var rank = source.GetArrayRank();
            str = $"{str}[{new string(',', rank - 1)}]";

            var sb = new StringBuilder();
            AddVariance(sb, source, options);
            AddAccessibility(sb, source, options);
            AddModifiers(sb, source, options);
            AddKind(sb, source, options);
            if (sb.Length > 0 && sb[^1] != ' ') sb.Append(' ');

            sb.Append(str).Append('*');
            return sb.ToString();
        }

        /// <summary>
        /// Invoked when the source is a nullable wrapper one.
        /// </summary>
        static string WhenWrapper(Type source, EasyTypeOptions options)
        {
            if (options.NullableStyle == EasyNullableStyle.UseAnnotations)
            {
                var xoptions = options.WithPrefixes(true, false, false, false, false);
                var arg = source.GetGenericArguments()[0];
                var str = arg.EasyName(xoptions);
                if (str.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                AddVariance(sb, source, options);
                AddAccessibility(sb, source, options);
                AddModifiers(sb, source, options);
                AddKind(sb, source, options);
                if (sb.Length > 0 && sb[^1] != ' ') sb.Append(' ');

                sb.Append(str);
                if (sb[^1] != '?') sb.Append('?');
                AddNullability(sb, source, options);
                return sb.ToString();
            }
            else if (options.NullableStyle == EasyNullableStyle.KeepWrappers)
            {
                var xoptions = options.WithPrefixes(true, false, false, false, false);
                var arg = source.GetGenericArguments()[0];
                var str = arg.EasyName(xoptions);
                if (str.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                AddVariance(sb, source, options);
                AddAccessibility(sb, source, options);
                AddModifiers(sb, source, options);
                AddKind(sb, source, options);
                if (sb.Length > 0 && sb[^1] != ' ') sb.Append(' ');

                var name = GetNullableWrapperName(source, options);
                sb.Append($"{name}<{str}>");
                AddNullability(sb, source, options);
                return sb.ToString();
            }
            else // No nullable style...
            {
                return GetNullableWrapperName(source, options);
            }
        }

        // Q&D way of obtaining the envelop's name...
        static string GetNullableWrapperName(Type source, EasyTypeOptions options)
        {
            var expand = options.NamespaceStyle != EasyNamespaceStyle.None;
            var global = options.NamespaceStyle == EasyNamespaceStyle.UseGlobal;

            var name = source.IsCoreNullable()
                ? (expand ? "System.Nullable" : "Nullable")
                : (expand ? "Yotei.Tools.IsNullable" : "IsNullable");

            if (global) name = $"global::{name}";
            return name;
        }

        // ------------------------------------------------

        /// <summary>
        /// Invoked to add the element's variance.
        /// </summary>
        static void AddVariance(StringBuilder sb, Type source, EasyTypeOptions options)
        {
            if (!options.UseVariance) return;
            if (!source.IsGenericParameter) return;

            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        /// <summary>
        /// Invoked to add the element's accessibility.
        /// </summary>
        static void AddAccessibility(StringBuilder sb, Type source, EasyTypeOptions options)
        {
            if (!options.UseAccessibility) return;

            if (source.IsPublic) { sb.Append("public "); return; }
            if (source.IsNestedPublic) { sb.Append("public "); return; }
            if (source.IsNestedFamily) { sb.Append("protected "); return; }
            if (source.IsNestedFamORAssem) { sb.Append("protected internal "); return; }
            if (source.IsNestedFamANDAssem) { sb.Append("private protected "); return; }
            if (source.IsNestedPrivate) { sb.Append("private "); return; }
            if (source.IsNestedAssembly) { sb.Append("internal "); return; }
        }

        /// <summary>
        /// Invoked to add the element's modifiers.
        /// </summary>
        static void AddModifiers(StringBuilder sb, Type source, EasyTypeOptions options)
        {
            if (!options.UseModifiers) return;

            if (source.IsAbstract && source.IsSealed) { sb.Append("static "); return; }
            if (source.IsAbstract) { sb.Append("abstract "); return; }
            if (source.IsSealed && !source.IsValueType) { sb.Append("sealed "); return; }
        }

        /// <summary>
        /// Invoked to add the element's kind.
        /// </summary>
        static void AddKind(StringBuilder sb, Type source, EasyTypeOptions options)
        {
            if (!options.UseKind) return;

            if (source.IsInterface) { sb.Append("interface "); return; }
            if (source.IsEnum) { sb.Append("enum "); return; }

            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic;
            var method = source.GetMethod("PrintMembers", flags);
            var record = method != null && method.GetParameters().FirstOrDefault()?.ParameterType == typeof(StringBuilder);
            record = record || source.GetMethod("<Clone>$", flags) != null;
            record = record || source.GetProperties(flags).Any(p => p.Name == "EqualityContract");

            if (source.IsValueType) { sb.Append(record ? "record struct " : "struct "); return; }
            if (source.IsClass)
            {
                if (typeof(Delegate).IsAssignableFrom(source)) { sb.Append("Delegate "); return; }
                sb.Append(record ? "record class " : "class ");
                return;
            }
        }

        /// <summary>
        /// Invoked to add the element's namespace.
        /// </summary>
        static void AddNamespace(StringBuilder sb, Type source, EasyTypeOptions options,
            Type? host,
            bool isgen,
            string? xname)
        {
            if (host != null ||
                isgen ||
                xname != null ||
                options.NamespaceStyle == EasyNamespaceStyle.None) return;

            var str = source.Namespace;
            if (str == null || str.Length == 0) return;

            if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
            sb.Append(str).Append('.');
        }

        /// <summary>
        /// Invoked to add the element's host.
        /// Here we need to use the captured types instead of obtaining new ones.
        /// </summary>
        static void AddHost(StringBuilder sb, EasyTypeOptions options, Type? host,
            bool isgen,
            string? xname,
            Type[] types)
        {
            if (host == null ||
                isgen ||
                xname != null ||
                (!options.UseHost && options.NamespaceStyle == EasyNamespaceStyle.None)) return;

            var xoptions = options.WithPrefixes(
                recursive: true,
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var str = host.EasyName(types, xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        /// <summary>
        /// Invoked to add the element's name.
        /// </summary>
        static void AddName(StringBuilder sb, Type source, EasyTypeOptions options,
            string? xname)
        {
            if (xname != null) { sb.Append(xname); return; }

            var name = source.Name;
            var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
            if (name.Length > 0 && name[^1] == '&') name = name[..^1];

            if (options.RemoveAttributeSuffix &&
                name != ATTRIBUTE &&
                name.EndsWith(ATTRIBUTE)) name = name.RemoveLast(ATTRIBUTE).ToString();

            sb.Append(name);
        }

        /// <summary>
        /// Invoked to add the element's generic type arguments.
        /// Here we use the captured types as a convenience.
        /// </summary>
        static void AddGenerics(StringBuilder sb, Type source, EasyTypeOptions options,
            Type? host,
            string? xname,
            Type[] types)
        {
            if (xname != null ||
                options.GenericListOptions == null) return;

            var xoptions = options.GenericListOptions.WithPrefixes(
                recursive: true,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var args = source.GetGenericArguments();
            var used = host == null ? 0 : host.GetGenericArguments().Length;
            var need = args.Length - used;

            if (need > 0)
            {
                sb.Append('<'); for (int i = 0; i < need; i++)
                {
                    var arg = types[used + i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        /// <summary>
        /// Invoked to add the element's nullability.
        /// </summary>
        static void AddNullability(StringBuilder sb, Type source, EasyTypeOptions options)
        {
            if (sb.Length == 0 || sb[^1] == '?') return;
            if (options.NullableStyle == EasyNullableStyle.None) return;

            if (source.IsNullableWrapper() &&
                options.NullableStyle == EasyNullableStyle.KeepWrappers) return;

            if (source.IsNullableAnnotated()) { sb.Append('?'); return; }
        }
    }
}