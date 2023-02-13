namespace Yotei.Generators.Cloneable;

// ========================================================
/// <summary>
/// Sources for the 'CloneableMember' attribute.
/// </summary>
public static class CloneableMemberSource
{
    public static string Name { get; } = nameof(CloneableMemberSource).Remove("Source") + "Attribute";
    public static string Deep { get; } = nameof(Deep);
    public static string Ignore { get; } = nameof(Ignore);

    public static string Code(string namespaceName)
    {
        namespaceName = namespaceName.NotNullNotEmpty(nameof(namespaceName));

        return $$"""
            namespace {{namespaceName}}
            {
                /// <summary>
                /// Used to decorate the members used for cloning purposes, either because in explicit
                /// mode they shall be considered, or because they shall be ignored when cloning their
                /// host type.
                /// </summary>
                [AttributeUsage(
                    AttributeTargets.Property | AttributeTargets.Field,
                    AllowMultiple = false,
                    Inherited = false)]
                public class {{Name}} : System.Attribute
                {
                    /// <summary>
                    /// If true, instructs the generator to use a deep clone of the decorated member
                    /// for cloning purposes, instead of just its value.
                    /// </summary>
                    public bool {{Deep}} { get; set; }
            
                    /// <summary>
                    /// If true, instructs the generator not to use the decorated member for cloning
                    /// purposes.
                    /// </summary>
                    public bool {{Ignore}} { get; set; }
                }
            }
            """;
    }
}