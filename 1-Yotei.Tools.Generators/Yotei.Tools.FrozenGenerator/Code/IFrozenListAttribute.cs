namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Implements the 'IFrozenList{T}' on the decorated interface.
/// The inherited members are upcasted so that their new return type is the decorated interface.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="ttype"></param>
    public IFrozenListAttribute(Type ttype)
    {
        KType = typeof(object);
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public IFrozenListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// The type of the keys used by elements of the list, if any.
    /// </summary>
    public Type KType { get; }

    /// <summary>
    /// The type of the elements of the list.
    /// </summary>
    public Type TType { get; }
}