namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates interfaces for which either 'IInvariantList[K,T]' or 'IInvariantList[T]' will be
/// implemented.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement 'IInvariantList[K,T]'.
    /// <br/> Use 'IsNullable[type]' if any is a nullable one.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance to implement 'IInvariantList[T]'.
    /// <br/> Use 'IsNullable[type]' if it is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ttype)
    {
        KType = null;
        TType = ttype.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the type of the keys in the collection.
    /// </summary>
    public Type? KType { get; set; }

    /// <summary>
    /// The type of the elements in the collection.
    /// </summary>
    public Type TType { get; set; }

    /// <summary>
    /// The return type of the generated methods.
    /// <br/> The default value of this setting is the type of the decorated host.
    /// </summary>
    public Type ReturnType { get; set; } = null!;
}