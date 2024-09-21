namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Uses the 'FrozenList{T}' class as the base one of the decorated class.
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class FrozenListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="ttype"></param>
    public FrozenListAttribute(Type ttype)
    {
        KType = typeof(object);
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public FrozenListAttribute(Type ktype, Type ttype)
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