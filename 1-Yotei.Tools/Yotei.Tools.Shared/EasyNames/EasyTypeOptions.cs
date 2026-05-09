namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given type-alike element.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// If enabled, then the returned string will be an empty one to be used as a placeholder.
    /// This setting is moslty used when the intent is to obtain an anonymous list of generic
    /// type arguments
    /// </summary>
    public bool UsePlaceHolder { get; set; }

    /// <summary>
    /// If enabled, then use the type's variance (the 'in' and 'out' keywords), if any.
    /// </summary>
    public bool UseVariance { get; set; }

    /// <summary>
    /// If enabled, then use the accessibility modifiers, if any.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use the method's modifiers (such as static, abstract, virtual, override,
    /// new, and ref-alike ones). Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// The style to use to obtain the namespace, if any, of the given type.
    /// <br/> A not-empty value of this property implies <see cref="UseHost"/>.
    /// </summary>
    public EasyNamespaceStyle NamespaceStyle { get; set; }

    /// <summary>
    /// If enabled, then use the type's host, if any (unless it is a special type).
    /// </summary>
    public bool UseHost { get; set; }

    /// <summary>
    /// If enabled, then use the type's special name, if possible.
    /// <br/> If enabled, it shortcircuit the other name-related options.
    /// </summary>
    public bool UseSpecialNames { get; set; }

    /// <summary>
    /// If enabled, then remove the 'Attribute' suffix, if any.
    /// </summary>
    public bool RemoveAttributeSuffix { get; set; }

    /// <summary>
    /// The style to use when the given type is a nullable one.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; set; }

    /// <summary>
    /// The options to use with the generic type arguments, if any. If null, then that list of
    /// generic type arguments is ignored.
    /// <br/> Otherwise, by default, it refers to its host instance to use the same settings.
    /// <br/> To modify how the list is obtained set the value of this property to null, or to
    /// a new instance with the desired settings. For instance, to obtain an anonymous list, the
    /// new instace shall have its own <see cref="UsePlaceHolder"/> value set to true.
    /// </summary>
    public EasyTypeOptions? GenericListOptions { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
    {
        UsePlaceHolder = false;
        UseVariance = false;
        UseAccessibility = false;
        UseModifiers = false;
        NamespaceStyle = EasyNamespaceStyle.None;
        UseHost = false;
        UseSpecialNames = true;
        RemoveAttributeSuffix = false;
        NullableStyle = EasyNullableStyle.None;
        GenericListOptions = null;

        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                RemoveAttributeSuffix = true;
                UseSpecialNames = true;
                NullableStyle = EasyNullableStyle.UseAnnotations;
                GenericListOptions = this;
                break;

            case Mode.Full:
                NamespaceStyle = EasyNamespaceStyle.Default;
                UseHost = true;
                UseSpecialNames = false;
                NullableStyle = EasyNullableStyle.KeepWrappers;
                GenericListOptions = this;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static EasyTypeOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static EasyTypeOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new default-alike instance that includes the namespace and host.
    /// </summary>
    public static EasyTypeOptions DefaultEx => new(Mode.DefaultEx);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static EasyTypeOptions Full => new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => source.EasyName(new EasyTypeOptions());

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
        AddNamespace(sb, source, options, xname, host, isgen);
        AddHost(sb, host, options, xname, isgen, types);
        AddName(sb, source, options, xname);
        AddGenerics(sb, source, options, xname, host, types);
        AddNullability(sb, source, options);
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Invoked when the source is an array-like type.
        /// </summary>
        static string WhenArray(Type source, EasyTypeOptions options)
        {
            var arg = source.GetElementType()!;
            var str = arg.EasyName(options);
            if (str.Length == 0) return string.Empty;

            var rank = source.GetArrayRank();
            str = $"{str}[{new string(',', rank - 1)}]";

            var sb = new StringBuilder();
            AddVariance(sb, source, options);
            sb.Append(str);
            AddNullability(sb, source, options);
            return sb.ToString();
        }

        /// <summary>
        /// Invoked when the source is an pointer-like type.
        /// Note that pointers do not allow nullable annotations (such as 'int*?), but yes they
        /// can point to a nullable type ('int?*').
        /// </summary>
        static string WhenPointer(Type source, EasyTypeOptions options)
        {
            var arg = source.GetElementType()!;
            var str = arg.EasyName(options);
            if (str.Length == 0) return string.Empty;

            var sb = new StringBuilder();
            AddVariance(sb, source, options);
            sb.Append(str);
            sb.Append('*');
            return sb.ToString();
        }

        /// <summary>
        /// Invoked when the source is a nullable wrapper.
        /// </summary>
        static string WhenWrapper(Type source, EasyTypeOptions options)
        {
            if (options.NullableStyle == EasyNullableStyle.UseAnnotations)
            {
                var arg = source.GetGenericArguments()[0];
                var str = arg.EasyName(options);
                if (str.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                AddVariance(sb, source, options); if (sb.Length > 0) sb.Append(' ');
                sb.Append(str);
                if (sb[^1] != '?') sb.Append('?');
                AddNullability(sb, source, options);
                return sb.ToString();
            }
            else if (options.NullableStyle == EasyNullableStyle.KeepWrappers)
            {
                var arg = source.GetGenericArguments()[0];
                var str = arg.EasyName(options);
                if (str.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                AddVariance(sb, source, options);
                var name = GetNullableWrapperName(source, options);
                sb.Append($"{name}<{str}>");
                AddNullability(sb, source, options);
                return sb.ToString();
            }
            else return GetNullableWrapperName(source, options);
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

        /// <summary>
        /// Invoked to add the type's variance (the 'in' and 'out' keywords).
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
        /// Invoked to add the type's namespace.
        /// </summary>
        static void AddNamespace(
            StringBuilder sb, Type source, EasyTypeOptions options,
            string? xname,
            Type? host,
            bool isgen)
        {
            if (xname != null) return;
            if (host != null) return;
            if (isgen) return;
            if (options.NamespaceStyle == EasyNamespaceStyle.None) return;

            var str = source.Namespace;
            if (str == null || str.Length == 0) return;

            if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
            sb.Append(str).Append('.');
        }

        /// <summary>
        /// Invoked to add the type's host.
        /// </summary>
        static void AddHost(
            StringBuilder sb, Type? host, EasyTypeOptions options,
            string? xname,
            bool isgen,
            Type[]? types)
        {
            if (xname != null) return;
            if (host == null) return;
            if (isgen) return;
            if (!options.UseHost && options.NamespaceStyle == EasyNamespaceStyle.None) return;

            var str = host.EasyName(types ?? [], options);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        /// <summary>
        /// Invoked to add the type's name.
        /// </summary>
        static void AddName(
            StringBuilder sb, Type source, EasyTypeOptions options,
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
        /// Invoked to add the type's generic type arguments.
        /// </summary>
        static void AddGenerics(
            StringBuilder sb, Type source, EasyTypeOptions options,
            string? xname,
            Type? host,
            Type[] types)
        {
            if (xname != null) return;
            if (options.GenericListOptions == null) return;

            var xoptions = options.GenericListOptions;
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
        /// Invoked as the last chance to add the type's nullability.
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