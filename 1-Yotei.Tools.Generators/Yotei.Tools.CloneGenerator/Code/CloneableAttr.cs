namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class CloneableAttr
{
    public static string ShortName { get; } = "Cloneable";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string PreventVirtual { get; } = nameof(PreventVirtual);

    // ----------------------------------------------------

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Decorates types for which a 'Clone()' method will be declared or implemented. If
            /// the type is not an interface, it must implement a copy constructor. Records are
            /// not supported.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
                Inherited = false,
                AllowMultiple = false
            )]
            internal class {{LongName}} : Attribute
            {
                /// <summary>
                /// Initializes a new instance.
                /// </summary>
                public {{LongName}}() { }

                /// <summary>
                /// When 'true' instructs the generator to not produce virtual-alike methods. Its
                /// default value is 'false', meaning that the generated methods are either virtual
                /// or overriden ones.
                /// </summary>
                public bool {{PreventVirtual}} { get; set; }
            }
        }
        """;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="PreventVirtual"/> property from the attributes
    /// applied to the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetPreventVirtual(ISymbol symbol, out bool value)
    {
        var ats = symbol.GetAttributes(LongName);
        foreach (var at in ats)
        {
            var arg = at.GetNamedArgument(PreventVirtual);
            if (arg != null &&
                arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = false;
        return false;
    }
}