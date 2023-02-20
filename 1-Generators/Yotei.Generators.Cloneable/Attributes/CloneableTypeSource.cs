namespace Yotei.Generators.Cloneable;

// ========================================================
/// <summary>
/// Sources for the 'CloneableType' attribute.
/// </summary>
internal static class CloneableTypeSource
{
    public static string Name { get; } = nameof(CloneableTypeSource).Remove("Source") + "Attribute";
    public static string ExplicitMode { get; } = nameof(ExplicitMode);
    public static string PreventAddICloneable { get; } = nameof(PreventAddICloneable);

    public static string Code(string namespaceName)
    {
        namespaceName = namespaceName.NotNullNotEmpty(nameof(namespaceName));

        return $$"""
            namespace {{namespaceName}}
            {
                /// <summary>
                /// Used to decorate the types for which a <see cref="System.ICloneable.Clone"/> method
                /// is to be generated.
                /// </summary>
                [AttributeUsage(
                    AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false,
                    Inherited = false)]
                public class {{Name}} : System.Attribute
                {
                    /// <summary>
                    /// If true, instructs the generator to take into consideration only the members
                    /// decorated with the <see cref="{{CloneableMemberSource.Name}}"/> attribute.
                    /// Otherwise, all available public and protected members will be taken into
                    /// consideration.
                    /// </summary>
                    public bool {{ExplicitMode}} { get; set; }

                    /// <summary>
                    /// If true, instructs the generator not to add the <see cref="System.ICloneable"/>
                    /// interface to the list of interfaces implemented by the type.
                    /// </summary>
                    public bool {{PreventAddICloneable}} { get; set; }
                }
            }
            """;
    }
}