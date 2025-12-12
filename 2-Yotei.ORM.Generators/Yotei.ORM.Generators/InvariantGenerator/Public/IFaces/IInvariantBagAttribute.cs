namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates interface types for which 'IInvariantBag[T]' will be implemented. The inherited
/// methods will be upcasted to the decorated host type, unless otherwise specified. Clone
/// functionality is handled automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantBagAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> Use '<see cref="IsNullable{T}"/>' if it is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    [SuppressMessage("", "IDE0290")]
    public IInvariantBagAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

    // ----------------------------------------------------

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