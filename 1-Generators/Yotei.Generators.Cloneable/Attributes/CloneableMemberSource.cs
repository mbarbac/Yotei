namespace Yotei.Generators.Cloneable;

// ========================================================
internal static class CloneableMemberSource
{
    static string AttributeShortName = "CloneableMember";
    public static string AttributeLongName => $"{AttributeShortName}Attribute";
    public static string Deep => nameof(Deep);
    public static string Ignore => nameof(Ignore);

    public static string Code(string nameSpace) => $$"""
        namespace {{nameSpace}}
        {
            /// <summary>
            /// Used to decorate the members used for cloning purposes when there is a custom
            /// configuration for them.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Property | AttributeTargets.Field,
                AllowMultiple = false,
                Inherited = false)]
            public class {{AttributeLongName}} : System.Attribute
            {
                /// <summary>
                /// If true, instructs the generator to use a deep clone of the decorated member
                /// instead of just its value.
                /// </summary>
                public bool {{Deep}} { get; set; }

                /// <summary>
                /// If true, instructs the generator to ignore this member when cloning its
                /// containing type.
                /// </summary>
                public bool {{Ignore}} { get; set; }
            }
        }
        """;
}