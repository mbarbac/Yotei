namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which either the 'InvariantList{T}' or the 'InvariantList{K,T}' one is
/// to be used as its base one.
/// <br/> 'Clone()' capability is added automatically if needed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance that refers to the 'IInvariantList{T}' interface.
    /// <br/> Use 'Nullable{type}' to indicate that the type is nullable.
    /// </summary>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ttype)
    {
        KType = null;
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance that refers to the 'IInvariantList{K,T}' interface.
    /// <br/> Use 'Nullable{type}' to indicate that the type is nullable.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// The type of the key for the 'IInvariantList{K,T}' interface reimplementation, or null
    /// if this instance represents a 'IInvariantList{T}' one.
    /// </summary>
    public Type? KType { get; }

    /// <summary>
    /// The type of the 'T' items the reimplementation of the interfaces refer to.
    /// </summary>
    public Type TType { get; }
}