namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces where either the 'IInvariantList{K,T}' of the 'IInvariantList{T}' one
/// is to be implemented. The return types are adjusted to the type of the host interface.
/// <br/> 'Clone()' capability is also added automatically if possible.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance that refers to the 'IInvariantList{T}' interface.
    /// <br/> Please use 'Nullable{type}' to indicate that the type is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ttype)
    {
        KType = null;
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance that refers to the 'IInvariantList{K,T}' interface.
    /// <br/> Please use 'Nullable{type}' to indicate that the type is a nullable one.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the keys of the implemented invariant list interface, or null if not used.
    /// </summary>
    public Type? KType { get; }

    /// <summary>
    /// The type of the elements of the implemented invariant list interface.
    /// </summary>
    public Type TType { get; }
}