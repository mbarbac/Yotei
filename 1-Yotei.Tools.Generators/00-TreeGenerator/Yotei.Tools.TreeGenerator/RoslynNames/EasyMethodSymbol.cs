namespace Yotei.Tools.Generators;

// ========================================================
internal static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given method-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IMethodSymbol source) => source.EasyName(new EasyMethodOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given method-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var iface = host != null && host.IsInterface;
        bool constructor = source.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor;

        // Accessibility...
        AddAccessibility();
        void AddAccessibility()
        {
            if (!options.UseAccessibility) return;
            if (constructor && source.IsStatic) return;

            var str = source.DeclaredAccessibility.ToAccessibilityString(iface);
            if (str != null) sb.Append(str).Append(' ');
        }

        // Modifiers...
        AddModifiers();
        void AddModifiers()
        {
            if (!options.UseModifiers) return;

            if (source.IsNew) sb.Append("new ");
            if (source.IsStatic) sb.Append("static ");
            if (source.IsSealed) sb.Append("sealed ");
            if (!iface && source.IsAbstract) sb.Append("abstract ");

            if (source.IsOverride) sb.Append("override ");
            else if (source.IsVirtual && !iface && !source.IsAbstract) sb.Append("virtual ");

            if (source.IsPartialDefinition ||
                source.PartialDefinitionPart is not null ||
                source.PartialImplementationPart is not null) sb.Append("partial ");
        }

        // Return type...
        AddReturnType();
        void AddReturnType()
        {
            if (options.ReturnTypeOptions == null) return;
            if (constructor) return;

            var xoptions = options.ReturnTypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = source.ReturnType;
            var str = arg.EasyName(xoptions);

            if (str.Length > 0)
            {
                // Nullability...
                var pointer = str.EndsWith('*');
                if (pointer) str = str[..^1].NotNullNotEmpty(trim: false); // Temporary removal...

                while (str[^1] != '?' && xoptions.NullableStyle != EasyNullableStyle.None)
                {
                    if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                        arg.IsNullableWrapper &&
                        arg is not IArrayTypeSymbol &&
                        arg is not IPointerTypeSymbol)
                        break;

                    if (arg.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                    break;
                }
                if (pointer) str += '*'; // Restoring pointer...

                // Modifiers...
                if (options.UseModifiers)
                {
                    switch (source.RefKind)
                    {
                        case RefKind.Ref: sb.Append("ref "); break;
                        case RefKind.Out: sb.Append("out "); break;
                        case RefKind.In: sb.Append("ref readonly "); break;
                    };
                }

                // Adding...
                sb.Append(str).Append(' ');
            }
        }

        // Host type...
        AddHostType();
        void AddHostType()
        {
            if(options.HostTypeOptions == null) return;
            if (host == null) return;

            var xoptions = options.HostTypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            // No top-level nullable annotations, but wrappers are ok...
            if (xoptions.NullableStyle == EasyNullableStyle.UseAnnotations)
                xoptions.NullableStyle = EasyNullableStyle.KeepWrappers;

            var str = host.EasyName(xoptions);
            if (str.Length > 0)
            {
                sb.Append(str);
                if (!constructor) sb.Append('.'); // Only for regular methods!
            }
        }

        // Name...
        if (constructor)
        {
            if (options.HostTypeOptions == null) // Otherwise it has been already captured!
            {
                var str = host?.EasyName() ?? "new";
                sb.Append(str);
            }
            if (options.UseTechName) // Adding the CLR name if requested
            {
                if (!source.Name.StartsWith('.')) sb.Append('.');
                sb.Append(source.Name);
            }
        }
        else
        {
            sb.Append(source.Name); // Regular method's name...
        }

        // Generic arguments (regular methods only)...
        AddGenerics();
        void AddGenerics()
        {
            if (options.GenericListOptions == null) return;
            if (constructor) return;

            var args = source.TypeArguments;
            if (args.Length > 0)
            {
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
                sb.Append('>');
            }
        }

        // Parameters...
        if (options.UseBrackets || options.ParameterOptions != null)
        {
            sb.Append('('); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;
                var args = source.Parameters;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }
}