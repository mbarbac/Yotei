namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes where either the 'InvariantList{K,T}' of the 'InvariantList{T}' one is
/// used as its base one. The return types are adjusted to the first direct interface that is
/// an 'IInvariantList' itself, or to the host class type if not found.
/// <br/> 'Clone()' capability is also added automatically if possible.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance that refers to the 'InvariantList{T}' base class.
    /// <br/> Please use 'Nullable{type}' to indicate that the type is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ttype)
    {
        KType = null;
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance that refers to the 'InvariantList{K,T}' base class.
    /// <br/> Please use 'Nullable{type}' to indicate that the type is a nullable one.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the keys of the implemented base class, or null if not used.
    /// </summary>
    public Type? KType { get; }

    /// <summary>
    /// The type of the elements of the base class.
    /// </summary>
    public Type TType { get; }
}