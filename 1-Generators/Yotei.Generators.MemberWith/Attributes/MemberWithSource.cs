namespace Yotei.Generators.MemberWith;

// ========================================================
internal static class MemberWithSource
{
    static string AttributeShortName = "MemberWith";
    public static string AttributeLongName => $"{AttributeShortName}Attribute";

    public static string Code(string nameSpace) => $$"""
        namespace {{nameSpace}}
        {
            /// <summary>
            /// Used to decorate the members of a type for which a 'With...()' method will either
            /// be declared (for interfaces) or implemented (for classes and structs). The name of
            /// the method consist in the 'With' prefix followed by the name of the member (either
            /// a property or a field). Its sole argument takes the new value of that member in
            /// the new instance returned by that method.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Property | AttributeTargets.Field,
                AllowMultiple = false,
                Inherited = false)]
            public class {{AttributeLongName}} : System.Attribute { }
        }
        """;
}