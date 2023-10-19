namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class CloneableAttr
{
    public static string ShortName { get; } = "Cloneable";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string Specs { get; } = nameof(Specs);
    public static string PreventVirtual { get; } = nameof(PreventVirtual);
    public static string IncludeUnderscores { get; } = nameof(IncludeUnderscores);

    // It is important to keep all settings as named arguments, not as constructors ones, as
    // this permits to identify which ones are explicitly used and where.
    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Used to decorate the types (classes, structs or interfaces) for which a 'Clone'
            /// method will be generated.
            /// </summary>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
                Inherited = false,
                AllowMultiple = false
            )]
            public class {{LongName}} : Attribute
            {
                /// <summary>
                /// Initializes a new instance with the given optional specifications.
                /// </summary>
                public {{LongName}}() { }

                /// <summary>
                /// If not null, then contains the specifications to use to obtain a new instance
                /// of the decorated type. The default value of this property is 'null' meaning
                /// that the generator will try the type constructors.
                /// </summary>
                public string? {{Specs}} { get; set; }

                /// <summary>
                /// If 'true' instructs the generator not to produce virtual-alike methods. The
                /// default value of this property is 'false' meaning that if will generate new,
                /// virtual or overriden ones, as needed.
                /// </summary>
                public bool {{PreventVirtual}} { get; set; }

                /// <summary>
                /// If 'true' instructs the generator to take into consideration the type members
                /// whose names begin with an underscore. The default value of this property is
                /// 'false' meaning that these members will be ignored.
                /// </summary>
                public bool {{IncludeUnderscores}} { get; set; }
            }
        }
        """;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the value of the <see cref="Specs"/> setting.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetSpecs(ISymbol symbol, out string? value)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            var arg = attr.GetNamedArgument(Specs);
            if (arg != null)
            {
                if (arg.Value.IsNull)
                {
                    value = null;
                    return true;
                }
                if (arg.Value.Value is string temp)
                {
                    value = temp;
                    return true;
                }
            }
        }
        value = null;
        return false;
    }

    /// <summary>
    /// Tries to obtain the value of the <see cref="PreventVirtual"/> setting.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetPreventVirtual(ISymbol symbol, out bool value)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            var arg = attr.GetNamedArgument(PreventVirtual);
            if (arg != null)
            {
                if (arg.Value.Value is bool temp)
                {
                    value = temp;
                    return true;
                }
            }
        }
        value = false;
        return false;
    }

    /// <summary>
    /// Tries to obtain the value of the <see cref="IncludeUnderscores"/> setting.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetIncludeUnderscores(ISymbol symbol, out bool value)
    {
        var attrs = symbol.GetAttributes(LongName);
        foreach (var attr in attrs)
        {
            var arg = attr.GetNamedArgument(IncludeUnderscores);
            if (arg != null)
            {
                if (arg.Value.Value is bool temp)
                {
                    value = temp;
                    return true;
                }
            }
        }
        value = false;
        return false;
    }
}