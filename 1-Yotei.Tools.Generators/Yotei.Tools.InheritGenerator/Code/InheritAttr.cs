namespace Yotei.Tools.InheritGenerator;

// ========================================================
internal static class InheritAttr
{
    public static string ShortName { get; } = "Inherit";
    public static string LongName { get; } = ShortName + "Attribute";
    public static string GenericNames { get; } = nameof(GenericNames);
    public static string ChangeProperties { get; } = nameof(ChangeProperties);

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Decorates types that inherit from a given type upcasting the original elements
            /// whose types are the one being inherited to the derived one. If the derived type
            /// does not specify explicitly the inherited one, then is added to the inheritance
            /// chain.
            /// </summary>
            /// <param name="type">The type to inherit from. If it is itself a generic one,
            /// leave these places blank and use the 'GenericNames' property to provide the
            /// names to use with they.</param>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
                Inherited = false,
                AllowMultiple = true
            )]
            internal class InheritAttribute(Type type) : Attribute
            {
                /// <summary>
                /// The type to inherit from.
                /// </summary>
                public Type Type { get; } = type.ThrowWhenNull();

                /// <summary>
                /// The names to use for the generic arguments of the type to inherit from,
                /// if any. Empty arrays or those containing null or empty elements are not
                /// allowed.
                /// </summary>
                public string[] {{GenericNames}} { get; set; } = [];

                /// <summary>
                /// Whether or not to upcast the properties of the inherited type. The default
                /// value of this property is <see langword="false"/>.
                /// </summary>
                public bool {{ChangeProperties}} { get; set; }
            }
        }
        """;

    // ----------------------------------------------------

    /// <summary>
    /// Gets the value of the <see cref="GenericNames"/> property.
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    public static bool GetGenericNames(AttributeData attr, out string[] value)
    {
        var arg = attr.GetNamedArgument(GenericNames);
        if (arg != null)
        {
            var item = arg.Value;
            if (item.Kind == TypedConstantKind.Array)
            {
                var list = new List<string>();
                foreach (var other in item.Values)
                {
                    list.Add((string)other.Value!);
                }
                value = list.ToArray();
                return true;
            }
        }

        value = default!;
        return false;
    }

    /// <summary>
    /// Gets the value of the <see cref="ChangeProperties"/> property.
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetChangeProperties(AttributeData attr, out bool value)
    {
        var arg = attr.GetNamedArgument(ChangeProperties);
        if (arg != null &&
            arg.Value.Value is bool temp)
        {
            value = temp;
            return true;
        }

        value = default;
        return false;
    }
}