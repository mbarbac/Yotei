namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class WithGeneratorAttr
{
    public static string ShortName { get; } = "WithGenerator";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string Specs { get; } = nameof(Specs);
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    // ----------------------------------------------------

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// When used to decorate type members, generates a 'With[name](value)' method that 
            /// returns an instance of the host type where the value of the decorated member has
            /// been replaced with the new given one.
            /// <br/>
            /// When used to decorate host types, generates 'With' methods for the inherited members
            /// that were decorated in any parent type (including interfaces), withoud the need of
            /// decorating them again.
            /// <br/>
            /// Types cannot be records.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface |
                AttributeTargets.Property | AttributeTargets.Field,
                Inherited = false,
                AllowMultiple = false
            )]
            internal class {{LongName}} : Attribute
            {
                /// <summary>
                /// Initializes a new instance.
                /// </summary>
                /// <param name="specs"></param>
                public {{LongName}}(string? specs = null)
                {
                    {{Specs}} = string.IsNullOrWhiteSpace(specs) ? null : specs;
                }

                /// <summary>
                /// If not null, determines the method used to generate the 'With' methods: 'copy'
                /// to use a copy constructor of the host type, 'this' to just modify the current
                /// instance, or 'base' to use a base method. If null, then 'copy' is used.
                /// </summary>
                public string? {{Specs}} { get; set; }

                /// <summary>
                /// When 'true' instructs the generator to not produce virtual-alike methods. Its
                /// default value is 'false', meaning that the generated methods are either virtual
                /// or override ones.
                /// </summary>
                bool {{PreventVirtual}} { get; set; }
            }
        }
        """;
}