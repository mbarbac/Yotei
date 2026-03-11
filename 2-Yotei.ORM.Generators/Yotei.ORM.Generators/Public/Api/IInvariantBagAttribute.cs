namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which 'IInvariantBag[T]' will be used as a base one, and its methods
/// (including clone) redeclared.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantBagAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    [SuppressMessage("", "IDE0290")]
    public IInvariantBagAttribute(Type type) => TType = type.ThrowWhenNull();

    /// <summary>
    /// The type of the elements of the decorated collection.
    /// </summary>
    public Type TType { get; }

    /// <summary>
    /// If not <see langword="null"/>, then specifies the return type of the generated methods.
    /// Otherwise, the type of the decorated host will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;
}