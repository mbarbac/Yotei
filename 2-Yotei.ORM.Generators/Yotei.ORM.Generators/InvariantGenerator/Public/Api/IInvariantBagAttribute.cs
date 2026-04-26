namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates types where the 'IInvariantbag[T]' one will be implemented, including its 'Clone'
/// capabilities.
/// <br/> Includes the interface in the base list if needed.
/// <br/> Derived types must maintain base compatibility.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantBagAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement a '[T]' collection.
    /// </summary>
    /// <param name="ttype"></param>
    public IInvariantBagAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

    // ----------------------------------------------------

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