namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class WithGeneratorAttr
{
    public static string ShortName { get; } = "WithGenerator";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string Specs { get; } = nameof(Specs);
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// When used to decorate type members, generates a 'With[name](value)' method for
            /// that member that returns a new instance of the host type where the value of the
            /// decorated member has been replaced by the new given one.
            /// <br/> When used to decorate host types, generates 'With' methods for the decorated
            /// members inherited from their base types and interfaces.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface |
                AttributeTargets.Property | AttributeTargets.Field,
                Inherited = false,
                AllowMultiple = false
            )]
            public class {{LongName}} : Attribute
            {
                /// <summary>
                /// Initializes a new instance.
                /// </summary>
                /// <param name="specs"></param>
                public {{LongName}}(string? specs = null)
                {
                    {{Specs}} = specs == null || (specs = specs.Trim()).Length == 0 ? null : specs;
                }
                
                /// <summary>
                /// If not null contains the specifications of the  method to use to generate a
                /// new instance of the host type. If null, then only the available constructors
                /// will be taken into consideration.
                /// </summary>
                public string? {{Specs}} { get; }
                
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
    /// Returns the value of the <see cref="Specs"/> setting, or null if any.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string? GetSpecs(ISymbol symbol)
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