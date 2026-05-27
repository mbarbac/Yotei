namespace Yotei.Tools.Generators;

// ========================================================
internal static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given property-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IPropertySymbol source) => source.EasyName(new EasyPropertyOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given property-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyPropertyOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;
        var iface = host != null && host.IsInterface;

        // Accessibility...
        AddAccessibility();
        void AddAccessibility()
        {
            if (!options.UseAccessibility) return;

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

        // Member type...
        AddMemberType();
        void AddMemberType()
        {
            if (options.MemberTypeOptions == null) return;

            var xoptions = options.MemberTypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = source.Type;
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
                    }
                    ;
                }

                // Adding...
                sb.Append(str).Append(' ');
            }
        }

        // Host type...
        AddHostType();
        void AddHostType()
        {
            if (options.HostTypeOptions == null) return;
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
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        var name = source.Name;
        var args = source.Parameters;
        if (args.Length > 0) name = options.UseTechName ? source.MetadataName : "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }
}