namespace Yotei.Tools.InheritGenerator;

// ========================================================
internal static class InheritAttr
{
    public static string ShortName { get; } = "Inherit";
    public static string LongName { get; } = ShortName + "Attribute";

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Used to decorate the types (interfaces, classes and records) that will inherit from
            /// a given one, specified as its generic argument. If it cannot be fully constructed
            /// because it is itself a generic one, use 'IGeneric' as a placeholder for each generic
            /// argument, and then use the optional list of names to specify which ones to use, in
            /// order.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            [AttributeUsage(
                AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
                Inherited = false,
                AllowMultiple = true
            )]
            internal class {{LongName}}<T> : Attribute
            {
                /// <summary>
                /// Initializes a new instance. It its generic type is itself a generic one, use
                /// in the type definitiion 'IGeneric' as a placeholder, and then the optional
                /// list of names to specify their names.
                /// </summary>
                /// <param name="names"></param>
                public {{LongName}}(params string[] names)
                {
                    if (names == null) throw new ArgumentNullException(nameof(names));
                    if (names.Length == 0) throw new ArgumentException("Generic names is empty.");
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (names[i] == null) throw new ArgumentException("Generic names carries null ones.");
                        if ((names[i] = names[i].Trim()) == null)
                            throw new ArgumentException("Generic names carries empty ones.");
                    }
                    Generics = names;
                }
                
                /// <summary>
                /// Gets the type from which the decorated type will inherit from.
                /// </summary>
                public Type Type => typeof(T);

                /// <summary>
                /// Gets the list of generic names to use, in case the type to inherit from is
                /// itself a generic one.
                /// </summary>
                public string[] Generics { get; }
            }
        }
        """;

    /// <summary>
    /// Gets the collection of inherit elements that decorates the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static IEnumerable<InheritElement> GetElements(ISymbol symbol)
    {
        symbol.ThrowWhenNull();
        return symbol.GetAttributes(LongName).Select(x => new InheritElement(x));
    }
}

// ========================================================
/// <summary>
/// Describes each of the inherit attributes that decorates a given symbol.
/// </summary>
internal class InheritElement
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="attr"></param>
    public InheritElement(AttributeData attr)
    {
        Provider = attr.AttributeClass!.TypeArguments[0];

        var arguments = new List<InheritTypeArgument>();
        if (Provider is INamedTypeSymbol named && named.Arity > 0)
        {
            for (int i = 0; i < named.TypeArguments.Length; i++)
            {
                var symbol = named.TypeArguments[i];
                var generic = false;
                var name = "";

                if (symbol.Name == "IGeneric")
                {
                    name = (string)attr.ConstructorArguments[0].Values[i].Value!;
                    generic = true;
                }

                arguments.Add(new(symbol, generic, name));
            }
        }
        TypeArguments = arguments.ToImmutableArray();
    }

    /// <summary>
    /// The type the attribute refers to, aka: the type whose elements will be inherited.
    /// </summary>
    public ITypeSymbol Provider { get; }

    /// <summary>
    /// The type arguments carried by the provider type, if any.
    /// </summary>
    public ImmutableArray<InheritTypeArgument> TypeArguments { get; }
}

// ========================================================
/// <summary>
/// Describes a type argument of a given inherit provider.
/// </summary>
public class InheritTypeArgument(ITypeSymbol symbol, bool isGeneric, string genericName)
{
    /// <summary>
    /// The type the argument refers to.
    /// </summary>
    public ITypeSymbol Symbol { get; } = symbol.ThrowWhenNull();

    /// <summary>
    /// Determines if this type argument is a generic one, or not.
    /// </summary>
    public bool IsGeneric { get; } = isGeneric;

    /// <summary>
    /// The generic name to use with this argument, or null if it is not a generic one.
    /// </summary>
    public string GenericName { get; } = genericName.NullWhenEmpty()!;
}