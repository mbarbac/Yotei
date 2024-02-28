namespace Yotei.Tools.UpcastGenerator;

// ========================================================
internal static class UpcastAttr
{
    public static string ShortName { get; } = "Upcast";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string ChangeProperties { get; } = nameof(ChangeProperties);
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    // ----------------------------------------------------

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Decorates types for which upcasting the elements of a base type or inherited
            /// interface is requested. Types to upcast must appear in the inheritance chain of
            /// the decorated syntax node and, if needed, identified by their index as specified
            /// in the appropriate constructor. Several attributes can be applied to the same
            /// symbol, the settings of the later overrwrite the ones of the former.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct,
                Inherited = false,
                AllowMultiple = true
            )]
            internal class {{LongName}} : Attribute
            {
                /// <summary>
                /// Upcasts the relevant elements of the base class and interfaces in the
                /// associated inheritance list.
                /// </summary>
                public {{LongName}}() { }

                /// <summary>
                /// Upcasts the relevant elements of the base class or interface whose zero-based
                /// index in the associated inheritance list is given.
                /// </summary>
                /// <param name="index"></param>
                public {{LongName}}(int index) { }

                /// <summary>
                /// Upcasts the relevant elements of the base class and interfaces whose zero-based
                /// indexes in the associated inheritance list are given.
                /// </summary>
                /// <param name="indexes"></param>
                public {{LongName}}(int[] indexes) { }

                /// <summary>
                /// True to change the inherited properties, reimplementing them with a 'new'
                /// keyword. The default value of this property is 'false' because C# does not
                /// support covariant overriden properties (unless they are read-only, which is
                /// not a supported case).
                /// </summary>
                public bool {{ChangeProperties}} { get; set; }
        
                /// <summary>
                /// When 'true' instructs the generator to not produce virtual-alike methods. Its
                /// default value is 'false' so that the generated methods are either virtual or
                /// overriden ones, if possible.
                /// </summary>
                public bool {{PreventVirtual}} { get; set; }
            }
        }
        """;
}