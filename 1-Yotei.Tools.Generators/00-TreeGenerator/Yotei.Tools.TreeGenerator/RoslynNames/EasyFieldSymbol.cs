namespace Yotei.Tools.Generators;

// ========================================================
internal static partial class RoslynNamesExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given field-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IFieldSymbol source) => source.EasyName(new EasyFieldOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given field-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;

        // Accessibility...
        AddAccessibility();
        void AddAccessibility()
        {
            if (!options.UseAccessibility) return;

            var str = source.DeclaredAccessibility.ToAccessibilityString(isInterface: false);
            if (str != null) sb.Append(str).Append(' ');
        }
        
        // Modifiers...
        AddModifiers();
        void AddModifiers()
        {
            if (!options.UseModifiers) return;

            if (source.IsNew) sb.Append("new ");
            if (source.IsStatic) sb.Append("static ");
            if (source.IsConst) sb.Append("const ");
            if (source.IsVolatile) sb.Append("volatile ");
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
                    var temp = source.RefKind switch
                    {
                        RefKind.Ref => sb.Append("ref "),
                        RefKind.Out => sb.Append("out "),
                        RefKind.In => sb.Append("ref readonly "),
                        _ => null
                    };
                    if (temp is null && source.IsReadOnly) sb.Append("readonly ");
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
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}