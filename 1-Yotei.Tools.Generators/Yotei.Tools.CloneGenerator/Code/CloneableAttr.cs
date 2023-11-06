namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class CloneableAttr
{
    public static string ShortName { get; } = "Cloneable";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    /// <summary>
    /// Invoked to generate the code of this attribute.
    /// </summary>
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
            public class {{LongName}} : System.Attribute
            {
                /// <summary>
                /// Initializes a new instance.
                /// </summary>
                public {{LongName}}() { }
        
                /// <summary>
                /// If 'true' instructs the generator to not produce a virtual-alike method. The
                /// default value of this property is 'false' meaning that the generated methods
                /// are either 'virtual' or 'override' ones.
                /// </summary>
                public string? {{PreventVirtual}} { get; set; }
            }
        }
        """;

    /// <summary>
    /// Tries to obtain the value of the <see cref="PreventVirtual"/> setting on the given
    /// symbol.
    /// </summary>
    public static bool GetPreventVirtual(ISymbol symbol, out bool value)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            var arg = attr.GetNamedArgument(PreventVirtual);
            if (arg != null)
            {
                if (arg.Value.Value is bool temp) { value = temp; return true; }
            }
        }
        value = false;
        return false;
    }
}