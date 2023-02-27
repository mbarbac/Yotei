namespace Yotei.Generators.Cloneable;

// ========================================================
internal static class CloneableTypeSource
{
    static string AttributeShortName = "CloneableType";
    public static string AttributeLongName => $"{AttributeShortName}Attribute";
    public static string ExplicitMode => nameof(ExplicitMode);
    public static string AddICloneable => nameof(AddICloneable);

    public static string Code(string nameSpace) => $$"""
        namespace {{nameSpace}}
        {
            /// <summary>
            /// Used to decorate the types for which a <see cref="System.ICloneable.Clone"/>
            /// method is to be declared or implemented.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct,
                AllowMultiple = false,
                Inherited = false)]
            public class {{AttributeLongName}} : System.Attribute
            {
                /// <summary>
                /// If true, instructs the generator to take into consideration the type members
                /// that are explicitly decorated with the <see cref="CloneableMemberAttribute"/>
                /// attribute, provided their <see cref"CloneableMemberAttribute.Ignore"/> property
                /// is not set to true.
                /// </summary>
                public bool {{ExplicitMode}} { get; set; }

                /// <summary>
                /// If false, instructs the generator to not add the <see cref"System.ICloneable"/>
                /// interface to the type being generated. The default value of this property is
                /// true.
                /// </summary>
                public bool {{AddICloneable}} { get; set; } = true;
            }
        }
        """;
}