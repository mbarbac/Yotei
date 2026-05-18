namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ITypeSymbol source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation for a given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        // Shortcuts...
        if (options.UsePlaceHolder) return string.Empty;
        if (source is IArrayTypeSymbol array) return WhenArray(array, options);
        if (source is IPointerTypeSymbol pointer) return WhenPointer(pointer, options);
        if (source.IsNullableWrapper) return WhenWrapper(source, options);

        // Standard case...
        var sb = new StringBuilder();
        var named = source as INamedTypeSymbol;
        var args = named is null ? [] : named.TypeArguments;
        var isgen = source.IsGenericAlike;
        var host = source.ContainingType;
        var xname = options.UseSpecialNames ? source.ToSpecialName() : null;

        AddVariance(sb, source, options);
        AddAccessibility(sb, source, options);
        AddModifiers(sb, source, options);
        AddKind(sb, source, options);
        AddNamespace(sb, source, options, host, isgen, xname);
        AddHost(sb, options, host, isgen, xname);
        AddName(sb, source, options, xname);
        AddGenerics(sb, source, options, host, xname, args);
        AddNullability(sb, source, options);
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Invoked when the source is an array one.
        /// </summary>
        static string WhenArray(IArrayTypeSymbol source, EasyTypeOptions options)
        {
            var xoptions = options.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = source.ElementType;
            var str = arg.EasyName(xoptions);
            if (str.Length == 0) return string.Empty;

            var rank = source.Rank;
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
        static string WhenPointer(IPointerTypeSymbol source, EasyTypeOptions options)
        {
            var xoptions = options.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = source.PointedAtType;
            var str = arg.EasyName(xoptions);
            if (str.Length == 0) return string.Empty;

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
        static string WhenWrapper(ITypeSymbol source, EasyTypeOptions options)
        {
            if (options.NullableStyle == EasyNullableStyle.UseAnnotations)
            {
                var xoptions = options.WithRecursive(
                    useVariance: false,
                    useAccessibility: false,
                    useModifiers: false,
                    useKind: false);

                var arg = ((INamedTypeSymbol)source).TypeArguments[0];
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
                var xoptions = options.WithRecursive(
                    useVariance: false,
                    useAccessibility: false,
                    useModifiers: false,
                    useKind: false);

                var arg = ((INamedTypeSymbol)source).TypeArguments[0];
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

        /// <summary>
        /// Q&D way of obtaining the envelop's name.
        /// </summary>
        static string GetNullableWrapperName(ITypeSymbol source, EasyTypeOptions options)
        {
            var expand = options.NamespaceStyle != EasyNamespaceStyle.None;
            var global = options.NamespaceStyle == EasyNamespaceStyle.UseGlobal;

            var name = source.IsCoreNullable
                ? (expand ? "System.Nullable" : "Nullable")
                : (expand ? "Yotei.Tools.IsNullable" : "IsNullable");

            if (global) name = $"global::{name}";
            return name;
        }

        // ------------------------------------------------

        /// <summary>
        /// Invoked to add the element's variance.
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
        /// Invoked to add the element's accessibility.
        /// </summary>
        static void AddAccessibility(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options)
        {
            if (!options.UseAccessibility) return;

            switch (source.DeclaredAccessibility)
            {
                case Accessibility.Public: sb.Append("public "); return;
                case Accessibility.Protected: sb.Append("protected "); return;
                case Accessibility.Internal: sb.Append("internal "); return;
                case Accessibility.Private: sb.Append("private "); return;
                case Accessibility.ProtectedOrInternal: sb.Append("protected internal "); return;
                case Accessibility.ProtectedAndInternal: sb.Append("private protected "); return;
            }
        }

        /// <summary>
        /// Invoked to add the element's modifiers.
        /// </summary>
        static void AddModifiers(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options)
        {
            if (!options.UseModifiers) return;

            if (source.IsStatic) { sb.Append("static "); return; } // Static evaluated first!

            if (source.IsAbstract) { sb.Append("abstract "); return; }
            if (source.IsSealed) { sb.Append("sealed "); return; }
        }

        /// <summary>
        /// Invoked to add the element's kind.
        /// Supports: enum, interface, struct, record struct, class, record class, and delegate.
        /// </summary>
        static void AddKind(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options)
        {
            if (!options.UseKind) return;

            var ini = sb.Length;

            // records first...
            if (source.IsRecord)
            {
                sb.Append(source.TypeKind == TypeKind.Struct
                    ? "record struct "
                    : "record class ");
            }
            else // Other kinds...
            {
                switch (source.TypeKind)
                {
                    case TypeKind.Class: sb.Append("class "); break;
                    case TypeKind.Struct: sb.Append("struct "); break;
                    case TypeKind.Interface: sb.Append("interface "); break;
                    case TypeKind.Enum: sb.Append("enum "); return;
                    case TypeKind.Delegate: sb.Append("delegate "); break;
                }
            }

            // Intercepting partial keyword...
            var end = sb.Length;
            if (end > ini && source.IsPartial) sb.Insert(ini, "partial ");
        }

        /// <summary>
        /// Invoked to add the element's namespace.
        /// </summary>
        static void AddNamespace(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options,
            INamedTypeSymbol? host,
            bool isgen,
            string? xname)
        {
            if (host != null ||
                isgen ||
                xname != null ||
                options.NamespaceStyle == EasyNamespaceStyle.None) return;

            var ns = source.ContainingNamespace;
            var str = ns == null ? null : Compute(ns, options);
            if (str == null || str.Length == 0) return;

            if (options.NamespaceStyle == EasyNamespaceStyle.UseGlobal) sb.Append("global::");
            sb.Append(str).Append('.');

            /// <summary>
            /// Invoked to compute the namespace, using the given options, not using 'global'
            /// </summary>
            static string? Compute(INamespaceSymbol source, EasyTypeOptions options)
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
        }

        /// <summary>
        /// Invoked to add the element's host.
        /// </summary>
        static void AddHost(StringBuilder sb, EasyTypeOptions options, INamedTypeSymbol? host,
            bool isgen,
            string? xname)
        {
            if (host == null ||
                isgen ||
                xname != null ||
                (!options.UseHost && options.NamespaceStyle == EasyNamespaceStyle.None)) return;

            var xoptions = options.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }


        /// <summary>
        /// Invoked to add the element's name.
        /// </summary>
        static void AddName(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options,
            string? xname)
        {
            if (xname != null) { sb.Append(xname); return; }

            var str = source.Name;
            if (options.RemoveAttributeSuffix &&
                str != ATTRIBUTE &&
                str.EndsWith(ATTRIBUTE)) str = str.RemoveLast(ATTRIBUTE).ToString();

            sb.Append(str);
        }

        /// <summary>
        /// Invoked to add the element's generic type arguments.
        /// Here we use the captured types as a convenience.
        /// </summary>
        static void AddGenerics(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options,
            INamedTypeSymbol? host,
            string? xname,
            ImmutableArray<ITypeSymbol> args)
        {
            if (xname != null || options.GenericListOptions == null) return;
            if (args.Length == 0) return;

            var xoptions = options.GenericListOptions.WithRecursive(
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = arg.EasyName(xoptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
        }

        /// <summary>
        /// Invoked to add the element's nullability.
        /// </summary>
        static void AddNullability(StringBuilder sb, ITypeSymbol source, EasyTypeOptions options)
        {
            if (sb.Length == 0 || sb[^1] == '?') return;
            if (options.NullableStyle == EasyNullableStyle.None) return;

            if (source.IsNullableWrapper &&
                options.NullableStyle == EasyNullableStyle.KeepWrappers) return;

            if (source.IsNullableAnnotated()) { sb.Append('?'); return; }
        }
    }
}
