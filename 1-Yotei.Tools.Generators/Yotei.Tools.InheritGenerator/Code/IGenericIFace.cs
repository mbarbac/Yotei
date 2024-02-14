namespace Yotei.Tools.InheritGenerator;

// ========================================================
internal static class IGenericIFace
{
    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Used as a placeholder for generic arguments in attributes when they cannot be
            /// fully constructed.
            /// </summary>
            internal interface IGeneric { }
        }
        """;
}