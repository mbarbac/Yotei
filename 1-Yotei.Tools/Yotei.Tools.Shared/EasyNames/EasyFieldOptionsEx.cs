namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given element, using default options.
    /// <br/> This method is just a best-effort one because there are syntax elements the compiler
    /// does not keep from the source code.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => source.EasyName(EasyFieldOptions.Default);

    /// <summary>
    /// Obtains a C#-alike representation for a given element, using the given options.
    /// <br/> This method is just a best-effort one because there are syntax elements the compiler
    /// does not keep from the source code.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Accessibility...
        AddAccessibility();
        void AddAccessibility()
        {
            if (!options.UseAccessibility) return;
            if (source == null) return;

            if (source.IsPublic) sb.Append("public ");
            if (source.IsPrivate) sb.Append("private ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        AddModifiers();
        void AddModifiers()
        {
            if (!options.UseModifiers) return;
            if (source == null) return;

            if (IsNew()) sb.Append("new ");

            if (source.IsLiteral) sb.Append("const ");
            else if (source.IsStatic) sb.Append("static ");

            if (IsVolatile()) sb.Append("volatile ");
            if (source.IsInitOnly) sb.Append("readonly ");
        }

        // Member type...
        AddMemberType();
        void AddMemberType()
        {
            if (options.MemberTypeOptions == null) return;
            if (source == null) return;

            var xoptions = options.MemberTypeOptions.WithRecursive(
                useVariance: false,
                useAccessibility: false,
                useModifiers: false,
                useKind: false);

            var arg = source.FieldType;
            var str = arg.EasyName(xoptions);

            if (str.Length > 0)
            {
                // Nullability...
                var pointer = str.EndsWith('*');
                if (pointer) str = str[..^1].NotNullNotEmpty(trim: false); // Temporary removal...

                while (str[^1] != '?' && xoptions.NullableStyle != EasyNullableStyle.None)
                {
                    if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                        arg.IsNullableWrapper() &&
                        !arg.IsArray &&
                        !arg.IsPointer)
                        break;

                    if (arg.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                    break;
                }
                if (pointer) str += '*'; // Restoring pointer...

                // Modifiers...
                if (options.UseModifiers && arg.IsByRef)
                {
                    var ronly =
                        source.HasReadOnlyAttribute() ||
                        arg.HasReadOnlyAttribute();

                    sb.Append(ronly ? "ref readonly " : "ref ");
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

        // ------------------------------------------------

        /// <summary>
        /// Determines if the field is a volatile one.
        /// </summary>
        bool IsVolatile() =>
            source.GetRequiredCustomModifiers().Any(x => x == typeof(IsVolatile) ||
            source.GetOptionalCustomModifiers().Any(x => x == typeof(IsVolatile)));

        /// <summary>
        /// Determines if the given field is a new one.
        /// </summary>
        bool IsNew() => FindBaseField(host) != null;

        FieldInfo? FindBaseField(Type? host)
        {
            if (host != null)
            {
                var parent = host.BaseType;
                while (parent != null)
                {
                    var temp = parent.GetField(
                        source.Name,
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.DeclaredOnly);

                    if (temp != null) return temp;
                    parent = parent.BaseType;
                }
            }
            return null;
        }
    }
}