namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates interface types for which 'IInvariantBag[T]' will be implemented.
/// Use '<see cref="IsNullable{T}"/>' if it is a nullable one.
/// The inherited methods will be upcasted to the decorated host type, unless otherwise specified.
/// Clone functionality is handled automatically.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantBagAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public IInvariantBagAttribute() { }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the elements of the implemented collection.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// The return type of the upcasted methods.
    /// <br/> The default value of this setting is the decorated type itself.
    /// </summary>
    public Type ReturnType { get; set; } = null!;
}