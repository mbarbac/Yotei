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
            /// <br/><br/>
            /// When used to decorate host types, generates 'With' methods for the inherited members
            /// that were decorated in any parent type (including interfaces), withoud the need of
            /// decorating them again.
            /// <br/> Members not inherited need their own decoration.
            /// <br/> Decorated types cannot be records.
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
                /// instance, or 'base' to use a base method. If null or empty, then 'copy' is used
                /// by default.
                /// </summary>
                public string? {{Specs}} { get; set; }
        
                /// <summary>
                /// When 'true' instructs the generator to not produce virtual-alike methods. Its
                /// default value is 'false', meaning that the generated methods are either virtual
                /// or override ones.
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

    ///// <summary>
    /// Tries to get the value of the <see cref="Specs"/> property from the attributes
    /// applied to the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetSpecs(ISymbol symbol, out string? value)
    {
        var ats = symbol.GetAttributes(LongName);
        foreach (var at in ats)
        {
            if (at.ConstructorArguments.Length > 0)
            {
                var item = at.ConstructorArguments[0];
                if (item.Value is string temp)
                {
                    value = temp.NullWhenEmpty();
                    return true;
                }
                if (item.IsNull)
                {
                    // We don't intercept this case: if so, it will end any further downstream
                    // search, preventing using settings from the inheritance chain!
                }
            }

            var arg = at.GetNamedArgument(Specs);
            if (arg != null)
            {
                if (arg.Value.IsNull)
                {
                    value = null;
                    return true;
                }
                if (arg.Value.Value is string temp)
                {
                    value = temp.NullWhenEmpty();
                    return true;
                }
            }
        }

        value = null;
        return false;
    }
}