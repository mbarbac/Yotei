namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates class types for which either 'InvariantList[T]' or 'InvariantList[K,T]' will be used
/// at its base one. The inherited methods will be upcasted to the decorated host type, unless
/// otherwise specified. Clone functionality is handled automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement 'IInvariantList[K,T]'.
    /// <br/> Use '<see cref="IsNullable{T}"/>' if any is a nullable one.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance to implement 'IInvariantList[T]'.
    /// <br/> Use '<see cref="IsNullable{T}"/>' if it is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// The type of the keys of the implemented collection, or '<c>null</c>' if not used.
    /// </summary>
    public Type? KType { get; set; }

    /// <summary>
    /// The type of the elements of the implemented collection.
    /// </summary>
    public Type TType { get; set; }

    /// <summary>
    /// The return type of the upcasted methods.
    /// <br/> The default value of this setting is the decorated type itself.
    /// </summary>
    public Type ReturnType { get; set; } = null!;
}