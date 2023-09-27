namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class CloneableAttr
{
    public static string ShortName { get; } = "Cloneable";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string Tag { get; } = nameof(Tag);
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Used to decorate types for which a 'Clone' method will be generated.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
                Inherited = false,
                AllowMultiple = false
            )]
            public class {{LongName}} : Attribute
            {
                /// <summary>
                /// Initializes a new instance.
                /// </summary>
                /// <param name="tag"></param>
                public {{LongName}}(string? tag = null)
                {
                    {{Tag}} = tag == null || (tag = tag.Trim()).Length == 0 ? null : tag;
                }
                
                /// <summary>
                /// If not null describes the method to use to generate a new instance of the
                /// host type. If null, then only the available constructors will be taken into
                /// consideration.
                /// </summary>
                public string? {{Tag}} { get; }
                
                /// <summary>
                /// If true instructs the generator not to use virtual-alike methods, but rather
                /// regular or new ones. The default value of this setting is false.
                /// </summary>
                public bool {{PreventVirtual}} { get; set; }
            }
        }
        """;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the value of the <see cref="Tag"/> setting, or null if any.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string? GetTag(ISymbol symbol)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            var args = attr.ConstructorArguments;
            if (args.Length > 0)
            {
                if (args[0].Value is string value) return value;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the value of the <see cref="PreventVirtual"/> setting, or false if any.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool GetPreventVirtual(ISymbol symbol)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            var arg = attr.GetNamedArgument(PreventVirtual);
            if (arg != null &&
                arg.Value.Value is bool value && value) return value;
        }
        return false;
    }
}