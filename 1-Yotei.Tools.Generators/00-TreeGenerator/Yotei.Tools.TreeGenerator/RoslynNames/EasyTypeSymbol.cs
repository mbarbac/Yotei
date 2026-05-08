namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(new EasyTypeOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given type-alike element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    public static string EasyName(this ITypeSymbol source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        /* NOTE: When a type has a generic argument as in 'typeof(Predicate<>)', the T-alike
         * element is, for whatever reasons, an 'IErrorTypeSymbol'. As it happens it implements
         * 'INamedTypeSymbol' we can simply ignore.
         */

        // Shortcuts...
        if (options.UsePlaceHolder) return string.Empty;
        if (source is IArrayTypeSymbol array) return WhenArray(array, options);
        if (source is IPointerTypeSymbol pointer) return WhenPointer(pointer, options);
        if (source.IsNullableWrapper()) return WhenWrapper(source, options);

        // Standard case...
        var sb = new StringBuilder();
        var named = source as INamedTypeSymbol;
        var args = named is null ? [] : named.TypeArguments;
        var host = source.ContainingType;
        var isgen = source.IsGenericAlike();
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        AddVariance(sb, source, options);
        AddNamespace(sb, source, options, xname, host, isgen);
        AddHost(sb, host, options, xname, isgen);
        AddName(sb, source, options, xname);
        AddGenerics(sb, source, options, args, xname, host);
        AddNullability(sb, source, options);
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Invoked when the source is an array-like type.
        /// </summary>
        static string WhenArray(IArrayTypeSymbol source, EasyTypeOptions options)
        {
            var arg = source.ElementType;
            var str = arg.EasyName(options);
            if (str.Length == 0) return string.Empty;

            var rank = source.Rank;
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
        static string WhenPointer(IPointerTypeSymbol source, EasyTypeOptions options)
        {
            var arg = source.PointedAtType;
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
        static string WhenWrapper(ITypeSymbol temp, EasyTypeOptions options)
        {
            var source = (INamedTypeSymbol)temp;
            var args = source.TypeArguments;

            if (options.NullableStyle == EasyNullableStyle.UseAnnotations)
            {
                var arg = args[0];
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
                var arg = args[0];
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
        static string GetNullableWrapperName(ITypeSymbol source, EasyTypeOptions options)
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
        static void AddVariance(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options)
        {
            if (!options.UseVariance) return;
            if (source is not ITypeParameterSymbol par) return;

            switch (par.Variance)
            {
                case VarianceKind.Out: sb.Append("out "); break;
                case VarianceKind.In: sb.Append("in "); break;
            }
        }

        /// <summary>
        /// Invoked to add the type's namespace.
        /// </summary>
        static void AddNamespace(
            StringBuilder sb, ITypeSymbol source, EasyTypeOptions options,
            string? xname,
            INamedTypeSymbol? host,
            bool isgen)
        {
            if (xname != null) return;
            if (host != null) return;
            if (isgen) return;
            if (options.NamespaceStyle == EasyNamespaceStyle.None) return;

            var ns = source.ContainingNamespace;
            var str = ns == null ? null : ComputeNamespace(ns, options);
            if (str == null || str.Length == 0) return;

            if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
            sb.Append(str).Append('.');
        }

        // Computes the namespace using the given options (without 'global').
        static string? ComputeNamespace(INamespaceSymbol source, EasyTypeOptions options)
        {
            var names = new List<string>();
            var node = (ISymbol?)source;

            while (node != null) // Using the node as the first one, even if its name may be null...
            {
                if (node is INamespaceSymbol ns &&
                    ns.Name != null &&
                    ns.Name.Length > 0) names.Add(ns.Name);

                node = node.ContainingSymbol;
            }

            if (names.Count == 0) return null; // Return 'null' if no one was found.
            else
            {
                names.Reverse();
                var str = string.Join(".", names);
                return str;
            }
        }

        /// <summary>
        /// Invoked to add the type's host.
        /// </summary>
        static void AddHost(
            StringBuilder sb, INamedTypeSymbol? host, EasyTypeOptions options,
            string? xname,
            bool isgen)
        {
            if (xname != null) return;
            if (host == null) return;
            if (isgen) return;
            if (!options.UseHost && options.NamespaceStyle == EasyNamespaceStyle.None) return;

            var str = host.EasyName(options);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        /// <summary>
        /// Invoked to add the type's name.
        /// It looks like that 'SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix' only
        /// works under very restrictive circumstances, and not in the general case. So, we
        /// cannot use it.
        /// </summary>
        static void AddName(
            StringBuilder sb, ITypeSymbol source, EasyTypeOptions options,
            string? xname)
        {
            if (xname != null) { sb.Append(xname); return; }

            //var misc = default(SymbolDisplayMiscellaneousOptions);
            //if (options.UseSpecialNames) misc |= SymbolDisplayMiscellaneousOptions.UseSpecialTypes;
            //var format = new SymbolDisplayFormat(miscellaneousOptions: misc);
            //var str = source.ToDisplayString(format);

            var str = source.Name;
            if (options.RemoveAttributeSuffix &&
                str != ATTRIBUTE &&
                str.EndsWith(ATTRIBUTE)) str = str.RemoveLast(ATTRIBUTE).ToString();

            sb.Append(str);
        }

        /// <summary>
        /// Invoked to add the type's generic type arguments.
        /// </summary>
        static void AddGenerics(
            StringBuilder sb, ITypeSymbol source, EasyTypeOptions options,
            ImmutableArray<ITypeSymbol> args,
            string? xname,
            INamedTypeSymbol? host)
        {
            if (args.Length == 0) return;
            if (xname != null) return;
            if (options.GenericListOptions == null) return;

            var xoptions = options.GenericListOptions;
            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(xoptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        /// <summary>
        /// Invoked as the last chance to add the type's nullability.
        /// </summary>
        static void AddNullability(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options)
        {
            if (sb.Length == 0 || sb[^1] == '?') return;
            if (options.NullableStyle == EasyNullableStyle.None) return;

            if (source.IsNullableWrapper() &&
                options.NullableStyle == EasyNullableStyle.KeepWrappers) return;

            if (source.IsNullableAnnotated()) { sb.Append('?'); return; }
        }
    }
}