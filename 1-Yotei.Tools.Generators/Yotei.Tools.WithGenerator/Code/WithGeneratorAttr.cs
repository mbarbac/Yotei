namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class WithGeneratorAttr
{
    public static string ShortName { get; } = "WithGenerator";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string Specs { get; } = nameof(Specs);
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    /// <summary>
    /// Invoked to generate the code of this attribute.
    /// </summary>
    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// When used to decorate type members, generates a 'With[name](value)' method that
            /// returns an instance of the host type where the value of the decorated member has
            /// been replaced by the new given one.
            /// <br/> When used to decorate host types, generates 'With' methods for the members
            /// that were decorated in any parent type, not needing to decorate them again.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface |
                AttributeTargets.Property | AttributeTargets.Field,
                Inherited = false,
                AllowMultiple = false
            )]
            public class {{LongName}} : System.Attribute
            {
                /// <summary>
                /// Initializes a new instance.
                /// </summary>
                public {{LongName}}(string? specs = null)
                {
                    {{Specs}} = string.IsNullOrWhiteSpace(specs) ? null : specs.Trim();
                }
        
                /// <summary>
                /// If not null determines the method to use for implementing the 'With' method.
                /// Acceptable values are 'copy' to use a copy constructor (which is the default
                /// method), 'this' to just modify the value on the current instance, or 'base'
                /// to use a base 'With' method.
                /// </summary>
                public string? {{Specs}} { get; set; }
        
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
    /// Tries to obtain the value of the <see cref="Specs"/> setting on the given symbol.
    /// </summary>
    public static bool GetSpecs(ISymbol symbol, out string? value)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            if (attr is AttributeData data)
            {
                if (data.ConstructorArguments.Length > 0)
                {
                    var item = data.ConstructorArguments[0];
                    if (item.Value is string temp) { value = temp; return true; }
                }
            }

            var arg = attr.GetNamedArgument(Specs);
            if (arg != null)
            {
                if (arg.Value.IsNull) { value = null; return true; }
                if (arg.Value.Value is string temp) { value = temp; return true; }
            }
        }
        value = null;
        return false;
    }

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