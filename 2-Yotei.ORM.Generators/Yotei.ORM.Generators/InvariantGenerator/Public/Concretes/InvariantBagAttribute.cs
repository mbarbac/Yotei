namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates class types for which 'InvariantBag[T]' will be used as its base one.
/// The inherited methods will be upcasted to the decorated host type, unless otherwise specified.
/// Clone functionality is handled automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantBagAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> Use '<see cref="IsNullable{T}"/>' if it is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    [SuppressMessage("", "IDE0290")]
    public InvariantBagAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

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