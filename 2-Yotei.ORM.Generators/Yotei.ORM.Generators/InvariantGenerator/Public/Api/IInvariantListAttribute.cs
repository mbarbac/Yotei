#nullable enable
namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates types where either the 'IInvariantList[K,T]' interface or the 'IInvariantList[T]'
/// one will be implemented, including their 'Clone' capabilities.
/// <br/> Includes the interface in the base list if needed.
/// <br/> Derived types must maintain base compatibility.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement a '[T]' collection.
    /// </summary>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance to implement a '[K, T]' collection.
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
    /// If not '<see langword="null"/>', then the type of the keys the '[K,T]' collection are
    /// associated with. If '<see langword="null"/>', then the collection is a '[T]' one.
    /// </summary>
    public Type? KType { get; }

    /// <summary>
    /// The type of the collection's elements.
    /// </summary>
    public Type TType { get; }

    /// <summary>
    /// If not null, specifies the return type of the generated method. Otherwise, the decorated
    /// host type will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;
}